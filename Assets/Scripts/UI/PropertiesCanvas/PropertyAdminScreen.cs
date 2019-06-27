﻿using System.Collections;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private RoomAdminScreen roomAdminScreen = null;
    [SerializeField]
    private InputField propertyNameInputField = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private AspectRatioFitter propertyImageAspectFitter = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private AspectRatioFitter backgroundImageAspectFitter = null;
    [SerializeField]
    private GameObject RoomsToggleField = null;
    [SerializeField]
    private Toggle HasRoomsToggle = null;
    [SerializeField]
    private Toggle NoRoomsToggle = null;
    [SerializeField]
    private Button saveButton = null;
    [SerializeField]
    private Button addPhotoButton = null;
    [SerializeField]
    private Button deleteButton = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button calcelButton = null;
    [SerializeField]
    private Text errorMessage = null;
    [SerializeField]
    private Text saveButtonText = null;

    private IProperty currentProperty;
    private bool canSave = true;
    private bool editProperty = false;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        addPhotoButton.onClick.AddListener(() => AddPhoto());
        deleteButton.onClick.AddListener(() => DeleteProperty());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
    }

    /// <summary>
    /// set the current property
    /// </summary>
    /// <param name="property">selected property</param>
    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
        if (currentProperty.Name != null)
        {
            RoomsToggleField.SetActive(false);
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            RoomsToggleField.SetActive(true);
            deleteButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// update screen details with the current property
    /// update name, property photo and rooms toggle
    /// </summary>
    private void SetPropertyFieldsText()
    {
        ResetError();
        propertyNameInputField.text = currentProperty.Name ?? "";
        if (ImageDataManager.PropertyPhotos.ContainsKey(currentProperty.ID))
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
            backgroundImage.sprite = (Sprite)ImageDataManager.BlurPropertyPhotos[currentProperty.ID];
            backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[Constants.defaultPropertyPicture];
            backgroundImage.gameObject.SetActive(false);
        }
        propertyImageAspectFitter.aspectRatio = backgroundImageAspectFitter.aspectRatio = (float)propertyImage.sprite.texture.width / propertyImage.sprite.texture.height;

        if (string.IsNullOrEmpty(currentProperty.Name))
        {
            editProperty = false;
            HasRoomsToggle.isOn = false;
            NoRoomsToggle.isOn = false;
            saveButton.interactable = false;
            saveButtonText.color = Constants.lightTextColor;
        }
        else
        {
            editProperty = true;
            if (currentProperty.HasRooms)
            {
                HasRoomsToggle.isOn = true;
                NoRoomsToggle.isOn = false;
            }
            else
            {
                NoRoomsToggle.isOn = true;
                HasRoomsToggle.isOn = false;
            }
        }
        ImageDataManager.AddedPhoto = false;
    }

    /// <summary>
    /// on add photo button
    /// open modal toggle dialog to select camera output or gallery
    /// </summary>
    private void AddPhoto()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            AditionalCallback = true,
            Message = Constants.OPEN_CAMERA_GALLERY,
            ConfirmText = Constants.OPEN_CAMERA,
            ConfirmTextSecond = Constants.OPEN_GALLERY,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                ImageDataManager.TakePhoto(currentProperty.ID, propertyImage, backgroundImage, propertyImageAspectFitter, backgroundImageAspectFitter);
            },
            ConfirmCallbackSecond = () =>
            {
                ImageDataManager.PickImage(currentProperty.ID, propertyImage, backgroundImage, propertyImageAspectFitter, backgroundImageAspectFitter);
            },
            CancelCallback = null
        });
    }

    /// <summary>
    /// save information for current property
    /// </summary>
    public void SaveProperty()
    {
        NameChanged(propertyNameInputField.text);
        if (canSave)
        {
            if (HasRoomsToggle.isOn)
            {
                currentProperty.HasRooms = true;
            }
            else
            {
                currentProperty.HasRooms = false;
                PropertyDataManager.CreatePropertyRoom(currentProperty);
                currentProperty.GetPropertyRoom().Name = currentProperty.Name;
            }
            if (ImageDataManager.AddedPhoto)
            {
                ImageDataManager.SaveImage(currentProperty.ID, propertyImage.sprite.texture, backgroundImage.sprite.texture);
            }
            if (PropertyDataManager.GetProperty(currentProperty.ID) == null)
            {
                PropertyDataManager.SaveProperty(currentProperty);
            }
            if (currentProperty.HasRooms && !editProperty)
            {
                OpenRoomAdminScreen(currentProperty);
            }
            else
            {
                navigator.GoBack();
            }
        }
    }

    /// <summary>
    /// delete current property
    /// open modal dialog canvas
    /// </summary>
    private void DeleteProperty()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_PROPERTY,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                PropertyDataManager.DeleteProperty(currentProperty.ID);
                ReservationDataManager.DeleteReservationsForProperty(currentProperty.ID);
                ImageDataManager.DeletePropertyPhoto(currentProperty.ID);
                navigator.GoBack();
                navigator.GoBack();
            },
            CancelCallback = null
        });
    }

    /// <summary>
    /// make save button interactable or not if any toggle is selected
    /// </summary>
    public void SaveInteractable()
    {
        if(HasRoomsToggle.isOn || NoRoomsToggle.isOn)
        {
            saveButton.interactable = true;
            saveButtonText.color = Color.white;
        }
        else
        {
            saveButton.interactable = false;
            saveButtonText.color = Constants.lightTextColor;
        }
    }

    /// <summary>
    /// reset error message and bool to default
    /// </summary>
    public void ResetError()
    {
        canSave = true;
        errorMessage.text = string.Empty;
    }

    /// <summary>
    /// change current property name
    /// </summary>
    /// <param name="value">string with the new name</param>
    private void NameChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            errorMessage.text = Constants.ERR_PROPERTY_NAME;
            canSave = false;
        }
        else
        {
            currentProperty.Name = value;
            ResetError();
        }
    }

    /// <summary>
    /// open add room screen after you create a new property with rooms
    /// </summary>
    /// <param name="property"> selected property</param>
    private void OpenRoomAdminScreen(IProperty property)
    {
        navigator.GoTo(roomAdminScreen.GetComponent<NavScreen>());
        roomAdminScreen.SetCurrentProperty(property);
    }
}
