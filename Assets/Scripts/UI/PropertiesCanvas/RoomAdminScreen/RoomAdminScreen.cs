using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    public Action GetRoomName = delegate { };
    public Action GetRoomType = delegate { };
    public Action GetBedsNumber = delegate { };
    public Action<string> SetRoomName = delegate { };
    public Action<string> SetRoomType = delegate { };
    public Action<int, int> SetBedsNumber = delegate { };
    public IRoom CurrentRoom { get; set; }

    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Text propertyRoomTitle = null;
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
            currentProperty.DeleteRoom(CurrentRoom.ID);
            ReservationDataManager.DeleteReservationsForRoom(CurrentRoom.ID);
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
        CurrentRoom = room;
        Initialize();
    }

    /// <summary>
    /// initialize input fields
    /// initialize room type dropdown
    /// initialize bed information
    /// </summary>
    public void Initialize()
    {
        if (CurrentRoom != null)
        {
            propertyRoomTitle.text = Constants.EDIT_ROOM;
            SetRoomName(CurrentRoom.Name);
            SetRoomType(CurrentRoom.Type);
            SetBedsNumber(CurrentRoom.SingleBeds, CurrentRoom.DoubleBeds);
        }
    }

    public void SaveChanges()
    {
        GetRoomName();
        GetRoomType();
        GetBedsNumber();
        navigator.GoBack();
    }

    public void DeleteRoom()
    {
        modalDialogOptions.Message = ReservationDataManager.GetActiveRoomReservations(CurrentRoom.ID).Count() > 0 ? Constants.DELETE_ROOM_RESERVATIONS : Constants.DELETE_ROOM;
        modalDialogOptions.ConfirmText = Constants.DELETE_CONFIRM;
        modalDialogOptions.CancelText = Constants.DELETE_CANCEL;
        modalDialogOptions.ConfirmCallback = () =>
        {
            currentProperty.DeleteRoom(CurrentRoom.ID);
            ReservationDataManager.DeleteReservationsForRoom(CurrentRoom.ID);
            navigator.GoBack();
            navigator.GoBack();
        };
        modalDialogOptions.CancelCallback = null;
        confirmationDialog.Show(modalDialogOptions);
    }

    public void DefaultValues()
    {
        errorMessage.text = string.Empty;
        currentProperty = null;
        CurrentRoom = null;
        canSave = true;
    }

    public void ResetError()
    {
        errorMessage.text = string.Empty;
        canSave = true;
    }
}
