using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RoomFieldsHandler : MonoBehaviour
{
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

    private void Awake()
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

    public void SetFloorInputField()
    {
        currentFloorValue = int.Parse(multipleFloorsField.text);
        multipleRoomsScript.multipleFloorsNumber = currentFloorValue;
        multipleFloorsField.characterValidation = InputField.CharacterValidation.None;
        if (currentFloorValue > 0)
        {
            multipleFloorsField.text = $"P+{currentFloorValue}";
        }
        else
        {
            multipleFloorsField.text = $"P";
        }
    }

    public void SetRoomInputField()
    {
        currenRoomValue = int.Parse(multipleRoomsField.text);
        multipleRoomsScript.multipleRoomsNumber = currenRoomValue;
    }

    public void SetFloorDropdownOption()
    {
        currentFloorValue = multipleFloorDropdown.value;
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
