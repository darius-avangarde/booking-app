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
    private GameObject floorRoomsObject = null;
    [SerializeField]
    private RectTransform floorRoomsParent = null;
    [SerializeField]
    private RectTransform screenContent = null;
    [SerializeField]
    private InputField multipleFloorsField = null;
    [SerializeField]
    private Dropdown multipleFloorDropdown = null;
    [SerializeField]
    private InputField allMultipleRoomsFields = null;
    [SerializeField]
    private Dropdown allMultipleRoomsDropdown = null;

    private List<FloorRoomsHandler> floorItemList = new List<FloorRoomsHandler>();
    private int currentFloorValue = 0;
    private int currenRoomValue = 0;

    private void Awake()
    {
        for (int i = 0; i <= 5; i++)
        {
            GameObject floorRooms = Instantiate(floorRoomsObject, floorRoomsParent);
            FloorRoomsHandler roomsFloor = floorRooms.GetComponent<FloorRoomsHandler>();
            floorRooms.SetActive(false);
            floorItemList.Add(roomsFloor);
        }
        propertyAdminScreen.SetMultipleRoomsFields += SetFields;
    }

    private void OnDisable()
    {
        for (int i = 0; i < floorItemList.Count; i++)
        {
            floorItemList[i].gameObject.SetActive(false);
        }
    }

    public void UpdateUILayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(floorRoomsParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(screenContent);
    }

    public void StartInput()
    {
        multipleFloorsField.text = string.Empty;
        multipleFloorsField.characterValidation = InputField.CharacterValidation.Integer;
    }

    private void SetFields(int floors, int[] floorRooms)
    {
        multipleRoomsScript.multipleFloorsNumber = floors;
        multipleRoomsScript.multipleRoomsNumber = floorRooms;
        SetFloorInputField(floors.ToString());
    }

    public void SetFloorInputField(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            currentFloorValue = int.Parse(value);
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
        else
        {
            currentFloorValue = 0;
            multipleFloorsField.characterValidation = InputField.CharacterValidation.None;
            multipleFloorsField.text = $"P";
        }
        multipleFloorDropdown.value = currentFloorValue;
        int[] tempArray = multipleRoomsScript.multipleRoomsNumber;
        multipleRoomsScript.multipleRoomsNumber = new int[currentFloorValue + 1];
        if (tempArray.Length < multipleRoomsScript.multipleRoomsNumber.Length)
        {
            for (int i = 0; i < tempArray.Length; i++)
            {
                multipleRoomsScript.multipleRoomsNumber[i] = tempArray[i];
            }
        }
        else
        {
            for (int i = 0; i < multipleRoomsScript.multipleRoomsNumber.Length; i++)
            {
                multipleRoomsScript.multipleRoomsNumber[i] = tempArray[i];
            }
        }
        InitializeFloorRooms(currentFloorValue);
    }

    public void SetFloorDropdownOption()
    {
        currentFloorValue = multipleFloorDropdown.value;
        multipleRoomsScript.multipleFloorsNumber = currentFloorValue;
        multipleFloorsField.characterValidation = InputField.CharacterValidation.None;
        multipleFloorsField.text = multipleFloorDropdown.options[multipleFloorDropdown.value].text;
        int[] tempArray = multipleRoomsScript.multipleRoomsNumber;
        multipleRoomsScript.multipleRoomsNumber = new int[currentFloorValue + 1];
        if (tempArray.Length < multipleRoomsScript.multipleRoomsNumber.Length)
        {
            for (int i = 0; i < tempArray.Length; i++)
            {
                multipleRoomsScript.multipleRoomsNumber[i] = tempArray[i];
            }
        }
        else
        {
            for (int i = 0; i < multipleRoomsScript.multipleRoomsNumber.Length; i++)
            {
                multipleRoomsScript.multipleRoomsNumber[i] = tempArray[i];
            }
        }
        InitializeFloorRooms(currentFloorValue);
    }

    public void SetRoomInputField(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            currenRoomValue = int.Parse(value);
            for (int i = 0; i <= currentFloorValue; i++)
            {
                floorItemList[i].SetRoomInputField(currenRoomValue.ToString());
            }
            allMultipleRoomsFields.text = currenRoomValue.ToString();
        }
    }

    public void SetRoomDropdownOption()
    {
        currenRoomValue = allMultipleRoomsDropdown.value + 1;
        for (int i = 0; i <= currentFloorValue; i++)
        {
            floorItemList[i].SetRoomInputField(currenRoomValue.ToString());
        }
        allMultipleRoomsFields.text = currenRoomValue.ToString();
    }

    private void InitializeFloorRooms(int floors)
    {
        if (floorItemList.Count != floors)
        {
            //Create New Objects as needed
            for (int i = floorItemList.Count - 1; i <= floors; i++)
            {
                InstantiateFloors();
            }

            //Disable unused objects
            for (int i = floorItemList.Count - 1; i >= floors; i--)
            {
                floorItemList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i <= floors; i++)
        {
            floorItemList[i].gameObject.SetActive(true);
            floorItemList[i].SetPropertyFloor(i);
            if (multipleRoomsScript.multipleRoomsNumber[i] != 0)
            {
                floorItemList[i].SetRoomInputField(multipleRoomsScript.multipleRoomsNumber[i].ToString());
            }
            else
            {
                floorItemList[i].SetRoomInputField(currenRoomValue.ToString());
            }
            UpdateUILayout();
        }
    }

    private void InstantiateFloors()
    {
        GameObject floorRooms = Instantiate(floorRoomsObject, floorRoomsParent);
        FloorRoomsHandler roomsFloor = floorRooms.GetComponent<FloorRoomsHandler>();
        floorItemList.Add(roomsFloor);
    }
}
