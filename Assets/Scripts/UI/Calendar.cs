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
    private DateTime dateTime;
    private DateTime previousMonthDateTime;
    private DateTime nextMonthDateTime;
    private int thisMonthIndex;
    private string[] monthNames = {"Ianuarie", "Februarie", "Martie", "Aprilie", "Mai", "Iunie",
        "Iulie", "August", "Septembrie", "Octombrie", "Noiembrie", "Decembrie"};

    // Start is called before the first frame update
    void Start()
    {
        thisMonthIndex = 0;
        dateTime = DateTime.Today;
        previousMonthDateTime = dateTime.AddMonths(-1);
        nextMonthDateTime = dateTime.AddMonths(1);
        UpdateCalendar(dateTime);
    }

    public void ShowPreviousMonth()
    {
        --thisMonthIndex;
        previousMonthDateTime = dateTime.AddMonths(thisMonthIndex - 1);
        nextMonthDateTime = dateTime.AddMonths(thisMonthIndex + 1);
        UpdateCalendar(dateTime.AddMonths(thisMonthIndex));
    }

    public void ShowNextMonth()
    {
        ++thisMonthIndex;
        previousMonthDateTime = dateTime.AddMonths(thisMonthIndex - 1);
        nextMonthDateTime = dateTime.AddMonths(thisMonthIndex + 1);
        UpdateCalendar(dateTime.AddMonths(thisMonthIndex));
    }

    private void UpdateCalendar(DateTime dateTime)
    {
        for (int i = 0; i < dayItemsInCalendarPanel.childCount; i++)
        {
            dayItemsInCalendarPanel.GetChild(i).GetComponent<DayItem>().Clear();
        }

        int day = 1;
        DateTime firstDay = new DateTime(dateTime.Year, dateTime.Month, day, 0, 0, 0, DateTimeKind.Local);
        
        int daysInPreviousMonth = DateTime.DaysInMonth(previousMonthDateTime.Year, previousMonthDateTime.Month);
        for (int p = GetDay(firstDay.DayOfWeek) - 1; p >= 0; --p)
        {
            previousMonthDateTime = new DateTime(previousMonthDateTime.Year, previousMonthDateTime.Month, daysInPreviousMonth, 0, 0, 0, DateTimeKind.Local);
            dayItemsInCalendarPanel.GetChild(p).GetComponent<DayItem>().UpdateDayItem(previousMonthDateTime, false);
            daysInPreviousMonth--;
        }

        //set and show current month days in UI
        monthName.text = GetNameOfMonth(dateTime.Month) + " " + dateTime.Year;
        for (int i = GetDay(firstDay.DayOfWeek); i < DateTime.DaysInMonth(dateTime.Year, dateTime.Month) + GetDay(firstDay.DayOfWeek); i++)
        {
            dateTime = new DateTime(dateTime.Year, dateTime.Month, day, 0, 0, 0, DateTimeKind.Local);
            dayItemsInCalendarPanel.GetChild(i).GetComponent<DayItem>().UpdateDayItem(dateTime, true);
            day++;
        }

        //set in current UICalendar next month days
        int firstDayInNextMonth = 1;
        DateTime firstDayFromNextMonth = new DateTime(nextMonthDateTime.Year, nextMonthDateTime.Month, firstDayInNextMonth, 0, 0, 0, DateTimeKind.Local);
        for (int n = DateTime.DaysInMonth(dateTime.Year, dateTime.Month) + GetDay(firstDay.DayOfWeek); n < dayItemsInCalendarPanel.childCount; n++)
        {
            nextMonthDateTime = new DateTime(nextMonthDateTime.Year, nextMonthDateTime.Month, firstDayInNextMonth, 0, 0, 0, DateTimeKind.Local);
            print(nextMonthDateTime);
            dayItemsInCalendarPanel.GetChild(n).GetComponent<DayItem>().UpdateDayItem(nextMonthDateTime, false);
            firstDayInNextMonth++;
        }
    }

    private int GetDay(DayOfWeek day)
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

    private string GetNameOfMonth(int monthIndex)
    {
        switch (monthIndex)
        {
            case 1: return monthNames[0];
            case 2: return monthNames[1];
            case 3: return monthNames[2];
            case 4: return monthNames[3];
            case 5: return monthNames[4];
            case 6: return monthNames[5];
            case 7: return monthNames[6];
            case 8: return monthNames[7];
            case 9: return monthNames[8];
            case 10: return monthNames[9];
            case 11: return monthNames[10];
            case 12: return monthNames[11];
        }
        return "";
    }
}
