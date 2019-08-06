using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorRoomsHandler : MonoBehaviour
{
    [SerializeField]
    private CreateMultipleRooms multipleRoomsScript = null;
    [SerializeField]
    private ThemeManager themeManager = null;
    [SerializeField]
    private Text floorRoomsFieldText = null;
    [SerializeField]
    private InputField floorRoomsField = null;
    [SerializeField]
    private Dropdown floorRoomsDropdown = null;

    [Header("Theme objects"), SerializeField]
    private Graphic floorText = null;
    [SerializeField]
    private Graphic roomsFloorText = null;
    [SerializeField]
    private Graphic dropdownText = null;
    [SerializeField]
    private Graphic dropdownPlaceholder = null;
    [SerializeField]
    private Graphic dropdownArrow = null;
    [SerializeField]
    private Graphic dropdownUnderline = null;
    [SerializeField]
    private Graphic dropdowntemplateOutline = null;
    [SerializeField]
    private Graphic dropdowntemplateBackground = null;
    [SerializeField]
    private Graphic dropdowntemplateItemBackground = null;
    [SerializeField]
    private Graphic dropdowntemplateItemLabel = null;
    [SerializeField]
    private Graphic dropdowntemplateItemUnderline = null;

    private int currenRoomValue = 0;
    private int currentFloor = 0;

    private void Start()
    {
        themeManager.AddItems(floorText, roomsFloorText, dropdownText, dropdownPlaceholder, dropdownArrow, dropdownUnderline, dropdowntemplateOutline, dropdowntemplateBackground, dropdowntemplateItemBackground, dropdowntemplateItemLabel, dropdowntemplateItemUnderline);
    }

    public void SetPropertyFloor(int floor)
    {
        if (floor == 0)
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
            if (currenRoomValue < 1)
            {
                currenRoomValue = 1;
            }
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
