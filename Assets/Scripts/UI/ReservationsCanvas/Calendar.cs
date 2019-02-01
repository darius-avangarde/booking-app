using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private CalendarScreen calendarScreen = null;
    [SerializeField]
    private Transform dayScreen = null;
    [SerializeField]
    private Text monthName = null;
    [SerializeField]
    private Transform dayItemsInCalendarPanel = null;
    private DateTime selectedDateTime;
    private List<IRoom> filteredRooms;

    // Start is called before the first frame update
    void Start()
    {
        filteredRooms = calendarScreen.GetRoomsInFilteredRoomsList();
        InstantiateCalendarDayItems();
        selectedDateTime = DateTime.Today;
        UpdateCalendar(selectedDateTime);
    }

    public void UpdateCalendarAfterFiltering()
    {
        filteredRooms = calendarScreen.filteredRooms;
        UpdateCalendar(selectedDateTime);
    }

    public void UpdateCalendarOnNewReservation()
    {
        filteredRooms = calendarScreen.filteredRooms;
        UpdateCalendar(selectedDateTime);
        dayScreen.GetComponent<DayScreen>().UpdateFilteredDayScreenPropertyItemsContent();
    }

    public void ShowPreviousMonth()
    {
        selectedDateTime = selectedDateTime.AddMonths(-1);
        UpdateCalendar(selectedDateTime);
    }

    public void ShowNextMonth()
    {
        selectedDateTime = selectedDateTime.AddMonths(1);
        UpdateCalendar(selectedDateTime);
    }

    private void InstantiateCalendarDayItems()
    {
        for (int dayItemIndex = 0; dayItemIndex < dayItemsInCalendarPanel.childCount; dayItemIndex++)
        {
            CalendarDayItem dayItem = dayItemsInCalendarPanel.GetChild(dayItemIndex).GetComponent<CalendarDayItem>();
            dayItem.Initialize(OpenDayScreen);
        }
    }

    private void OpenDayScreen(DateTime dateTime, List<IRoom> rooms)
    {
        dayScreen.GetComponent<DayScreen>().UpdateDayScreenInfo(dateTime, rooms);
        navigator.GoTo(dayScreen.GetComponent<NavScreen>());
    }

    private void UpdateCalendar(DateTime selectedDateTime)
    {
        DateTime firstDayOfMonthInSelectedDate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, 1, 0, 0, 0, DateTimeKind.Local);
        
        SetDayItemsForPreviousMonth(selectedDateTime, firstDayOfMonthInSelectedDate);
        
        SetDayItemsForCurrentMonth(selectedDateTime, firstDayOfMonthInSelectedDate);
        
        SetDayItemsForNextMonth(selectedDateTime, firstDayOfMonthInSelectedDate);
    }

    private void SetDayItemsForNextMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {

        DateTime nextMonthDateTime = selectedDateTime.AddMonths(1);
        int daysInSelectedMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int firstDayFromNextMonthIndex = daysInSelectedMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromNextMonth = 1;

        for (int dayItemIndex = firstDayFromNextMonthIndex; dayItemIndex < dayItemsInCalendarPanel.childCount; dayItemIndex++)
        {
            nextMonthDateTime =
                new DateTime(nextMonthDateTime.Year, nextMonthDateTime.Month, dayVisibleFromNextMonth, 0, 0, 0, DateTimeKind.Local);
            CalendarDayItem dayItem = dayItemsInCalendarPanel.GetChild(dayItemIndex).GetComponent<CalendarDayItem>();
            dayItem.UpdateDayItem(nextMonthDateTime, false, filteredRooms);
            dayVisibleFromNextMonth++;
        }
    }

    private void SetDayItemsForCurrentMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        monthName.text = Constants.monthNamesDict[selectedDateTime.Month] + " " + selectedDateTime.Year;
        int daysInMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int daysVisibleInCurrentMonth = daysInMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromSelectedMonth = 1;

        for (int i = daysVisibleFromPreviousMonth; i < daysVisibleInCurrentMonth; i++)
        {
            selectedDateTime = 
                new DateTime(selectedDateTime.Year, selectedDateTime.Month, dayVisibleFromSelectedMonth, 0, 0, 0, DateTimeKind.Local);
            CalendarDayItem dayItem = dayItemsInCalendarPanel.GetChild(i).GetComponent<CalendarDayItem>();
            dayItem.UpdateDayItem(selectedDateTime, true, filteredRooms);
            dayVisibleFromSelectedMonth++;
        }
    }

    private void SetDayItemsForPreviousMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        DateTime previousMonthDateTime = selectedDateTime.AddMonths(-1);
        int dayVisibleFromPreviousMonth = DateTime.DaysInMonth(previousMonthDateTime.Year, previousMonthDateTime.Month);

        for (int p = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek) - 1; p >= 0; --p)
        {
            previousMonthDateTime = 
                new DateTime(previousMonthDateTime.Year, previousMonthDateTime.Month, dayVisibleFromPreviousMonth, 0, 0, 0, DateTimeKind.Local);
            CalendarDayItem dayItem = dayItemsInCalendarPanel.GetChild(p).GetComponent<CalendarDayItem>();
            dayItem.UpdateDayItem(previousMonthDateTime, false, filteredRooms);
            dayVisibleFromPreviousMonth--;
        }
    }

    private int GetDaysVisibleFromPreviousMonth(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 0;
            case DayOfWeek.Tuesday: return 1;
            case DayOfWeek.Wednesday: return 2;
            case DayOfWeek.Thursday: return 3;
            case DayOfWeek.Friday: return 4;
            case DayOfWeek.Saturday: return 5;
            case DayOfWeek.Sunday: return 6;
        }

        return 0;
    }
}
