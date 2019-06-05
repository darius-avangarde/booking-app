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
    private Text propertyRoomTitle = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private RectTransform roomNameInputFieldTransform = null;
    [SerializeField]
    private InputField multiplePrefixField = null;
    [SerializeField]
    private InputField multipleFloorsField = null;
    [SerializeField]
    private InputField multipleNrRoomsField = null;
    [SerializeField]
    private GameObject roomInfoInputPanel = null;

    [SerializeField]
    private InputField roomPriceInputField = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;
    [SerializeField]
    private Toggle singleRoomToggle = null;
    [SerializeField]
    private Toggle multipleRoomsToggle = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button calcelButton = null;

    private IProperty currentProperty;
    private IRoom currentRoom;
    private List<IRoom> MultipleRooms = new List<IRoom>();
    private int SingleBedsNr;
    private int DoubleBedsNr;

    private void Awake()
    {
        backButton.onClick.AddListener(() => GoBack());
        calcelButton.onClick.AddListener(() => GoBack());
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
            propertyRoomTitle.text = currentProperty.Name;
            roomNameInputField.text = currentRoom.Name ?? Constants.NEW_ROOM;
            singleRoomToggle.isOn = true;
            if (string.IsNullOrEmpty(currentRoom.Name))
            {
                roomInfoInputPanel.SetActive(true);
            }
            else
            {
                roomInfoInputPanel.SetActive(false);
            }
            roomPriceInputField.text = currentRoom.Price ?? Constants.PRICE;
            roomSingleBedQuantityInputField.text = currentRoom.SingleBeds.ToString();
            roomDoubleBedQuantityInputField.text = currentRoom.DoubleBeds.ToString();
            SingleBedsNr = currentRoom.SingleBeds;
            DoubleBedsNr = currentRoom.DoubleBeds;
        }
    }

    public void SaveChanges()
    {
        if (singleRoomToggle.isOn)
        {
            SaveRoom();
        }
        else if (multipleRoomsToggle.isOn)
        {
            SaveMultipleRooms();
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

    public void SaveMultipleRooms()
    {

    }

    public void SelectSingleRoom()
    {
        if (singleRoomToggle.isOn)
        {
            roomNameInputFieldTransform.sizeDelta = new Vector2(0, 110);
        }
    }

    public void SelectMultipleRooms()
    {
        if (multipleRoomsToggle.isOn)
        {
            //roomNameInputFieldTransform.rect.
        }
    }

    public void DeleteRoom(IRoom selectedRoom)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_ROOM,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                IProperty selectedProperty = PropertyDataManager.GetProperty(selectedRoom.PropertyID);
                selectedProperty.DeleteRoom(selectedRoom.ID);
                ReservationDataManager.DeleteReservationsForRoom(selectedRoom.ID);
                navigator.GoBack();
            },
            CancelCallback = null
        });
    }

    public void OnRoomNameValueChanged(string value)
    {
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
            //propertiesScreen.OpenRoomDropdown = currentProperty.ID;
            propertiesScreen.Initialize();
            propertiesScreen = null;
            navigator.GoBack();
        }
        if (disponibilityScreen != null)
        {
            //disponibilityScreen.OpenRoomDropdown = currentProperty.ID;
            //disponibilityScreen.Initialize();
            //disponibilityScreen = null;
            navigator.GoBack();
        }
    }

    private void GoBack()
    {
        disponibilityScreen = null;
        propertiesScreen = null;
        navigator.GoBack();
    }
}
