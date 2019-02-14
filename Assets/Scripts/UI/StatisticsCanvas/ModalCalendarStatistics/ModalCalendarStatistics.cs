using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalCalendarStatistics : MonoBehaviour
{
    [SerializeField]
    private EasyTween easyTween = null;
    [SerializeField]
    private Transform modalItemsCalendarPanel = null;
    [SerializeField]
    private Text monthName = null;
    [SerializeField]
    private Transform dayItemsInCalendarPanel = null;
    private DateTime selectedDateTime = DateTime.Today;
    private DateTime startDateTime;
    private DateTime endDateTime;
    private bool isSetStartDay = false;

    private Action<DateTime, DateTime> DoneCallback;

    void Start()
    {
        InstantiateModalCalendarDayItems();
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

    public void Show(Action<DateTime, DateTime> doneCallback)
    {
        easyTween.OpenCloseObjectAnimation();
        DoneCallback = doneCallback;
    }

    private void CloseModalCalendar()
    {
        DoneCallback?.Invoke(startDateTime, endDateTime);
        DoneCallback = null;
        easyTween.OpenCloseObjectAnimation();
    }

    private void SetStatisticsPeriod(DateTime dateTime, Image modalItemImage)
    {
        if (!isSetStartDay)
        {
            startDateTime = dateTime;
            modalItemImage.color = Constants.selectedItemColor;
            isSetStartDay = true;
        }
        else if (dateTime > startDateTime)
        {
            endDateTime = dateTime;
            modalItemImage.color = Constants.selectedItemColor;
            CloseModalCalendar();
            isSetStartDay = false;
        }
    }

    private void InstantiateModalCalendarDayItems()
    {
        for (int dayItemIndex = 0; dayItemIndex < modalItemsCalendarPanel.childCount; dayItemIndex++)
        {
            ModalCalendarStatisticsDayItem dayItem = modalItemsCalendarPanel.GetChild(dayItemIndex).GetComponent<ModalCalendarStatisticsDayItem>();
            dayItem.Initialize(SetStatisticsPeriod);
        }
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
            ModalCalendarStatisticsDayItem dayItem = dayItemsInCalendarPanel.GetChild(dayItemIndex).GetComponent<ModalCalendarStatisticsDayItem>();
            dayItem.UpdateDayItem(nextMonthDateTime);
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
            ModalCalendarStatisticsDayItem dayItem = dayItemsInCalendarPanel.GetChild(i).GetComponent<ModalCalendarStatisticsDayItem>();
            dayItem.UpdateDayItem(selectedDateTime);
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
            ModalCalendarStatisticsDayItem dayItem = dayItemsInCalendarPanel.GetChild(p).GetComponent<ModalCalendarStatisticsDayItem>();
            dayItem.UpdateDayItem(previousMonthDateTime);
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
