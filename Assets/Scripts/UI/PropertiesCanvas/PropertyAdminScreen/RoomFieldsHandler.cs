using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RoomFieldsHandler : MonoBehaviour
{
    [SerializeField]
    private PropertyAdminScreen propertyAdminScreen = null;
    [SerializeField]
    private CreateMultipleRooms multipleRoomsScript = null;
    [SerializeField]
    private InputField multipleFloorsField = null;
    [SerializeField]
    private InputField multipleRoomsField = null;
    [SerializeField]
    private Dropdown multipleFloorDropdown = null;
    [SerializeField]
    private Dropdown multipleRoomsDropdown = null;

    private int currentFloorValue = 0;
    private int currenRoomValue = 0;

    private void Start()
    {
        propertyAdminScreen.SetMultipleRoomsFields += SetFields;
    }

    private void OnEnable()
    {
        multipleFloorDropdown.value = 1;
        multipleRoomsDropdown.value = 1;
        multipleFloorDropdown.value = 0;
        multipleRoomsDropdown.value = 0;
    }

    public void StartInput()
    {
        multipleFloorsField.characterValidation = InputField.CharacterValidation.Integer;
    }

    private void SetFields(int floors, int rooms)
    {
        SetFloorInputField((floors - 1).ToString());
        SetRoomInputField(rooms.ToString());
    }

    public void SetFloorInputField(string value)
    {
        currentFloorValue = int.Parse(value) + 1;
        multipleRoomsScript.multipleFloorsNumber = currentFloorValue;
        multipleFloorsField.characterValidation = InputField.CharacterValidation.None;
        if (currentFloorValue > 0)
        {
            multipleFloorsField.text = $"P+{currentFloorValue - 1}";
        }
        else
        {
            multipleFloorsField.text = $"P";
        }
    }

    public void SetRoomInputField(string value)
    {
        currenRoomValue = int.Parse(value);
        multipleRoomsScript.multipleRoomsNumber = currenRoomValue;
        multipleRoomsField.text = value;
    }

    public void SetFloorDropdownOption()
    {
        currentFloorValue = multipleFloorDropdown.value + 1;
        multipleRoomsScript.multipleFloorsNumber = currentFloorValue;
        multipleFloorsField.characterValidation = InputField.CharacterValidation.None;
        multipleFloorsField.text = multipleFloorDropdown.options[multipleFloorDropdown.value].text;
    }

    public void SetRoomDropdownOption()
    {
        currenRoomValue = multipleRoomsDropdown.value;
        multipleRoomsScript.multipleRoomsNumber = currenRoomValue;
        multipleRoomsField.text = multipleRoomsDropdown.options[multipleRoomsDropdown.value].text;
    }
}
