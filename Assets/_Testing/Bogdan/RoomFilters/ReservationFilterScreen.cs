using System;
using UnityEngine;
using UnityEngine.UI;

public class ReservationFilterScreen : MonoBehaviour
{
    [SerializeField]
    private Text startDateText;
    [SerializeField]
    private Text endDateText;
    [SerializeField]
    private Dropdown roomTypeDropdown;
    // [SerializeField]
    // private ClientPicker object;


    private ReservationFilter activeFilter = new ReservationFilter();


    public void OpenFilterScreen()
    {

    }

    public void ClearActiveFilters()
    {
        activeFilter = new ReservationFilter();
    }

    private void UpdateUI()
    {
        if(activeFilter.startDate != null)  startDateText.text  = activeFilter.startDate.Value.ToString(Constants.DateTimePrintFormat);
        if(activeFilter.endDate != null)  endDateText.text      = activeFilter.endDate.Value.ToString(Constants.DateTimePrintFormat);
        //Update room dropdown
        //if(activeFilter.client != null)  client.text      =

    }
}
