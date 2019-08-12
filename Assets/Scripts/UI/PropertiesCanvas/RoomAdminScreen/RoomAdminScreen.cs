using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    public bool CanSave { get; set; } = true;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private PropertyDropdownHandler propertyDropdownHandler = null;
    [SerializeField]
    private NavScreen roomAdminScreen = null;
    [SerializeField]
    private SetRoomName setRoomName = null;
    [SerializeField]
    private SetRoomTypeDropdown setRoomTypeDropdown = null;
    [SerializeField]
    private SetBedsNumber setBedsNumber = null;
    [SerializeField]
    private Text propertyRoomTitle = null;
    [SerializeField]
    private GameObject roomTypeContainer = null;
    [SerializeField]
    private Button deleteButton = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button calcelButton = null;
    [SerializeField]
    private Text errorMessage = null;

    private ConfirmationDialogOptions modalDialogOptions = new ConfirmationDialogOptions();
    private Action returnCallback = null;
    private IProperty currentProperty;
    private IRoom currentRoom;
    private bool canSave = true;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        calcelButton.onClick.AddListener(() => navigator.GoBack());
        deleteButton.onClick.AddListener(() => DeleteRoom());
    }

    private void OnDisable()
    {
        DefaultValues();
    }

    /// <summary>
    /// set the current property and room
    /// </summary>
    /// <param name="room">selected room</param>
    public void OpenRoomAdminScreen(IRoom room, Action callback = null)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        returnCallback = callback;
        navigator.GoTo(roomAdminScreen);
        Initialize();
    }

    /// <summary>
    /// initialize input fields
    /// initialize room type dropdown
    /// initialize bed information
    /// </summary>
    public void Initialize()
    {
        if (currentRoom != null)
        {
            if (currentProperty.HasRooms)
            {
                propertyRoomTitle.text = LocalizedText.Instance.PropertyRoomsTitle[1];
                roomTypeContainer.SetActive(true);
            }
            else
            {
                propertyRoomTitle.text = LocalizedText.Instance.PropertyRoomsTitle[0];
                roomTypeContainer.SetActive(false);
            }
            setRoomName.SetCurrentName(currentRoom.Name);
            setRoomTypeDropdown.CurrentRoomType = currentRoom.RoomType;
            Vector2Int bedInfo = new Vector2Int(currentRoom.SingleBeds, currentRoom.DoubleBeds);
            setBedsNumber.SetCurrentBeds(bedInfo);
        }
    }

    public void SaveChanges()
    {
        if (CanSave)
        {
            currentRoom.Name = setRoomName.GetCurrentName();
            if (!currentProperty.HasRooms)
            {
                currentProperty.Name = setRoomName.GetCurrentName();
            }
            currentRoom.RoomType = setRoomTypeDropdown.CurrentRoomType;
            Vector2Int bedInfo = setBedsNumber.GetCurrentBeds();
            currentRoom.SingleBeds = bedInfo.x;
            currentRoom.DoubleBeds = bedInfo.y;
            PropertyDataManager.SavePropertyData();
            navigator.GoBack();
            returnCallback?.Invoke();
        }
        else
        {
            errorMessage.text = LocalizedText.Instance.PropertyErrorRoom;
        }
    }

    public void DeleteRoom()
    {
        if (currentProperty.HasRooms)
        {
            modalDialogOptions.Message = ReservationDataManager.GetActiveRoomReservations(currentRoom.ID).Count() > 0 ? Constants.DELETE_ROOM_RESERVATIONS : Constants.DELETE_ROOM;
        }
        else
        {
            modalDialogOptions.Message = LocalizedText.Instance.ConfirmDelete[2];
        }
        modalDialogOptions.ConfirmText = LocalizedText.Instance.ConfirmAction[0];
        modalDialogOptions.CancelText = LocalizedText.Instance.ConfirmAction[1];
        modalDialogOptions.ConfirmCallback = () =>
        {
            if (currentProperty.HasRooms)
            {
                currentProperty.DeleteRoom(currentRoom.ID);
                ReservationDataManager.DeleteReservationsForRoom(currentRoom.ID);
            }
            else
            {
                PropertyDataManager.DeleteProperty(currentProperty.ID);
                ReservationDataManager.DeleteReservationsForProperty(currentProperty.ID);
                propertyDropdownHandler.UpdateDropdown();
            }
            navigator.GoBack();
            returnCallback?.Invoke();
        };
        modalDialogOptions.CancelCallback = null;
        confirmationDialog.Show(modalDialogOptions);
    }

    public void DefaultValues()
    {
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
}
