using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReservationFilterScreen : MonoBehaviour
{
    [SerializeField]
    private UINavigation.Navigator navigator;
    [SerializeField]
    private UINavigation.NavScreen navScreen;
    [SerializeField]
    private ReservationsCalendarManager calendarManager;
    [SerializeField]
    private ReservationsFilterButton filterButton;
    [SerializeField]
    private ModalCalendar modalCalendar;

    #region Filter Toggles
        [Space]
        [SerializeField]
        private ReservationFilterToggle periodToggle;
        [SerializeField]
        private ReservationFilterToggle roomTypeToggle;
        [SerializeField]
        private ReservationFilterToggle roomBedsToggle;
        [SerializeField]
        private ReservationFilterToggle clientToggle;
    #endregion

    #region UI_References
        [Space]
        [SerializeField]
        private Text startDateText;
        [SerializeField]
        private Text endDateText;
        [SerializeField]
        private SetRoomTypeDropdown roomTypeDropdown;
        [SerializeField]
        private SetBedsNumber roomBedsDropdown;
        [SerializeField]
        private ClientPicker clientPicker;
    #endregion

    private DateTime? startDate = null;
    private DateTime? endDate = null;
    private PropertyDataManager.RoomType? roomType = null;
    private Vector2Int? roomBeds = null;
    private IClient client = null;

    private UnityAction<ReservationFilter> callback;
    private ReservationFilter currentFilter = new ReservationFilter();


    #region ScreenFunctions
        public void OpenFilterScreen(UnityAction<ReservationFilter> filtersCallback, ReservationFilter filter = null)
        {
            if(filter == null)
                currentFilter = new ReservationFilter();

            callback = filtersCallback;
            UpdateFilerFields(true, currentFilter);
            clientPicker.SetCallback(SetClientCallback, currentFilter.Client);
            navigator.GoTo(navScreen);
        }

        public void CancelFilters()
        {
            navigator.GoBack();
        }

        public void ClearFilters()
        {
            startDate = null;
            endDate = null;
            roomType = null;
            roomBeds = null;
            client = null;
            filterButton.Close();
            callback?.Invoke(null);
        }

        public void ApplyFilters()
        {
            if(periodToggle.IsOn)
            {
                currentFilter.StartDate = (startDate != null && startDate.HasValue) ? startDate.Value.Date : DateTime.Today.Date;
                currentFilter.EndDate = (endDate != null && endDate.HasValue) ? endDate.Value.Date : currentFilter.StartDate.Value.AddDays(1).Date;
            }
            else
            {
                currentFilter.StartDate = null;
                currentFilter.EndDate = null;
            }

            if(roomTypeToggle.IsOn)
            {
                currentFilter.RoomType = (PropertyDataManager.RoomType?)roomTypeDropdown.CurrentRoomType;
            }
            else
            {
                currentFilter.RoomType = null;
            }

            if(roomBedsToggle.IsOn)
            {
                currentFilter.RoomBeds = (Vector2Int?)roomBedsDropdown.GetCurrentBeds();
            }
            else
            {
                currentFilter.RoomBeds = null;
            }

            if (clientToggle.IsOn && client != null)
            {
                currentFilter.Client = client;
            }
            else
            {
                currentFilter.Client = null;
            }

            if(currentFilter.Client == null && !periodToggle.IsOn && !roomTypeToggle.IsOn && !roomBedsToggle.IsOn)
            {
                currentFilter = null;
                navigator.GoBack();
                filterButton.Close();
                callback?.Invoke(currentFilter);
                return;
            }

            navigator.GoBack();
            filterButton.Open();
            callback?.Invoke(currentFilter);
        }
    #endregion

    public void SetDate(bool isStart)
    {
        modalCalendar.OpenCallendar((d) => SetDatesCallback(d, isStart));
    }

    private void SetDatesCallback(DateTime date, bool isStart)
    {
        if(isStart)
        {
            startDate = date.Date;
        }
        else if (startDate.HasValue && startDate.Value > date.Date)
        {
            endDate = startDate;
            startDate = date.Date;
        }
        else
        {
            endDate = date.Date;
        }

        startDateText.text = startDate.HasValue ? startDate.Value.ToString(Constants.DateTimePrintFormat) : "Din";
        endDateText.text = endDate.HasValue ? endDate.Value.ToString(Constants.DateTimePrintFormat) : "Până în";
    }

    private void SetClientCallback(IClient selectedClient)
    {
        client = selectedClient;
    }

    private void UpdateFilerFields(bool alsoToggle, ReservationFilter currentFilter)
    {
        if(currentFilter != null)
        {
            startDate = currentFilter.StartDate;
            endDate = currentFilter.EndDate;
            roomType = currentFilter.RoomType;
            roomBeds = currentFilter.RoomBeds;
            client = currentFilter.Client;
        }
        else
        {
            startDate = null;
            endDate = null;
            roomType = null;
            roomBeds = null;
            client = null;
        }

        if(startDate != null || endDate != null)
        {
            if(alsoToggle) periodToggle.Toggle(true);
            startDateText.text = startDate.Value.ToString(Constants.DateTimePrintFormat);
            endDateText.text = endDate.Value.ToString(Constants.DateTimePrintFormat);
        }
        else
        {
            if(alsoToggle) periodToggle.Toggle(false);
            startDateText.text = "Din";
            endDateText.text = "Până în";
        }

        if (roomType != null)
        {
            if(alsoToggle) roomTypeToggle.Toggle(true);
            roomTypeDropdown.CurrentRoomType = roomType.Value;
        }
        else
        {
            if(alsoToggle) roomTypeToggle.Toggle(false);
            roomTypeDropdown.CurrentRoomType = default;
        }

        if (roomBeds != null)
        {
            if(alsoToggle) roomBedsToggle.Toggle(true);
            roomBedsDropdown.SetCurrentBeds(roomBeds.Value);
        }
        else
        {
            if(alsoToggle) roomBedsToggle.Toggle(false);
            roomBedsDropdown.SetCurrentBeds(Vector2Int.zero);
        }

        if (client != null)
        {
            if(alsoToggle) clientToggle.Toggle(true);
        }
        else
        {
            if(alsoToggle) clientToggle.Toggle(false);
        }
    }

}

public class ReservationFilter
{
    public DateTime? StartDate = null;
    public DateTime? EndDate = null;
    public PropertyDataManager.RoomType? RoomType = null;
    public Vector2Int? RoomBeds = null;
    public IClient Client = null;
}
