using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Transform dayItemsContainer = null;

    private DateTime selectedDateTime;
    private List<IRoom> rooms;

    void Start()
    {
        SetGridLayoutGroupCellSize();
        AddButtonListeners();

        selectedDateTime = DateTime.Today;
    }

    public void SetRooms(List<IRoom> rooms)
    {
        this.rooms = rooms;
        UpdateCalendar(selectedDateTime);
    }

    private void SetGridLayoutGroupCellSize()
    {
        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
        float height = GetComponent<RectTransform>().rect.height;
        const float CELL_WIDTH = 150;
        const int NUMBER_OF_ROWS = 6;
        float spacing = gridLayoutGroup.spacing.y * (NUMBER_OF_ROWS - 1);
        Vector2 newSize = new Vector2(CELL_WIDTH, (height - spacing) / NUMBER_OF_ROWS);
        gridLayoutGroup.cellSize = newSize;
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

    private void AddButtonListeners()
    {
        foreach (Transform dayItem in dayItemsContainer)
        {
            dayItem.GetComponent<CalendarDayItem>().AddListener(OpenDayScreen);
        }
    }

    public void OpenDayScreen(DateTime dateTime)
    {
        dayScreen.GetComponent<DayScreen>().UpdateDayScreenInfo(dateTime);
        navigator.GoTo(dayScreen.GetComponent<NavScreen>());
    }

    private void UpdateCalendar(DateTime selectedDateTime)
    {
        DateTime firstDayOfMonthInSelectedDate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, 1, 0, 0, 0, DateTimeKind.Local);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);

        SetDayItemsForPreviousMonth(selectedDateTime, daysVisibleFromPreviousMonth);

        SetDayItemsForCurrentMonth(selectedDateTime, daysVisibleFromPreviousMonth);

        SetDayItemsForNextMonth(selectedDateTime, daysVisibleFromPreviousMonth);
    }

    private void SetDayItemsForNextMonth(DateTime selectedDateTime, int daysVisibleFromPreviousMonth)
    {
        DateTime nextMonthDateTime = selectedDateTime.AddMonths(1);
        int daysInSelectedMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int firstDayFromNextMonthIndex = daysInSelectedMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromNextMonth = 1;

        for (int dayItemIndex = firstDayFromNextMonthIndex; dayItemIndex < dayItemsContainer.childCount; dayItemIndex++)
        {
            nextMonthDateTime =
                new DateTime(nextMonthDateTime.Year, nextMonthDateTime.Month, dayVisibleFromNextMonth, 0, 0, 0, DateTimeKind.Local);
            CalendarDayItem dayItem = dayItemsContainer.GetChild(dayItemIndex).GetComponent<CalendarDayItem>();
            dayItem.UpdateDayItem(nextMonthDateTime, false, rooms, calendarScreen.GetReservedRoomsInCurrentDay(nextMonthDateTime));
            dayVisibleFromNextMonth++;
        }
    }

    private void SetDayItemsForCurrentMonth(DateTime selectedDateTime, int daysVisibleFromPreviousMonth)
    {
        monthName.text = Constants.MonthNamesDict[selectedDateTime.Month] + " " + selectedDateTime.Year;
        int daysInMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleInCurrentMonth = daysInMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromSelectedMonth = 1;

        for (int i = daysVisibleFromPreviousMonth; i < daysVisibleInCurrentMonth; i++)
        {
            selectedDateTime =
                new DateTime(selectedDateTime.Year, selectedDateTime.Month, dayVisibleFromSelectedMonth, 0, 0, 0, DateTimeKind.Local);
            CalendarDayItem dayItem = dayItemsContainer.GetChild(i).GetComponent<CalendarDayItem>();
            dayItem.UpdateDayItem(selectedDateTime, true, rooms, calendarScreen.GetReservedRoomsInCurrentDay(selectedDateTime));
            dayVisibleFromSelectedMonth++;
        }
    }

    private void SetDayItemsForPreviousMonth(DateTime selectedDateTime, int daysVisibleFromPreviousMonth)
    {
        DateTime previousMonthDateTime = selectedDateTime.AddMonths(-1);
        int dayCount = DateTime.DaysInMonth(previousMonthDateTime.Year, previousMonthDateTime.Month);

        for (int p = daysVisibleFromPreviousMonth - 1; p >= 0; --p)
        {
            previousMonthDateTime =
                new DateTime(previousMonthDateTime.Year, previousMonthDateTime.Month, dayCount, 0, 0, 0, DateTimeKind.Local);
            CalendarDayItem dayItem = dayItemsContainer.GetChild(p).GetComponent<CalendarDayItem>();
            dayItem.UpdateDayItem(previousMonthDateTime, false, rooms, calendarScreen.GetReservedRoomsInCurrentDay(previousMonthDateTime));
            dayCount--;
        }
    }

    private int GetDaysVisibleFromPreviousMonth(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday:
                return 0;
            case DayOfWeek.Tuesday:
                return 1;
            case DayOfWeek.Wednesday:
                return 2;
            case DayOfWeek.Thursday:
                return 3;
            case DayOfWeek.Friday:
                return 4;
            case DayOfWeek.Saturday:
                return 5;
            case DayOfWeek.Sunday:
                return 6;
            default:
                return 0;
        }
    }
}
