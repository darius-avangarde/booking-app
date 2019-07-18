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
        // [SerializeField]
        // private ClientPicker object;
    #endregion

    private UnityAction<ReservationFilter> callback;

    private ReservationFilter activeFilter = new ReservationFilter();


    //toggle functions for each group
        //on toggle off remove active filter options

    #region ButtonActions/Callbacks
        public void SetDate(bool isStart)
        {
            if(isStart)
            {
                modalCalendar.OpenCallendar((d) => activeFilter.startDate = d.Date);
            }
            else
            {
                modalCalendar.OpenCallendar((d) => activeFilter.endDate = d.Date);
            }
        }
    #endregion



    public void OpenFilterScreen(UnityAction<ReservationFilter> filterCallback = null)
    {
        UpdateUI();
        callback = filterCallback;
        navigator.GoTo(navScreen);
    }

    public void ApplyFilters()
    {
        activeFilter.roomType = roomTypeDropdown.CurrentRoomType;
        Vector2Int bedsInDrop = roomBedsDropdown.GetCurrentBeds();
        if (bedsInDrop.x != 0 || bedsInDrop.y != 0)
        {
            activeFilter.beds = bedsInDrop;
        }
        else
        {
            activeFilter.beds = null;
        }

        callback?.Invoke(activeFilter);
        navigator.GoBack();
    }

    public void CancelFilters()
    {
        activeFilter = null;
        navigator.GoBack();
    }

    public void ClearActiveFilters()
    {
        activeFilter = new ReservationFilter();
    }

    private void UpdateUI()
    {
        roomBedsToggle.Toggle(false);
        roomTypeToggle.Toggle(false);
        roomBedsToggle.Toggle(false);
        clientToggle.Toggle(false);

        if(activeFilter != null)
        {
            if(activeFilter.startDate != null || activeFilter.endDate != null)
            {
                if(activeFilter.startDate != null)
                {
                startDateText.text  = activeFilter.startDate.Value.ToString(Constants.DateTimePrintFormat);
                }

                if(activeFilter.endDate != null)
                {
                    endDateText.text      = activeFilter.endDate.Value.ToString(Constants.DateTimePrintFormat);
                }
                roomBedsToggle.Toggle(true);
            }
            if(activeFilter.roomType != null)
            {
                roomTypeDropdown.CurrentRoomType = activeFilter.roomType.Value;
                roomTypeToggle.Toggle(true);
            }
            if(activeFilter.beds != null)
            {
                roomBedsDropdown.SetCurrentBeds(activeFilter.beds.Value);
                roomBedsToggle.Toggle(true);
            }
            if(activeFilter.client != null)
            {
                clientToggle.Toggle(true);

            }
        }
    }
}
