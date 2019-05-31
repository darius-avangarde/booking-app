using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    public PropertiesScreen propertiesScreen { get; set; }
    public DisponibilityScreen disponibilityScreen { get; set; }

    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Text roomAdminScreenTitle = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private InputField roomPriceInputField = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private Button calcelButton;

    private IProperty currentProperty;
    private IRoom currentRoom;
    private int SingleBedsNr;
    private int DoubleBedsNr;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
    }

    public void SetCurrentPropertyRoom(IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        Initialize();
    }

    public void Initialize()
    {
        if (currentRoom != null)
        {
            roomNameInputField.text = currentRoom.Name ?? "";
            roomPriceInputField.text = currentRoom.Price ?? "";
            roomSingleBedQuantityInputField.text = currentRoom.SingleBeds.ToString();
            roomDoubleBedQuantityInputField.text = currentRoom.DoubleBeds.ToString();
            SingleBedsNr = currentRoom.SingleBeds;
            DoubleBedsNr = currentRoom.DoubleBeds;
            roomAdminScreenTitle.text = currentRoom.Name ?? Constants.NEW_ROOM;
        }
    }

    public void SaveRoom()
    {
        OnRoomNameValueChanged(roomNameInputField.text);
        OnRoomPriceValueChanged(roomPriceInputField.text);
        OnSingleBedsChanged(roomSingleBedQuantityInputField.text);
        OnDoubleBedsChanged(roomDoubleBedQuantityInputField.text);
        if (currentProperty.GetRoom(currentRoom.ID) == null)
        {
            currentProperty.SaveRoom(currentRoom);
        }
        OpenPropertiesScreen();
    }

    public void OnRoomNameValueChanged(string value)
    {
        roomAdminScreenTitle.text = value;
        currentRoom.Name = string.IsNullOrEmpty(value) ? Constants.NEW_ROOM : value;
    }

    public void OnRoomPriceValueChanged(string value)
    {
        currentRoom.Price = string.IsNullOrEmpty(value) ? "" : value;
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
        roomSingleBedQuantityInputField.text = (++SingleBedsNr).ToString();
    }

    public void DecrementSingleBedQuantity()
    {
        if (roomSingleBedQuantityInputField.text != "0")
        {
            roomSingleBedQuantityInputField.text = (--SingleBedsNr).ToString();
        }
    }

    public void IncrementDoubleBedQuantity()
    {
        roomDoubleBedQuantityInputField.text = (++DoubleBedsNr).ToString();
    }

    public void DecrementDoubleBedQuantity()
    {
        if (roomDoubleBedQuantityInputField.text != "0")
        {
            roomDoubleBedQuantityInputField.text = (--DoubleBedsNr).ToString();
        }
    }

    private void OpenPropertiesScreen()
    {
        if (propertiesScreen != null)
        {
            propertiesScreen.OpenRoomDropdown = currentProperty.ID;
            propertiesScreen.Initialize();
            navigator.GoBack();
        }
        if(disponibilityScreen != null)
        {
            disponibilityScreen.OpenRoomDropdown = currentProperty.ID;
            disponibilityScreen.Initialize();
            navigator.GoBack();
        }
    }
}
