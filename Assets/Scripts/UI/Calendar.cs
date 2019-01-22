using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour
{
    [SerializeField]
    private Text monthName = null;
    [SerializeField]
    private Transform dayItemsInCalendarPanel = null;
    private DateTime selectedDateTime;
    private DateTime previousMonthDateTime;
    private DateTime nextMonthDateTime;
    private int selectedMonthIndex;
    private Dictionary<int, string> monthNamesDict = new Dictionary<int, string>()
                                                                {
                                                                    {1,"Ianuarie"},
                                                                    {2, "Februarie"},
                                                                    {3,"Martie"},
                                                                    {4,"Aprilie"},
                                                                    {5,"Mai"},
                                                                    {6,"Iunie"},
                                                                    {7,"Iulie"},
                                                                    {8,"August"},
                                                                    {9,"Septembrie"},
                                                                    {10,"Octombrie"},
                                                                    {11,"Noiembrie"},
                                                                    {12,"Decembrie"}
                                                                };
    // Start is called before the first frame update
    void Start()
    {
        selectedMonthIndex = 0;
        selectedDateTime = DateTime.Today;
        UpdateCalendar(selectedDateTime);
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

    private void UpdateCalendar(DateTime selectedDateTime)
    {
        DateTime firstDayOfMonthInSelectedDate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, 1, 0, 0, 0, DateTimeKind.Local);
        
        SetDayItemsForPreviousMonth(selectedDateTime, firstDayOfMonthInSelectedDate);
        
        SetDayItemsForCurrentMonth(selectedDateTime, firstDayOfMonthInSelectedDate);
        
        SetDayItemsForNextMonth(selectedDateTime, firstDayOfMonthInSelectedDate);
    }

    private void SetDayItemsForNextMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        nextMonthDateTime = selectedDateTime.AddMonths(1);
        int daysInSelectedMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int firstDayFromNextMonthIndex = daysInSelectedMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromNextMonth = 1;

        for (int dayItemIndex = firstDayFromNextMonthIndex; dayItemIndex < dayItemsInCalendarPanel.childCount; dayItemIndex++)
        {
            nextMonthDateTime =
                new DateTime(nextMonthDateTime.Year, nextMonthDateTime.Month, dayVisibleFromNextMonth, 0, 0, 0, DateTimeKind.Local);
            DayItem dayItem = dayItemsInCalendarPanel.GetChild(dayItemIndex).GetComponent<DayItem>();
            dayItem.UpdateDayItem(nextMonthDateTime, false);
            dayVisibleFromNextMonth++;
        }
    }

    private void SetDayItemsForCurrentMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        monthName.text = monthNamesDict[selectedDateTime.Month] + " " + selectedDateTime.Year;
        int daysInMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int daysVisibleInCurrentMonth = daysInMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromSelectedMonth = 1;

        for (int i = daysVisibleFromPreviousMonth; i < daysVisibleInCurrentMonth; i++)
        {
            selectedDateTime = 
                new DateTime(selectedDateTime.Year, selectedDateTime.Month, dayVisibleFromSelectedMonth, 0, 0, 0, DateTimeKind.Local);
            DayItem dayItem = dayItemsInCalendarPanel.GetChild(i).GetComponent<DayItem>();
            dayItem.UpdateDayItem(selectedDateTime, true);
            dayVisibleFromSelectedMonth++;
        }
    }

    private void SetDayItemsForPreviousMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        previousMonthDateTime = selectedDateTime.AddMonths(selectedMonthIndex - 1);
        int dayVisibleFromPreviousMonth = DateTime.DaysInMonth(previousMonthDateTime.Year, previousMonthDateTime.Month);

        for (int p = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek) - 1; p >= 0; --p)
        {
            previousMonthDateTime = 
                new DateTime(previousMonthDateTime.Year, previousMonthDateTime.Month, dayVisibleFromPreviousMonth, 0, 0, 0, DateTimeKind.Local);
            DayItem dayItem = dayItemsInCalendarPanel.GetChild(p).GetComponent<DayItem>();
            dayItem.UpdateDayItem(previousMonthDateTime, false);
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
