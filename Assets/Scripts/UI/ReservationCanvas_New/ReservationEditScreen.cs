﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UINavigation;

public class ReservationEditScreen : MonoBehaviour
{
    #region Inspector references

        [Header("Navigation")]
        [SerializeField]
        private Navigator navigator;
        [SerializeField]
        private NavScreen navScreen;

        [Header("Modal dialogues")]
        [SerializeField]
        private ModalCalendar modalCalendarDialog;
        [SerializeField]
        private ConfirmationDialog confirmationDialog;

        [Space]
        [SerializeField]
        private ReservationsViewScreen reservationsScreen;
        [SerializeField]
        private Text titleText;
        [SerializeField]
        private Dropdown propertyDropdown;
        [SerializeField]
        private Dropdown roomDropdown;
        [SerializeField]
        private InputField clientInputField;
        [SerializeField]
        private Text reservationPeriodText;
        [SerializeField]
        private Button confirmButton;

    #endregion

    #region Private variables
        private Dictionary<string,Dropdown.OptionData> propertyOptions;
        private Dictionary<string,Dropdown.OptionData> roomOptions;

        private IReservation currentReservation;
        private IRoom currentRoom;
        private IProperty currentProperty;
        private IClient currentClient;

        private List<IReservation> roomReservationList;
        //set these on callback from calendar overlay
        private DateTime start;
        private DateTime end;

        private ConfirmationDialogOptions editConfirmation;
    #endregion

    private void Start()
    {
        editConfirmation = new ConfirmationDialogOptions();
        editConfirmation.Message = ReservationConstants.EDIT_DIALOG;
        propertyDropdown.onValueChanged.AddListener(SelectProperty);
        roomDropdown.onValueChanged.AddListener(SelectRoom);
    }

    private void OnDestroy()
    {
        propertyDropdown.onValueChanged.RemoveAllListeners();
        roomDropdown.onValueChanged.RemoveAllListeners();
    }

    #region Open panel functions
        ///<summary>
        /// Fills the available fields and opens the reservation edit pannel withe the provided reservation object
        /// Use to EDIT a reservation from either the client screen or the room screen when tapping on an existing reservation.
        ///</summary>
        public void OpenEditReservation(IReservation reservation)
        {
            UpdateEditableOptions(
                reservation,
                ClientDataManager.GetClient(reservation.CustomerID),
                PropertyDataManager.GetProperty(reservation.PropertyID).GetRoom(reservation.RoomID)
                );
            titleText.text = ReservationConstants.EDIT_TITLE;
            ToggleConfirmButton();
            navigator.GoTo(navScreen);
        }

        ///<summary>
        /// Fills the available fields and opens the reservation edit pannel withe the provided client object
        /// Use when adding a NEW reservation for a specific client from the client screen.
        ///</summary>
        public void OpenEditReservation(IClient client)
        {
            UpdateEditableOptions(null, client, null);
            titleText.text = ReservationConstants.NEW_TITLE;
            ToggleConfirmButton();
            navigator.GoTo(navScreen);
        }

        ///<summary>
        /// Fills the available fields and opens the reservation edit pannel withe the provided room object.
        /// Use when adding a NEW reservation for a specific room from the client screen.
        ///</summary>
        public void OpenEditReservation(IRoom room)
        {
            UpdateEditableOptions(null, null, room);
            titleText.text = ReservationConstants.NEW_TITLE;
            ToggleConfirmButton();
            navigator.GoTo(navScreen);
        }
    #endregion

    #region Public functions
        ///<summary>
        /// Opens the modal calendar overlay if a property is selected in order to change or set the reservation period
        ///</summary>
        public void ChangePeriod()
        {
            string reservationID = (currentReservation != null) ? currentReservation.ID : string.Empty;

            if(currentRoom != null)
            {
                modalCalendarDialog.Show(
                    DateTime.Today,
                    currentReservation,
                    ReservationDataManager.GetReservations()
                        .Where(r => r.RoomID == currentRoom.ID && r.ID != reservationID)
                        .ToList(),
                        UpdateReservationPeriod
                    );
            }
        }

        ///<summary>
        /// Propmpts a confirmation screen if a reservetion is being edited. If the reservation is new the confirmation dialog is skipped
        ///</summary>
        public void CommitChanges()
        {
            if (currentReservation != null)
            {
                editConfirmation.ConfirmCallback = () =>
                {
                    currentReservation.EditReservation(
                        currentRoom,
                        currentClient,
                        start,
                        end
                    );
                    navigator.GoBack();
                    reservationsScreen.ReloadReservations();
                };
                confirmationDialog.Show(editConfirmation);
            }
            else
            {
                ReservationDataManager.AddReservation(
                    currentRoom,
                    currentClient,
                    start,
                    end
                );
                navigator.GoBack();
                reservationsScreen.ReloadReservations();
            }
        }

