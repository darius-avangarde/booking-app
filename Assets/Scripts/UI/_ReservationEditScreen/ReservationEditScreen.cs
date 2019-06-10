using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UINavigation;
using UnityEngine.Events;

public class ReservationEditScreen : MonoBehaviour
{
    internal bool AllowEdit { get; set; } = false;

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

        [Header("Screen references")]
        [SerializeField]
        private ClientsScreen clientsScreen;

        [Space]
        [SerializeField]
        private Text titleText = null;
        [SerializeField]
        private Dropdown propertyDropdown = null;
        [SerializeField]
        private Text reservationPeriodText = null;
        [SerializeField]
        private Button confirmButton = null;
        [SerializeField]
        private Text errorText = null;
        [SerializeField]
        private Text roomButtonText;
        [SerializeField]
        private GameObject roomButton;
        [SerializeField]
        private Text clientButtonText;
        [SerializeField]
        private GameObject deleteReservationButton;
        [SerializeField]
        private RectTransform editablesRect;
        [SerializeField]
        private RectTransform editablesReferenceObject;
    #endregion

    #region Private variables
        private Dictionary<string,Dropdown.OptionData> propertyOptions;

        private IReservation currentReservation;
        private List<IRoom> currentRooms;
        private IProperty currentProperty;
        private IClient currentClient;

        private DateTime periodStart;
        private DateTime periodEnd;
        private ConfirmationDialogOptions editConfirmation;
        private ConfirmationDialogOptions deleteConfirmation;
        private UnityAction<IReservation> confirmationCallback;
        private UnityAction deletionCallback;
    #endregion

    private void Start()
    {
        AllowEdit = false;
        errorText.enabled = false;
        editConfirmation = new ConfirmationDialogOptions();
        editConfirmation.Message = Constants.EDIT_DIALOG;
        deleteConfirmation = new ConfirmationDialogOptions();
        deleteConfirmation.Message = Constants.DELETE_DIALOG;
        periodStart = DateTime.Today.Date;
        periodEnd = periodStart.AddDays(1).Date;
    }

    private void OnDestroy()
    {
        propertyDropdown.onValueChanged.RemoveAllListeners();
    }

    #region Public functions
        public void EnablePropertyDropdownListeners()
        {
            propertyDropdown.onValueChanged.AddListener(SetProperty);
        }

