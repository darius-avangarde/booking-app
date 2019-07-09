using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private PropertyRoomScreen propertyRoomScreen = null;
    [SerializeField]
    private Text propertyRoomTitle = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private GameObject roomsCreationText = null;
    [SerializeField]
    private GameObject roomInfoInputPanel = null;
    [SerializeField]
    private InputField roomPriceInputField = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;
    [SerializeField]
    private Button deleteButton = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button calcelButton = null;
    [SerializeField]
    private Text errorMessage = null;

    private ConfirmationDialogOptions modalDialogOptions = new ConfirmationDialogOptions();
    private IProperty currentProperty;
    private IRoom currentRoom;
    private int SingleBedsNr;
    private int DoubleBedsNr;
    private bool canSave = true;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
        deleteButton.onClick.AddListener(() => DeleteRoom());

        modalDialogOptions.Message = Constants.DELETE_ROOM;
        modalDialogOptions.ConfirmText = Constants.DELETE_CONFIRM;
        modalDialogOptions.CancelText = Constants.DELETE_CANCEL;
        modalDialogOptions.ConfirmCallback = () =>
        {
            currentProperty.DeleteRoom(currentRoom.ID);
            ReservationDataManager.DeleteReservationsForRoom(currentRoom.ID);
            navigator.GoBack();
            navigator.GoBack();
        };
        modalDialogOptions.CancelCallback = null;
    }

    /// <summary>
    /// set the current property and room
    /// </summary>
    /// <param name="room">selected room</param>
    public void SetCurrentPropertyRoom(IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        Initialize();
    }

    /// <summary>
    /// initialize current property photo
    /// initialize input fields
    /// initialize beds information
    /// </summary>
    public void Initialize()
    {
        if (currentRoom != null)
        {
            propertyRoomTitle.text = currentProperty.Name;
            if (string.IsNullOrEmpty(currentRoom.Name))
            {
                roomNameInputField.gameObject.SetActive(false);
                roomsCreationText.SetActive(true);
                roomInfoInputPanel.SetActive(true);
                propertyRoomTitle.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(false);
            }
            else
            {
                roomNameInputField.gameObject.SetActive(true);
                roomsCreationText.SetActive(false);
                roomNameInputField.text = currentRoom.Name;
                roomInfoInputPanel.SetActive(false);
                propertyRoomTitle.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(true);
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
        
    }

    public void DeleteRoom()
    {
        modalDialogOptions.Message = ReservationDataManager.GetActiveRoomReservations(currentRoom.ID).Count() > 0 ? Constants.DELETE_ROOM_RESERVATIONS : Constants.DELETE_ROOM;
        modalDialogOptions.ConfirmText = Constants.DELETE_CONFIRM;
        modalDialogOptions.CancelText = Constants.DELETE_CANCEL;
        modalDialogOptions.ConfirmCallback = () =>
        {
            currentProperty.DeleteRoom(currentRoom.ID);
            ReservationDataManager.DeleteReservationsForRoom(currentRoom.ID);
            navigator.GoBack();
            navigator.GoBack();
        };
        modalDialogOptions.CancelCallback = null;
        confirmationDialog.Show(modalDialogOptions);
    }

    public void DefaultValues()
    {
        roomNameInputField.text = string.Empty;
        roomPriceInputField.text = string.Empty;
        roomSingleBedQuantityInputField.text = string.Empty;
        roomDoubleBedQuantityInputField.text = string.Empty;
        errorMessage.text = string.Empty;
        currentProperty = null;
        currentRoom = null;
        canSave = true;
    }

    public void ResetError()
    {
        errorMessage.text = string.Empty;
        canSave = true;
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
}