        ///<summary>
        /// Sets the curent client and updates the client input field text to the client name
        ///</summary>
        public void SetClient(IClient client)
        {
            currentClient = client;
            clientInputField.text = client.Name;
        }
    #endregion

    //Toggles the save/confirm reservation edit button off if there are any necessary fields unfilled
    private void ToggleConfirmButton()
    {
        if(currentProperty == null)
        {
            confirmButton.interactable = false;
            return;
        }

        if(currentRoom == null)
        {
            confirmButton.interactable = false;
            return;
        }

        if(currentClient == null)
        {
            confirmButton.interactable = false;
            return;
        }

        if(start == end)
        {
            confirmButton.interactable = false;
            return;
        }

        confirmButton.interactable = true;
    }

    //TODO: Integrate has room property from new property data
    //Sets the selected property object as selected from the dropdown options. This also sets the room in the case of the property not having rooms
    private void SelectProperty(int optionIndex)
    {
        if(optionIndex > 0)
        {
            currentProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(optionIndex).Key);
        }
        else
        {
            currentProperty = null; //if prop has 1 room also set room
            currentRoom = null;
        }

        UpdateRoomDropdown(currentProperty);
        ToggleConfirmButton();
    }

    //Sets the selected room object as selected from the dropdown options
    private void SelectRoom(int optionIndex)
    {
        if(optionIndex > 0)
        {
            currentRoom = currentProperty.GetRoom(roomOptions.ElementAt(optionIndex).Key);
        }
        else
        {
            currentRoom = null;
        }
        ToggleConfirmButton();
    }

    //Updates all editable fields in the edit reservation screen
    private void UpdateEditableOptions(IReservation reservation, IClient client, IRoom room)
    {
        currentReservation = reservation;
        currentRoom = room;
        currentClient = client;

        UpdatePropertyDropdown();

        clientInputField.text = (client != null) ? client.Name : Constants.defaultCustomerName;

        if(reservation != null)
        {
            start = reservation.Period.Start;
            end = reservation.Period.End;


            reservationPeriodText.text =
                start.ToString(Constants.DateTimePrintFormat)
                + Constants.AndDelimiter
                + end.ToString(Constants.DateTimePrintFormat);
        }
        else
        {
            start = DateTime.Today;
            end = DateTime.Today;
            reservationPeriodText.text = start.ToString(Constants.DateTimePrintFormat);
        }

    }

    private void UpdatePropertyDropdown()
    {
        int selected = 0;
        bool searching = true;
        currentProperty = null;

        propertyOptions = new Dictionary<string, Dropdown.OptionData>();
        propertyOptions.Add(String.Empty, new Dropdown.OptionData("Alege"));
        foreach(IProperty p in PropertyDataManager.GetProperties())
        {
            propertyOptions.Add(p.ID, new Dropdown.OptionData(p.Name));


            if (currentRoom != null)
            {
                if(searching)
                {
                    selected++;
                }

                if(currentRoom.PropertyID == p.ID)
                {
                    currentProperty = p;
                    searching = false;
                }
            }
        }

        propertyDropdown.options = propertyOptions.Values.ToList();

        propertyDropdown.value = (searching) ? 0 : selected;
        propertyDropdown.RefreshShownValue();

        UpdateRoomDropdown(currentProperty);
    }

    private void UpdateRoomDropdown(IProperty property)
    {
        if(property != null)
        {
            if(property.Rooms.Count() > 1)
            {
                roomDropdown.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                roomDropdown.transform.parent.gameObject.SetActive(false);
                currentRoom = property.Rooms.ToList()[0];
                return;
            }
        }
        else
        {
            roomDropdown.transform.parent.gameObject.SetActive(false);
            return;
        }

        int selected = 0;
        bool searching = true;


        roomOptions = new Dictionary<string, Dropdown.OptionData>();
        roomOptions.Add(String.Empty, new Dropdown.OptionData("Alege"));
        foreach(IRoom r in property.Rooms)
        {
            roomOptions.Add(r.ID, new Dropdown.OptionData(r.Name));

            if (currentRoom != null)
            {
                if(searching)
                {
                    selected++;
                }

                if(currentRoom.ID == r.ID)
                {
                    searching = false;
                }
            }
        }

        roomDropdown.options = roomOptions.Values.ToList();

        roomDropdown.value = (searching) ? 0 : selected;
        roomDropdown.RefreshShownValue();
    }

    //Callback function for the modal calendar overlay, sets selected start and end times
    private void UpdateReservationPeriod(DateTime _start, DateTime _end)
    {
        start = _start;
        end = _end;

        reservationPeriodText.text =
            start.ToString(Constants.DateTimePrintFormat)
            + Constants.AndDelimiter
            + end.ToString(Constants.DateTimePrintFormat);
        ToggleConfirmButton();
    }
}
