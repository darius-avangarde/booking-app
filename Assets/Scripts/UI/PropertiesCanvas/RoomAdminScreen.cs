using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    public IProperty currentProperty;
    private IRoom currentRoom;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private Text roomAdminScreenTitle = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;

    public void DeleteRoom()
    {
        // TODO: made the cancelCallback optional, we can remove the second argument
        confirmationDialog.Show(() => {
            // TODO: we already have a reference to currentProperty, no need to get it again
            PropertyDataManager.GetProperty(currentProperty.ID).DeleteRoom(currentRoom.ID);
            navigator.GoBack();
            // TODO: we can probably remove this, PropertyAdminScreen should deal with initializing itself through NavScreen.Showing
            propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().InstantiateRooms();
        }, null);
    }

    // TODO: we should let RoomAdminScreen initialize itself (through NavScreen.Showing)
    // this way we can limit other scrips' access to simply setting the current property
    // This means we should probably have something like a SetRoom method called by PropertyAdminScreen and an Initialize method called through NavScreen.Showing
    public void UpdateRoomFields(IProperty property, IRoom room)
    {
        currentProperty = property;
        currentRoom = room;
        roomNameInputField.text = currentRoom.Name ?? "";
        roomSingleBedQuantityInputField.text = room.SingleBeds.ToString();
        roomDoubleBedQuantityInputField.text = room.DoubleBeds.ToString();
        roomAdminScreenTitle.text = currentRoom.Name ?? Constants.defaultRoomAdminScreenName;
    }

    public void OnRoomNameValueChanged(string value)
    {
        roomAdminScreenTitle.text = value;
        currentRoom.Name = string.IsNullOrEmpty(value) ? Constants.defaultRoomAdminScreenName : value;
    }

    // TODO: maybe calling this OnSingleBedsChanged is easier to read but still tells us enough information about what it does
    public void OnRoomSingleBedQuantityValueChanged(string value)
    {
        if (value == "-")
        {
            roomSingleBedQuantityInputField.text = "";
            return;
        }
        currentRoom.SingleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    // TODO: OnDoubleBedsChanged maybe?
    public void OnRoomDoubleBedQuantityValueChanged(string value)
    {
        if (value == "-")
        {
            roomDoubleBedQuantityInputField.text = "";
            return;
        }
        currentRoom.DoubleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    // TODO: is this used anywhere? we should remove it if not
    public void ChangeSingleBedQuantity(string sign)
    {
        if (sign == "+")
        {
            roomSingleBedQuantityInputField.text = (++currentRoom.SingleBeds).ToString();
        }
        else
        {
            if (roomSingleBedQuantityInputField.text != "0")
            {
                roomSingleBedQuantityInputField.text = (--currentRoom.SingleBeds).ToString();
            }
        }
    }

    // TODO: is this used anywhere? we should remove it if not
    public void ChangeDoubleBedQuantity(string sign)
    {
        if (sign == "+")
        {
            roomDoubleBedQuantityInputField.text = (++currentRoom.DoubleBeds).ToString();
        }
        else
        {
            if (roomDoubleBedQuantityInputField.text != "0")
            {
                roomDoubleBedQuantityInputField.text = (--currentRoom.DoubleBeds).ToString();
            }
        }
    }
}
