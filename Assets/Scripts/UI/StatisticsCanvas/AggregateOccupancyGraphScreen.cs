using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AggregateOccupancyGraphScreen : MonoBehaviour
{
    [SerializeField]
    private ModalCalendarStatistics modalCalendar = null;
    [SerializeField]
    private FilterDialog modalFilterDialog = null;
    [SerializeField]
    private AggregateGraphComponent graphComponent = null;
    [Header("Filter Button Text Components In Aggregate Occupancy Graph Screen")]
    [SerializeField]
    private Text propertyInfoText = null;
    [SerializeField]
    private Text roomCapacityText = null;
    [SerializeField]
    private Text singleBedText = null;
    [SerializeField]
    private Text doubleBedText = null;
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
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            filteredRoomList.AddRange(property.Rooms);
        }
        reservationList.AddRange(ReservationDataManager.GetReservations());
        UpdateFilterButtonText();
        InitializeDropdown();
        graphComponent.UpdateGraph(filteredRoomList, startDateTimePeriod, endDateTimePeriod, reservationList, xAxisTypeValueIndex);
    }

    public void UpdateAggregateOccupancyGraphScreen()
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

    public void SetXAxisValueType()
    {
        xAxisTypeValueIndex = xAxisTypeValueDropdown.value;
        graphComponent.UpdateGraph(filteredRoomList, startDateTimePeriod, endDateTimePeriod, reservationList, xAxisTypeValueIndex);
    }

    private void UpdateFilterButtonText()
    {
        string propertyInfo = "";
        string roomCapacityInfo = Constants.Persoane + filter.RoomCapacity.ToString();
        string singleBedInfo = Constants.SingleBed + filter.SingleBeds;
        string doubleBedInfo = Constants.DoubleBed + filter.DoubleBeds;

        if (!string.IsNullOrEmpty(filter.PropertyID))
        {
            propertyInfo = Constants.Proprietate + PropertyDataManager.GetProperty(filter.PropertyID).Name;
        }

        propertyInfoText.text = propertyInfo;
        roomCapacityText.text = roomCapacityInfo;
        singleBedText.text = singleBedInfo;
        doubleBedText.text = doubleBedInfo;
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
        xAxisTypeValueList.AddRange(Constants.AggregateXAxisDict.Values);
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
        UpdateFilterButtonText();
        graphComponent.UpdateGraph(filteredRoomList, startDateTimePeriod, endDateTimePeriod, reservationList, xAxisTypeValueIndex);
    }
}

