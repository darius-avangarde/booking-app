using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OccupancyGraphScreen : MonoBehaviour
{
    private const string INFO_MESSAGE =
@"În funcție de opțiunea selectată mai jos graficul va afișa gradul de ocupare in diferite moduri.
Gradul de ocupare se referă la numărul de zile rezervate raportat la numărul maxim de zile rezervate (număr de camere X număr de zile în perioada selectată).

Timp: Fiecare coloană reprezintă gradul de ocupare pentru ziua respectivă.

Locație: Fiecare coloană reprezintă gradul de ocupare pentru proprietatea respectivă.

Tip cameră: Fiecare coloană reprezintă gradul de ocupare pentru categoria de cameră respectivă (număr de persoane).

Camere: Fiecare coloană reprezintă gradul de ocupare pentru camera respectivă.";

    [SerializeField]
    private ModalCalendarStatistics modalCalendar = null;
    [SerializeField]
    private FilterDialog modalFilterDialog = null;
    [SerializeField]
    private InfoBox infoBox = null;
    [SerializeField]
    private GraphComponent graphComponent = null;
    [SerializeField]
    private FilterButton filterButton = null;
    [SerializeField]
    private Text statisticsPeriodText = null;
    [SerializeField]
    private Dropdown xAxisTypeValueDropdown = null;
    private DateTime startDateTimePeriod;
    private DateTime endDateTimePeriod;
    private RoomFilter filter = new RoomFilter();
    private List<IRoom> filteredRoomList = new List<IRoom>();
    private List<IReservation> reservationList = new List<IReservation>();
    private List<string> xAxisTypeValueList = new List<string>();
    private int xAxisTypeValueIndex = 0;

    void Start()
    {
        DateTime today = DateTime.Today;
        startDateTimePeriod = new DateTime(today.Year, today.Month, 1);
        endDateTimePeriod = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

        ShowUpdatedStatisticsPeriod(startDateTimePeriod, endDateTimePeriod);
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            filteredRoomList.AddRange(property.Rooms);
        }
        reservationList.AddRange(ReservationDataManager.GetReservations());
        filterButton.UpdateFilterButtonText(filter);
        InitializeDropdown();
        graphComponent.UpdateGraph(filteredRoomList, startDateTimePeriod, endDateTimePeriod, reservationList, xAxisTypeValueIndex);
    }

    public void UpdateOccupancyGraphScreen()
    {
        reservationList = new List<IReservation>();
        reservationList.AddRange(ReservationDataManager.GetReservations());
        FilterList(filter);
        graphComponent.UpdateGraph(filteredRoomList, startDateTimePeriod, endDateTimePeriod, reservationList, xAxisTypeValueIndex);
    }

    public void ShowModalCalendar()
    {
        modalCalendar.Show(ShowUpdatedStatisticsPeriod);
    }

    public void ShowFilterDialog()
    {
        modalFilterDialog.Show(filter, (updatedFilter) => {
            FilterList(updatedFilter);
        });
    }

    public void ShowInfo()
    {
        infoBox.Show(INFO_MESSAGE);
    }

    public void SetXAxisValueType()
    {
        xAxisTypeValueIndex = xAxisTypeValueDropdown.value;
        graphComponent.UpdateGraph(filteredRoomList, startDateTimePeriod, endDateTimePeriod, reservationList, xAxisTypeValueIndex);
    }

    private void ShowUpdatedStatisticsPeriod(DateTime startDateTime, DateTime endDateTime)
    {
        startDateTimePeriod = startDateTime;
        endDateTimePeriod = endDateTime;
        statisticsPeriodText.text = startDateTimePeriod.ToString(Constants.DateTimePrintFormat)
                                   + Constants.AndDelimiter
                                   + endDateTimePeriod.ToString(Constants.DateTimePrintFormat);

        graphComponent.UpdateGraph(filteredRoomList, startDateTimePeriod, endDateTimePeriod, reservationList, xAxisTypeValueIndex);
    }

    private void InitializeDropdown()
    {
        xAxisTypeValueDropdown.ClearOptions();
        xAxisTypeValueList.AddRange(Constants.XAxisDict.Values);
        xAxisTypeValueDropdown.AddOptions(xAxisTypeValueList);
    }

    private void FilterList(RoomFilter updatedFilter)
    {
        filteredRoomList = new List<IRoom>();
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            filteredRoomList.AddRange(property.Rooms);
        }
        filteredRoomList = updatedFilter.Apply(filteredRoomList);
        filterButton.UpdateFilterButtonText(updatedFilter);
        graphComponent.UpdateGraph(filteredRoomList, startDateTimePeriod, endDateTimePeriod, reservationList, xAxisTypeValueIndex);
    }
}
