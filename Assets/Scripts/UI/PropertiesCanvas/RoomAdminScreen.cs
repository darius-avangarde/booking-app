using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Text roomAdminScreenTitle = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;

    private IProperty currentProperty;
    private IRoom currentRoom;

    public void SetCurrentPropertyRoom(IProperty property, IRoom room)
    {
        currentProperty = property;
        currentRoom = room;
    }

    public void Initialize()
    {
        roomNameInputField.text = currentRoom.Name ?? "";
        roomSingleBedQuantityInputField.text = currentRoom.SingleBeds.ToString();
        roomDoubleBedQuantityInputField.text = currentRoom.DoubleBeds.ToString();
        roomAdminScreenTitle.text = currentRoom.Name ?? Constants.defaultRoomAdminScreenName;
    }

    public void SaveRoom()
    {
        OnRoomNameValueChanged(roomNameInputField.text);
        OnSingleBedsChanged(roomSingleBedQuantityInputField.text);
        OnDoubleBedsChanged(roomDoubleBedQuantityInputField.text);
        if (currentProperty.GetRoom(currentRoom.ID) == null)
        {
            currentProperty.SaveRoom(currentRoom);
        }
        navigator.GoBack();
    }

    public void DeleteRoom()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Ștergeți camera?",
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = ()=> {
                currentProperty.DeleteRoom(currentRoom.ID);
                ReservationDataManager.DeleteReservationsForRoom(currentRoom.ID);
                navigator.GoBack();
            },
            CancelCallback = null
        });
    }

    public void OnRoomNameValueChanged(string value)
    {
        roomAdminScreenTitle.text = value;
        currentRoom.Name = string.IsNullOrEmpty(value) ? Constants.defaultRoomAdminScreenName : value;
    }

    public void OnSingleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomSingleBedQuantityInputField.text = "";
            return;
        }
        currentRoom.SingleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void OnDoubleBedsChanged(string value)
    {
        if (value == "-")
        {
            roomDoubleBedQuantityInputField.text = "";
            return;
        }
        currentRoom.DoubleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void IncrementSingleBedQuantity()
    {
        roomSingleBedQuantityInputField.text = (++currentRoom.SingleBeds).ToString();
    }

    public void DecrementSingleBedQuantity()
    {
        if (roomSingleBedQuantityInputField.text != "0")
        {
            roomSingleBedQuantityInputField.text = (--currentRoom.SingleBeds).ToString();
        }
    }

    public void IncrementDoubleBedQuantity()
    {
        roomDoubleBedQuantityInputField.text = (++currentRoom.DoubleBeds).ToString();
    }

    public void DecrementDoubleBedQuantity()
    {
        if (roomDoubleBedQuantityInputField.text != "0")
        {
            roomDoubleBedQuantityInputField.text = (--currentRoom.DoubleBeds).ToString();
        }
    }
}
