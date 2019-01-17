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
    private ConfirmationDialog confirmationDialog;
    [SerializeField]
    private Navigator navigator;
    [SerializeField]
    private Transform propertyAdminScreenTransform;
    [SerializeField]
    private Text roomAdminScreenTitle;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField;
    
    public void DeleteRoom()
    {
        confirmationDialog.Show(() => {
            PropertyDataManager.GetProperty(currentProperty.ID).DeleteRoom(currentRoom.ID);
            navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
            propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().InstantiateRooms();
        }, null);
    }

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

    public void OnRoomSingleBedQuantityValueChanged(string value)
    {
        currentRoom.SingleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void OnRoomDoubleBedQuantityValueChanged(string value)
    {
        currentRoom.DoubleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }
}
