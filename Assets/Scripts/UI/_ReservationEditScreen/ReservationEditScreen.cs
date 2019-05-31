using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UINavigation;
using UnityEngine.Events;

public class ReservationEditScreen : MonoBehaviour
{
    #region Inspector references
        [Header("Navigation")]
        [SerializeField]
        private Navigator navigator = null;
        [SerializeField]
        private NavScreen navScreen = null;

        [Header("Modal dialogues")]
        [SerializeField]
        private ModalCalendarNew modalCalendarDialog = null;
        [SerializeField]
        private ConfirmationDialog confirmationDialog = null;

        [Space]
        [SerializeField]
        private Text titleText = null;
        [SerializeField]
        private Dropdown propertyDropdown = null;
        [SerializeField]
        private Dropdown roomDropdown = null;
        [SerializeField]
        private InputField clientInputField = null;
        [SerializeField]
        private Text reservationPeriodText = null;
        [SerializeField]
        private Button confirmButton = null;
        [SerializeField]
        private Button setPeriodButton = null;
        [SerializeField]
        private Text errorText = null;
    #endregion
    #region Private variables
    internal bool allowEdit { get; set; } = false;

    private Dictionary<string,Dropdown.OptionData> propertyOptions;
        private Dictionary<string,Dropdown.OptionData> roomOptions;

        private IReservation currentReservation;
        private IRoom currentRoom;
        private IProperty currentProperty;
        private IClient currentClient;

        //set these on callback from calendar overlay
        private IDateTimePeriod period;
        private ConfirmationDialogOptions editConfirmation;

        private UnityAction<IReservation> confirmationCallback;
    #endregion

    private void Start()
    {
        allowEdit = false;
        errorText.enabled = false;
        editConfirmation = new ConfirmationDialogOptions();
        editConfirmation.Message = Constants.EDIT_DIALOG;
        period = ReservationDataManager.DefaultPeriod();
    }

    private void OnDestroy()
    {
        propertyDropdown.onValueChanged.RemoveAllListeners();
        roomDropdown.onValueChanged.RemoveAllListeners();
        clientInputField.onEndEdit.RemoveAllListeners();
    }

