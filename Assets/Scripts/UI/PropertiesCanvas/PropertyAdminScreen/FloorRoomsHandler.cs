using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorRoomsHandler : MonoBehaviour
{
    [SerializeField]
    private CreateMultipleRooms multipleRoomsScript = null;
    [SerializeField]
    private Text floorRoomsFieldText = null;
    [SerializeField]
    private InputField floorRoomsField = null;
    [SerializeField]
    private Dropdown floorRoomsDropdown = null;

    private int currenRoomValue = 0;
    private int currentFloor = 0;

    public void SetPropertyFloor(int floor)
    {
        if(floor == 0)
        {
            floorRoomsFieldText.text = $"Etaj P:";
        }
        else
        {
            floorRoomsFieldText.text = $"Etaj P+{floor}:";
        }
        currentFloor = floor;
        SetRoomInputField(multipleRoomsScript.multipleRoomsNumber[currentFloor].ToString());
    }

    public void SetRoomInputField(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            currenRoomValue = int.Parse(value);
        }
        else
        {
            currenRoomValue = 1;
        }
        multipleRoomsScript.multipleRoomsNumber[currentFloor] = currenRoomValue;
        floorRoomsField.text = currenRoomValue.ToString();
    }

    public void SetRoomDropdownOption()
    {
        currenRoomValue = floorRoomsDropdown.value + 1;
        multipleRoomsScript.multipleRoomsNumber[currentFloor] = currenRoomValue;
        floorRoomsField.text = currenRoomValue.ToString();
    }
}
