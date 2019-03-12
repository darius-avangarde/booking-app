﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OccupancyGraphScreen : MonoBehaviour
{
    [SerializeField]
    private ModalCalendarStatistics modalCalendar = null;
    [SerializeField]
    private FilterDialog modalFilterDialog = null;
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