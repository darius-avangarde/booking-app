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

    #region UI_References
        [SerializeField]
        private Text startDateText;
        [SerializeField]
        private Text endDateText;
        [SerializeField]
        private Dropdown roomTypeDropdown;
        // [SerializeField]
        // private ClientPicker object;
    #endregion

    private UnityAction<ReservationFilter> callback;

    private ReservationFilter activeFilter = new ReservationFilter();


    //toggle functions for each group
        //on toggle off remove active filter options

    public void OpenFilterScreen(UnityAction<ReservationFilter> filterCallback = null)
    {
        UpdateUI();
        callback = filterCallback;
        navigator.GoTo(navScreen);
    }

    public void ApplyFilters()
    {
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
        if(activeFilter != null)
        {
            if(activeFilter.startDate != null)  startDateText.text  = activeFilter.startDate.Value.ToString(Constants.DateTimePrintFormat);
            if(activeFilter.endDate != null)  endDateText.text      = activeFilter.endDate.Value.ToString(Constants.DateTimePrintFormat);
            //Update room dropdown
            //if(activeFilter.client != null)  client.text      =
        }
        else
        {

        }

    }
}
