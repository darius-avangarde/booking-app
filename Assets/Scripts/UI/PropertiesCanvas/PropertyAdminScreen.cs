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
    private Button addPhotoButton = null;
    [SerializeField]
    private Button deleteButton = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button calcelButton = null;
    [SerializeField]
    private Text errorMessage = null;

    private IProperty currentProperty;
    private bool canSave = true;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
    }

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
        //SetPropertyFieldsText();
    }

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
        ImageDataManager.AddedPhoto = false;
    }

    public void AddPhoto()
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
            navigator.GoBack();
        }
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

    public void ResetError()
    {
        canSave = true;
        errorMessage.text = string.Empty;
    }

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
}