        ///<summary>
        /// Opens the modal calendar overlay if a property is selected in order to change or set the reservation period
        ///</summary>
        public void ChangePeriod()
        {
            string reservationID = (currentReservation != null) ? currentReservation.ID : Constants.defaultCustomerName;

            if(currentRooms != null && currentRooms.Count > 0)
            {
                modalCalendarDialog.OpenCallendar(
                    currentReservation,
                    ReservationDataManager.GetReservations()
                        .Where(r => r.ID != reservationID && r.RoomIDs.Any(s => currentRooms.Any(ro => ro.ID == s)))
                        .ToList(),
                    UpdateReservationPeriod
                    );
            }

            else
            {
                modalCalendarDialog.OpenCallendar(periodStart, periodEnd, UpdateReservationPeriod);
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
                        currentRooms,
                        currentClient,
                        periodStart,
                        periodEnd
                    );
                    if(confirmationCallback != null)
                        confirmationCallback.Invoke(currentReservation);
                    navigator.GoBack();
                };
                confirmationDialog.Show(editConfirmation);
            }
            else
            {
                IReservation newReservation = ReservationDataManager.AddReservation(
                    currentRooms,
                    currentClient,
                    periodStart,
                    periodEnd
                );
                if(confirmationCallback != null)
                    confirmationCallback.Invoke(newReservation);
                navigator.GoBack();
            }
        }

        ///<summary>
        /// Removes listeners, trigger on hidden in nav screen
        ///</summary>
        public void CancelChanges()
        {
            confirmationCallback = null;
            deletionCallback = null;
            propertyDropdown.onValueChanged.RemoveAllListeners();
            AllowEdit = false;
        }

        public void RequestDelete()
        {
            deleteConfirmation.ConfirmCallback = DeleteReservation;
            confirmationDialog.Show(deleteConfirmation);
        }

        public void SelectClient()
        {
            clientsScreen.OpenClientReservation(SetClient);
        }

        //TODO: Integrate with room screen
        public void SelectRoom()
        {
            string selectedRooms = string.Empty;
            if(currentRooms != null)
            foreach(IRoom r in currentRooms)
            {
                selectedRooms += r.Name + ", ";
            }

            Debug.Log("Unimplemented >Room screen > rooms: " + selectedRooms);
            Debug.Log("Selecting random room");
            List<IRoom> tr = new List<IRoom>();
            tr.Add(currentProperty.Rooms.ToList()[UnityEngine.Random.Range(0, currentProperty.Rooms.Count())]);
            SetRooms(tr);

            //Go to room screen OpenRoomScreen(Datetime start, end, selection = curentRooms, callback = SetRooms)
        }
    #endregion

    #region OpenEditPanel functions
        ///<summary>
        ///Fills/Selects the edit fields with the available data from the reservation (IReservation) parameter.
        ///<para>Use to EDIT a reservation from either the client screen or the room screen when tapping on an existing reservation's edit reservation button.</para>
        ///<para>Confirmation callback is triggered if any changes are made to the newly created, or curently edited reservation, and the save button is pressed.</para>
        ///<para>Cancelation callback is triggered when the panel is exited without saving modifications.</para>
        ///</summary>
        internal void OpenEditReservation(IReservation reservation, UnityAction<IReservation> confirmCallback, UnityAction deleteCallback = null)
        {
            confirmationCallback = confirmCallback;
            deletionCallback = deleteCallback;
            periodStart = reservation.Period.Start.Date;
            periodEnd = reservation.Period.End.Date;
            UpdateEditableOptions(
                reservation,
                ClientDataManager.GetClient(reservation.CustomerID),
                PropertyDataManager.GetProperty(reservation.PropertyID).Rooms.Where(r => reservation.RoomIDs.Contains(r.ID)).ToList()
                );
            titleText.text = Constants.EDIT_TITLE;
            navigator.GoTo(navScreen);

        }

        ///<summary>
        /// Fills/Selects the edit fields with the available data from the client (IClient) parameter.
        ///<para>Use when adding a NEW reservation from a specific clients's new reservation button.</para>
        ///<para> Confirmation callback is triggered if any changes are made to the newly created, or curently edited reservation, and the save button is pressed.</para>
        ///<para>Cancelation callback is triggered when the panel is exited without saving modifications.</para>
        ///</summary>
        internal void OpenAddReservation(IClient client, UnityAction<IReservation> confirmCallback)
        {
            periodStart = DateTime.Today.Date;
            periodEnd = periodStart.AddDays(1).Date;
            confirmationCallback = confirmCallback;
            deletionCallback = null;
            UpdateEditableOptions(null, client, null, true);
            titleText.text = Constants.NEW_TITLE;
            navigator.GoTo(navScreen);
        }

        ///<summary>
        /// Fills/Selects the edit fields with the available data from the room (IRoom) parameter.
        ///<para>Use when adding a NEW reservation from a specific room's new reservation button.</para>
        ///<para>Confirmation callback is triggered if any changes are made to the newly created, or curently edited reservation, and the save button is pressed.</para>
        ///<para>Cancelation callback is triggered when the panel is exited without saving modifications.</para>
        ///</summary>
        internal void OpenAddReservation(DateTime start, DateTime end, List<IRoom> rooms, UnityAction<IReservation> confirmCallback)
        {
            periodStart = start.Date;
            periodEnd = end.Date;
            confirmationCallback = confirmCallback;
            deletionCallback = null;
            UpdateEditableOptions(null, null, rooms);
            titleText.text = Constants.NEW_TITLE;
            navigator.GoTo(navScreen);
            AllowEdit = true;
        }

        ///<summary>
        /// Fills/Selects the edit fields with the available data from the room (IRoom) parameter.
        ///<para>Use when adding a NEW reservation from a specific room's new reservation button.</para>
        ///<para>Confirmation callback is triggered if any changes are made to the newly created, or curently edited reservation, and the save button is pressed.</para>
        ///<para>Cancelation callback is triggered when the panel is exited without saving modifications.</para>
        ///</summary>
        internal void OpenAddReservation(DateTime start, DateTime end, IRoom room, UnityAction<IReservation> confirmCallback)
        {
            periodStart = start.Date;
            periodEnd = end.Date;
            confirmationCallback = confirmCallback;
            deletionCallback = null;
            List<IRoom> sr = new List<IRoom>();
            sr.Add(room);
            UpdateEditableOptions(null, null, sr);
            titleText.text = Constants.NEW_TITLE;
            navigator.GoTo(navScreen);
            AllowEdit = true;
        }

        //TODO: Remove once new functions are implemented
        ///<summary>
        /// Obsolete, use
        ///<para> OpenAddReservation(DateTime, DateTime, IRoom/List(Iroom), UnityAction(IReservation)) instead </para>
        ///</summary>
        internal void OpenAddReservation(IRoom room, UnityAction<IReservation> confirmCallback)
        {
            periodStart = DateTime.Today.Date;
            periodEnd = DateTime.Today.AddDays(1).Date;
            confirmationCallback = confirmCallback;
            deletionCallback = null;
            List<IRoom> sr = new List<IRoom>();
            sr.Add(room);
            UpdateEditableOptions(null, null, sr);
            titleText.text = Constants.NEW_TITLE;
            navigator.GoTo(navScreen);
            AllowEdit = true;
        }

    #endregion

    private void SetClient(IClient client)
    {
        if(client != null)
        {
            currentClient = client;
            clientButtonText.text = client.Name;
        }
        else
        {
            clientButtonText.text = Constants.CHOOSE;
        }

        ValidateInput();
    }

    private void DeleteReservation()
    {
        if(deletionCallback != null)
        {
            deletionCallback.Invoke();
        }
        ReservationDataManager.DeleteReservation(currentReservation.ID);
        currentReservation = null;
        deleteConfirmation.ConfirmCallback = null;
        navigator.GoBack();
    }

    //Toggles the save/confirm reservation edit button off if there are any fields with invalid data and displays an error message
    private void ValidateInput()
    {
        if(currentProperty == null)
        {
            SetErrorAndInteractability(Constants.ERR_PROP, false);
            return;
        }

        if(currentRooms == null || currentRooms.Count < 0)
        {
            SetErrorAndInteractability(Constants.ERR_ROOM, false);
            return;
        }

        if(periodStart == periodEnd)
        {
            SetErrorAndInteractability(Constants.ERR_DATES, false);
            return;
        }

        if(OverlapsOtherReservation(periodStart.Date, periodEnd.Date))
        {
            SetErrorAndInteractability(Constants.ERR_PERIOD, false);
            return;
        }

        if(currentClient == null)
        {
            SetErrorAndInteractability(Constants.ERR_CLIENT, false);
            return;
        }

        SetErrorAndInteractability(string.Empty, true);
    }

    private void SetRooms(List<IRoom> rooms)
    {
        currentRooms = new List<IRoom>();
        currentRooms = rooms;

        if(rooms != null)
        {
            if(rooms.Count == 1)
            {
                roomButtonText.text = rooms[0].Name;
            }
            else
            {
                roomButtonText.text = rooms.Count + Constants.SPACE + Constants.ROOMS_SELECTED;
            }
        }
        else
        {
            roomButtonText.text = Constants.CHOOSE;
        }

        ValidateInput();
    }

    //Updates all editable fields in the edit reservation screen
    private void UpdateEditableOptions(IReservation reservation, IClient client, List<IRoom> rooms, bool fromClient = false)
    {
        currentReservation = reservation;
        currentRooms = rooms;
        currentClient = client;

        InitializePropertyDropdown();

        clientButtonText.text = (client != null) ? client.Name : Constants.CHOOSE;

        if(reservation != null)
        {
            deleteReservationButton.SetActive(true);
            UpdateReservationPeriod(reservation.Period.Start, reservation.Period.End);
        }
        else
        {
            deleteReservationButton.SetActive(false);
            UpdateReservationPeriod(periodStart, periodEnd);
            if(fromClient)
            {
                reservationPeriodText.text += Constants.SPACE + Constants.CHOOSE + Constants.SPACE + Constants.RESERVATION_PERIOD;
            }
        }

        if(rooms != null)
        {
            if(rooms.Count == 1)
            {
                roomButton.SetActive(PropertyDataManager.GetProperty(rooms[0].PropertyID).HasRooms);
                roomButtonText.text = rooms[0].Name;
            }
            else
            {
                roomButton.SetActive(true);
                roomButtonText.text = rooms.Count + Constants.SPACE + Constants.ROOMS_SELECTED;
            }
        }
        else
        {
            roomButton.SetActive(false);
            roomButtonText.text = Constants.CHOOSE;
        }

        SizeEditablesRect();
        ValidateInput();

        AllowEdit = true;
    }

    //Sets the selected property object as selected from the dropdown options. This also sets the room in the case of the property not having rooms
    private void SetProperty(int optionIndex)
    {
        if(optionIndex != 0)
        {
            roomButtonText.text = Constants.CHOOSE;
            currentProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(optionIndex).Key);
            if(!currentProperty.HasRooms)
            {
                roomButton.SetActive(false);
                currentRooms = new List<IRoom>();
                currentRooms.Add(currentProperty.Rooms.ToList()[0]);
            }
            else
            {
                roomButton.SetActive(true);
                currentRooms = null;
            }
        }
        else
        {
            roomButton.SetActive(false);
            currentProperty = null;
            currentRooms = null;
        }

        SizeEditablesRect();
        ValidateInput();
    }

    //Updates the properties dropdown with all available properties with at least one room or roomles properties
    private void InitializePropertyDropdown()
    {
        int selected = 0;
        bool searching = true;
        currentProperty = null;

        propertyOptions = new Dictionary<string, Dropdown.OptionData>();
        propertyOptions.Add(String.Empty, new Dropdown.OptionData(Constants.CHOOSE));
        foreach(IProperty p in PropertyDataManager.GetProperties().Where(p => p.Rooms.Count() != 0))
        {
            propertyOptions.Add(p.ID, new Dropdown.OptionData(p.Name));


            if (currentRooms != null)
            {
                if(searching)
                {
                    selected++;
                }

                if(currentRooms.Any(r => r.PropertyID == p.ID))
                {
                    currentProperty = p;
                    searching = false;
                }
            }
        }

        propertyDropdown.options = propertyOptions.Values.ToList();

        propertyDropdown.value = (searching) ? 0 : selected;
        propertyDropdown.RefreshShownValue();
    }

    //Callback function for the modal calendar overlay, sets selected start and end times
    private void UpdateReservationPeriod(DateTime _start, DateTime _end)
    {
        periodStart = _start;
        periodEnd = _end;

        reservationPeriodText.text =
            periodStart.ToString(Constants.DateTimePrintFormat)
            + Constants.AndDelimiter
            + periodEnd.ToString(Constants.DateTimePrintFormat);

        ValidateInput();
    }

    //Updates and enables/disables the error text to show the given error message
    private void SetErrorAndInteractability(string errorMessage, bool confirmAllowed)
    {
        confirmButton.interactable = confirmAllowed;
        errorText.text = errorMessage;
        errorText.enabled = !string.IsNullOrEmpty(errorMessage);
    }

    //Returns true if no other reservations for this room overlap the curently set period
    private bool OverlapsOtherReservation(DateTime start, DateTime end)
    {
        string currentResId = (currentReservation != null) ? currentReservation.ID : Constants.defaultCustomerName;

        return ReservationDataManager.GetReservations().Where(r => currentRooms.Any(room => r.ContainsRoom(room.ID))
            && r.ID != currentResId)  //get room reservation excluding current curent
            .Any(r =>
            ((start.Date > r.Period.Start.Date && start.Date < r.Period.End.Date) //start in period
            || (end.Date > r.Period.Start.Date   && end.Date < r.Period.End.Date)   //end in period
            || (start.Date < r.Period.Start.Date && end.Date > r.Period.End.Date)   //selection engulfs other reservation
            || r.Period.Start.Date == periodStart.Date || r.Period.End.Date == periodEnd.Date   //start or end coincide
            ));
    }

    private void SizeEditablesRect()
    {
        editablesRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, editablesReferenceObject.rect.height * ((roomButton.activeSelf) ? 4 : 3));
    }
}
