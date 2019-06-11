using System.Collections;
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
    private InputField propertyNameInputField = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private GameObject RoomsToggleField = null;
    [SerializeField]
    private Toggle HasRoomsToggle = null;
    [SerializeField]
    private Toggle NoRoomsToggle = null;
    [SerializeField]
    private Button addPhotoButton = null;
    [SerializeField]
    private Button deleteButton = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button calcelButton = null;

    private IProperty currentProperty;
    private bool addedPhoto = false;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
    }

    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
        if(currentProperty.Name != null)
        {
            RoomsToggleField.SetActive(false);
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            RoomsToggleField.SetActive(true);
            deleteButton.gameObject.SetActive(false);
        }
        //SetPropertyFieldsText();
    }

    private void SetPropertyFieldsText()
    {
        propertyNameInputField.text = currentProperty.Name ?? "";
        if (ImageDataManager.PropertyPhotos.ContainsKey(currentProperty.ID))
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
            backgroundImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
            backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[Constants.defaultPropertyPicture];
            backgroundImage.gameObject.SetActive(false);
        }
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

    public void AddPhoto()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Deschideți galeria sau camera foto?",
            ConfirmText = "Camera",
            CancelText = "Galerie",
            ConfirmCallback = () =>
            {
                ImageDataManager.TakePhoto(currentProperty.ID, propertyImage);
                addedPhoto = true;
            },
            CancelCallback = () =>
            {
                ImageDataManager.PickImage(currentProperty.ID, propertyImage);
                addedPhoto = true;
            }
        });
    }

    public void SaveProperty()
    {
        NameChanged(propertyNameInputField.text);
        if (HasRoomsToggle.isOn)
        {
            currentProperty.HasRooms = true;
        }
        else
        {
            currentProperty.HasRooms = false;
        }
        if (PropertyDataManager.GetProperty(currentProperty.ID) == null)
        {
            PropertyDataManager.SaveProperty(currentProperty);
        }
        if (addedPhoto)
        {
            ImageDataManager.SaveImage(currentProperty.ID, propertyImage.sprite.texture);
        }
        navigator.GoBack();
    }

    public void DeleteProperty()
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

    private void NameChanged(string value)
    {
        currentProperty.Name = string.IsNullOrEmpty(value) ? Constants.NEW_PROPERTY : value;
        if (!currentProperty.HasRooms)
        {
            currentProperty.GetPropertyRoom().Name = currentProperty.Name;
        }
    }

    private IEnumerator UploadPhoto()
    {
        yield return null;
        //yield return WaitUntil((t) => ImageDataManager.TakePhoto(currentProperty.ID));
        propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
    }
}
