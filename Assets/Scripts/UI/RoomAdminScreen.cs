using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    public IProperty currentProperty;
    private IRoom currentRoom;
    [SerializeField]
    private Text roomAdminScreenTitle;
    [SerializeField]
    private InputField roomNameInputField;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void DeleteRoom()
    {
        PropertyDataManager.GetProperty(currentProperty.ID).DeleteRoom(currentRoom.ID);
    }

    public void UpdateRoomFields(IProperty property, IRoom room)
    {
        currentProperty = property;
        currentRoom = room;
        roomNameInputField.text = currentRoom.Name ?? Constants.defaultRoomName;
        roomSingleBedQuantityInputField.text = room.SingleBeds.ToString();
        roomDoubleBedQuantityInputField.text = room.DoubleBeds.ToString();
        roomAdminScreenTitle.text = currentRoom.Name ?? Constants.defaultRoomAdminScreenName;
    }

    private void OnValueChanged(string value)
    {
        roomAdminScreenTitle.text = value;
        currentRoom.Name = string.IsNullOrEmpty(value) ? Constants.defaultRoomAdminScreenName : value;
        currentRoom.SingleBeds = int.Parse(roomSingleBedQuantityInputField.text == "" ? "Cantitate pat single" : roomSingleBedQuantityInputField.text);
        currentRoom.DoubleBeds = int.Parse(roomDoubleBedQuantityInputField.text == "" ? "Cantitate pat dublu" : roomDoubleBedQuantityInputField.text);
    }
}