    #region Public and internal functions
        ///<summary>
        /// Opens the modal calendar overlay if a property is selected in order to change or set the reservation period
        ///</summary>
        public void ChangePeriod()
        {
            string reservationID = (currentReservation != null) ? currentReservation.ID : string.Empty;

            if(currentRoom != null)
            {
                modalCalendarDialog.OpenCallendar(
                    currentReservation,
                    ReservationDataManager.GetActiveRoomReservations(currentRoom.ID)
                        .Where(r => r.ID != reservationID)
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
                        period.Start,
                        period.End
                    );
                    if(confirmationCallback != null)
                        confirmationCallback.Invoke(currentReservation);
                    navigator.GoBack();
                    CancelChanges();
                };
                confirmationDialog.Show(editConfirmation);
            }
            else
            {
                IReservation newReservation = ReservationDataManager.AddReservation(
                    currentRoom,
                    currentClient,
                    period.Start,
                    period.End
                );
                if(confirmationCallback != null)
                    confirmationCallback.Invoke(newReservation);
                CancelChanges();
                navigator.GoBack();
            }
        }

        ///<summary>
        /// Removes listeners, trigger on hidden in nav screen
        ///</summary>
        public void CancelChanges()
        {
            propertyDropdown.onValueChanged.RemoveAllListeners();
            roomDropdown.onValueChanged.RemoveAllListeners();
            clientInputField.onEndEdit.RemoveAllListeners();
            allowEdit = false;
        }

        ///<summary>
        /// Sets the curent client and updates the client input field text to the client name
        ///</summary>
        internal void SetClient(IClient client)
        {
            if(client != null)
            {
                currentClient = client;
                clientInputField.text = client.Name;
            }
            ValidateInput();
        }
    #endregion

    #region OpenEditPanel functions
        ///<summary>
        ///Fills/Selects the edit fields with the available data from the reservation (IReservation) parameter.
        ///<para>Use to EDIT a reservation from either the client screen or the room screen when tapping on an existing reservation's edit reservation button.</para>
        ///<para>Confirmation callback is triggered if any changes are made to the newly created, or curently edited reservation, and the save button is pressed.</para>
        ///</summary>
        internal void OpenEditReservation(IReservation reservation, UnityAction<IReservation> callback)
        {
            confirmationCallback = callback;
            period.Start = reservation.Period.Start.Date;
            period.End = reservation.Period.End.Date;
            UpdateEditableOptions(
                reservation,
                ClientDataManager.GetClient(reservation.CustomerID),
                PropertyDataManager.GetProperty(reservation.PropertyID).GetRoom(reservation.RoomID)
                );
            titleText.text = Constants.EDIT_TITLE;
            navigator.GoTo(navScreen);

        }

        ///<summary>
        /// Fills/Selects the edit fields with the available data from the client (IClient) parameter.
        ///<para>Use when adding a NEW reservation from a specific clients's new reservation button.</para>
        ///<para> Confirmation callback is triggered if any changes are made to the newly created, or curently edited reservation, and the save button is pressed.</para>
        ///</summary>
        internal void OpenAddReservation(IClient client, UnityAction<IReservation> callback)
        {
            period = ReservationDataManager.DefaultPeriod();
            confirmationCallback = callback;
            UpdateEditableOptions(null, client, null);
            titleText.text = Constants.NEW_TITLE;
            navigator.GoTo(navScreen);
        }

        ///<summary>
        /// Fills/Selects the edit fields with the available data from the room (IRoom) parameter.
        ///<para>Use when adding a NEW reservation from a specific room's new reservation button.</para>
        ///<para>Confirmation callback is triggered if any changes are made to the newly created, or curently edited reservation, and the save button is pressed.</para>
        ///</summary>
        internal void OpenAddReservation(IRoom room, UnityAction<IReservation> callback)
        {
            period = ReservationDataManager.DefaultPeriod();
            confirmationCallback = callback;
            UpdateEditableOptions(null, null, room);
            titleText.text = Constants.NEW_TITLE;
            navigator.GoTo(navScreen);
            allowEdit = true;
        }
    #endregion

    //Toggles the save/confirm reservation edit button off if there are any fields with invalid data and displays an error message
    private void ValidateInput()
    {
        if(currentProperty == null)
        {
            DisplayErrorAndSetInteractability(Constants.ERR_PROP, false, false);
            return;
        }

        if(currentRoom == null)
        {
            DisplayErrorAndSetInteractability(Constants.ERR_ROOM, false, false);
            return;
        }

        if(currentClient == null || clientInputField.text != currentClient.Name)
        {
            DisplayErrorAndSetInteractability(Constants.ERR_CLIENT, true, false);
            return;
        }

        if(period.Start == period.End)
        {
            DisplayErrorAndSetInteractability(Constants.ERR_DATES, true, false);
            return;
        }

        if(OverlapsOtherReservation(period.Start.Date, period.End.Date))
        {
            DisplayErrorAndSetInteractability(Constants.ERR_PERIOD, true, false);
            return;
        }

        DisplayErrorAndSetInteractability(string.Empty, true, true);
    }

    //Sets the selected property object as selected from the dropdown options. This also sets the room in the case of the property not having rooms
    private void SelectProperty(int optionIndex)
    {
        if(optionIndex != 0)
        {
            currentProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(optionIndex).Key);
            if(!currentProperty.HasRooms)
            {
                currentRoom = currentProperty.Rooms.ToList()[0];
            }
            else
            {
                currentRoom = null;
            }
        }
        else
        {
            currentProperty = null;
            currentRoom = null;
        }

        UpdateRoomDropdown(currentProperty);
        ValidateInput();
    }

    //Sets the selected room object as selected from the dropdown options
    private void SelectRoom(int optionIndex)
    {
        if(optionIndex != 0)
        {
            currentRoom = currentProperty.GetRoom(roomOptions.ElementAt(optionIndex).Key);
        }
        else
        {
            currentRoom = null;
        }
        ValidateInput();
    }

    //Updates all editable fields in the edit reservation screen
    private void UpdateEditableOptions(IReservation reservation, IClient client, IRoom room)
    {
        currentReservation = reservation;
        currentRoom = room;
        currentClient = client;

        UpdatePropertyDropdown();

        clientInputField.text = (client != null) ? client.Name : string.Empty;

        if(reservation != null)
        {
            UpdateReservationPeriod(reservation.Period.Start, reservation.Period.End);
        }
        else
        {
            period.Start = DateTime.Today;
            period.End = DateTime.Today;
            reservationPeriodText.text = period.Start.ToString(Constants.DateTimePrintFormat);
        }

        ValidateInput();

        propertyDropdown.onValueChanged.AddListener(SelectProperty);
        roomDropdown.onValueChanged.AddListener(SelectRoom);
        clientInputField.onEndEdit.AddListener((s) => ValidateInput());
        allowEdit = true;
    }

    //Updates the properties dropdown with all available properties with at least one room or roomles properties
    private void UpdatePropertyDropdown()
    {
        int selected = 0;
        bool searching = true;
        currentProperty = null;

        propertyOptions = new Dictionary<string, Dropdown.OptionData>();
        propertyOptions.Add(String.Empty, new Dropdown.OptionData("Alege"));
        foreach(IProperty p in PropertyDataManager.GetProperties().Where(p => p.Rooms.Count() != 0))
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

    //Updates the room dropdown opttions, is hidden if selected property has no rooms
    private void UpdateRoomDropdown(IProperty property)
    {
        if(property != null)
        {
            if(property.HasRooms)
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
        period.Start = _start;
        period.End = _end;

        reservationPeriodText.text =
            period.Start.ToString(Constants.DateTimePrintFormat)
            + Constants.AndDelimiter
            + period.End.ToString(Constants.DateTimePrintFormat);

        ValidateInput();
    }

    //Updates and enables/disables the error text to show the given error message
    private void DisplayErrorAndSetInteractability(string errorMessage, bool setDateAllowed, bool confirmAllowed)
    {
        setPeriodButton.interactable = setDateAllowed;
        confirmButton.interactable = confirmAllowed;
        errorText.text = errorMessage;
        errorText.enabled = !string.IsNullOrEmpty(errorMessage);
    }

    //Returns true if no other reservations for this room overlap the curently set period
    private bool OverlapsOtherReservation(DateTime start, DateTime end)
    {
        return ReservationDataManager.GetActiveRoomReservations(currentRoom.ID)
        .Any(r => ((currentReservation != null) ? r.ID != currentReservation.ID : r.ID != Constants.defaultCustomerName)
            && ((start.Date > r.Period.Start && start.Date < r.Period.End.Date)
            || r.Period.Start.Date > start.Date && r.Period.End.Date < end.Date
            || r.Period.Start.Date == start.Date || r.Period.End.Date == end.Date
        ));
    }
}
