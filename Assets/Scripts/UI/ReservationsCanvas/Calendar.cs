using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

// TODO: Calendar doesn't really need to know about CalendarScreen
// To do its job, all Calendar needs is data that can be set by CalendarScreen directly
// We can consider CalendarScreen to be "responsible" of Calendar and all the UI components it contains. Since it
// is aware of when the user navigates to it or from it (through NavScreen.Showing or NavScreen.Hidden) it is a good place
// to place responsibility for the data used to display it and all its components
// Let's discuss this when we refactor this code.
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
    // TODO: this is a Transform but it's called dayItems, the name should be singular and reflect what the transform is
    private Transform dayItemsInCalendarPanel = null;
    private DateTime selectedDateTime;
    private List<IRoom> filteredRoomList;

    void Start()
    {
        // TODO: the room data should be set from CalendarScreen
        // moving responsability for initialization from Calendar to CalendarScreen allows CalendarScreen
        // to keep Calendar updated, since it is responsible for the filter and can subscribe to NavScreen.Showing
        filteredRoomList = calendarScreen.GetRoomsInFilteredRoomsList();
        InstantiateCalendarDayItems();
        selectedDateTime = DateTime.Today;
        UpdateCalendar(selectedDateTime);
    }

    // TODO: this would be better called by CalendarScreen with the necessary data
    // we could probably get rid of this method and simply call UpdateCalendar, maybe with an optional data parameter
    public void UpdateCalendarAfterFiltering()
    {
        filteredRoomList = calendarScreen.GetFilteredRooms();
        UpdateCalendar(selectedDateTime);
    }

    public void UpdateItemsStatusOnCalendarAndDayScreenOnReservationChange()
    {
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

    // TODO: we're not really instantiating anything in this method
    private void InstantiateCalendarDayItems()
    {
        // TODO: foreach would be slightly clener here
        for (int dayItemIndex = 0; dayItemIndex < dayItemsInCalendarPanel.childCount; dayItemIndex++)
        {
            CalendarDayItem dayItem = dayItemsInCalendarPanel.GetChild(dayItemIndex).GetComponent<CalendarDayItem>();
            dayItem.Initialize(OpenDayScreen);
        }
    }

    private void OpenDayScreen(DateTime dateTime)
    {
        dayScreen.GetComponent<DayScreen>().UpdateDayScreenInfo(dateTime);
        navigator.GoTo(dayScreen.GetComponent<NavScreen>());
    }

    private void UpdateCalendar(DateTime selectedDateTime)
    {
        // TODO: a tip on naming variables: details that are obvious from context don't need to be specified in the name
        // in this case ask yourself: could firstDayOfMonthInSelectedDate be the first day of month in any other date?
        // if no then it could simple be firstDayOfMonth since "in selected date" is understood from the context
        DateTime firstDayOfMonthInSelectedDate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, 1, 0, 0, 0, DateTimeKind.Local);

        // TODO: we're passing firstDayOfMonthInSelectedDate to these three methods and we use it in the same way in all three (GetDaysVisibleFromPreviousMonth)
        // we can simply move that calculation here and pass the result instead (daysVisibleFromPreviousMonth)
        SetDayItemsForPreviousMonth(selectedDateTime, firstDayOfMonthInSelectedDate);

        SetDayItemsForCurrentMonth(selectedDateTime, firstDayOfMonthInSelectedDate);

        SetDayItemsForNextMonth(selectedDateTime, firstDayOfMonthInSelectedDate);
    }

    // TODO: since we now have three methods for setting DayItems some local variables don't need to be specifically named (e.g. firstDayFromNextMonthIndex)
    // since it's understood that it's "from next month" from the the fact that it's in the SetDayItemsForNextMonth method
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
            dayItem.UpdateDayItem(nextMonthDateTime, false, filteredRoomList, calendarScreen.GetReservedRoomsInCurrentDay(nextMonthDateTime));
            dayVisibleFromNextMonth++;
        }
    }

    private void SetDayItemsForCurrentMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        monthName.text = Constants.MonthNamesDict[selectedDateTime.Month] + " " + selectedDateTime.Year;
        int daysInMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int daysVisibleInCurrentMonth = daysInMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromSelectedMonth = 1;

        for (int i = daysVisibleFromPreviousMonth; i < daysVisibleInCurrentMonth; i++)
        {
            selectedDateTime =
                new DateTime(selectedDateTime.Year, selectedDateTime.Month, dayVisibleFromSelectedMonth, 0, 0, 0, DateTimeKind.Local);
            CalendarDayItem dayItem = dayItemsInCalendarPanel.GetChild(i).GetComponent<CalendarDayItem>();
            dayItem.UpdateDayItem(selectedDateTime, true, filteredRoomList, calendarScreen.GetReservedRoomsInCurrentDay(selectedDateTime));
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
            dayItem.UpdateDayItem(previousMonthDateTime, false, filteredRoomList, calendarScreen.GetReservedRoomsInCurrentDay(previousMonthDateTime));
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

        // TODO: instead of returning 0 outside of the switch we can use the switch's default case to return 0
        return 0;
    }
}
