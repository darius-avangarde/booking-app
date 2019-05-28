using System.Collections;

using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Text propertyScreenTitle = null;
    [SerializeField]
    private InputField propertyNameInputField = null;
    [SerializeField]
    private Toggle HasRoomsToggle;
    [SerializeField]
    private Toggle NoRoomsToggle;
    [SerializeField]
    private GameObject RoomsToggleField;
    [SerializeField]
    private Button backButton;
    private IProperty currentProperty;

    private void Start()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
    }

    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
        if(currentProperty.Name != null)
        {
            RoomsToggleField.SetActive(false);
        }
        else
        {
            RoomsToggleField.SetActive(true);
        }
    }

    public void SetPropertyFieldsText()
    {
        propertyNameInputField.text = currentProperty.Name ?? "";
        propertyScreenTitle.text = currentProperty.Name ?? Constants.NEW_PROPERTY;
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
        navigator.GoBack();
    }

    public void DeleteProperty()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_PROPERTY,
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () =>
            {
                PropertyDataManager.DeleteProperty(currentProperty.ID);
                ReservationDataManager.DeleteReservationsForProperty(currentProperty.ID);
                navigator.GoBack();
            },
            CancelCallback = null
        });
    }

    public void NameChanged(string value)
    {
        propertyScreenTitle.text = value;
        currentProperty.Name = string.IsNullOrEmpty(value) ? Constants.NEW_PROPERTY : value;
    }
}
