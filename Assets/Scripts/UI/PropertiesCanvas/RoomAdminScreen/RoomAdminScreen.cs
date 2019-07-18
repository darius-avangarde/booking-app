﻿using System;
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
    private SetRoomName setRoomName = null;
    [SerializeField]
    private SetRoomTypeDropdown setRoomTypeDropdown = null;
    [SerializeField]
    private SetBedsNumber setBedsNumber = null;
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
    private IRoom currentRoom;
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

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        DefaultValues();
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
    /// initialize input fields
    /// initialize room type dropdown
    /// initialize bed information
    /// </summary>
    public void Initialize()
    {
        if (currentRoom != null)
        {
            propertyRoomTitle.text = Constants.EDIT_ROOM;
            setRoomName.SetCurrentName(currentRoom.Name);
            setRoomTypeDropdown.CurrentRoomType = currentRoom.RoomType;
            //setRoomTypeDropdown.SetRoomType(currentRoom.RoomType);
            setBedsNumber.SetCurrentBeds(currentRoom.SingleBeds, currentRoom.DoubleBeds);
        }
    }

    public void SaveChanges()
    {
        currentRoom.Name = setRoomName.GetCurrentName();
        currentRoom.RoomType = setRoomTypeDropdown.CurrentRoomType;
        Vector2 bedsInfo = setBedsNumber.GetCurrentBeds();
        currentRoom.SingleBeds = (int)bedsInfo.x;
        currentRoom.DoubleBeds = (int)bedsInfo.y;
        navigator.GoBack();
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
