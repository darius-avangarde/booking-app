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
    private IProperty currentProperty;

    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
        if (property.HasRooms)
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

    public void SetPropertyFieldsText()
    {
        propertyNameInputField.text = currentProperty.Name ?? "";
        propertyScreenTitle.text = currentProperty.Name ?? Constants.defaultProperyAdminScreenName;
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
            Message = "Ștergeți proprietatea?",
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
        currentProperty.Name = string.IsNullOrEmpty(value) ? Constants.defaultProperyAdminScreenName : value;
    }
}
