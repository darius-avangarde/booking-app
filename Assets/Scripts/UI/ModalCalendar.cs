using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalCalendar : MonoBehaviour
{
    public EasyTween easyTween;

    private Action ConfirmCallback;
    private Action CancelCallback;
    [SerializeField]
    private Text monthName = null;
    [SerializeField]
    private Transform modalDayItemsInCalendarPanel = null;
    private DateTime selectedDateTime;
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
        selectedDateTime = DateTime.Today;
        UpdateCalendar(selectedDateTime);
    }

    public void ShowTest() { easyTween.OpenCloseObjectAnimation(); }

    public void Show(Action confirmCallback, Action cancelCallback)
    {
        easyTween.OpenCloseObjectAnimation();
        ConfirmCallback = confirmCallback;
        CancelCallback = cancelCallback;
    }

    public void Confirm()
    {
        ConfirmCallback?.Invoke();

        ConfirmCallback = null;
        CancelCallback = null;

        easyTween.OpenCloseObjectAnimation();
    }

    public void Cancel()
    {
        CancelCallback?.Invoke();

        ConfirmCallback = null;
        CancelCallback = null;

        easyTween.OpenCloseObjectAnimation();
    }
   
    public void ShowPreviousMonth()
    {
        if (selectedDateTime > DateTime.Today)
        {
            selectedDateTime = selectedDateTime.AddMonths(-1);
            UpdateCalendar(selectedDateTime);
        }
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

        DateTime nextMonthDateTime = selectedDateTime.AddMonths(1);
        int daysInSelectedMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int firstDayFromNextMonthIndex = daysInSelectedMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromNextMonth = 1;

        for (int dayItemIndex = firstDayFromNextMonthIndex; dayItemIndex < modalDayItemsInCalendarPanel.childCount; dayItemIndex++)
        {
            nextMonthDateTime =
                new DateTime(nextMonthDateTime.Year, nextMonthDateTime.Month, dayVisibleFromNextMonth, 0, 0, 0, DateTimeKind.Local);
            ModalCalendarDayItem dayItem = modalDayItemsInCalendarPanel.GetChild(dayItemIndex).GetComponent<ModalCalendarDayItem>();
            dayItem.UpdateDayItem(nextMonthDateTime, true);
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
            ModalCalendarDayItem dayItem = modalDayItemsInCalendarPanel.GetChild(i).GetComponent<ModalCalendarDayItem>();
            dayItem.UpdateDayItem(selectedDateTime, selectedDateTime > DateTime.Today);
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
            ModalCalendarDayItem dayItem = modalDayItemsInCalendarPanel.GetChild(p).GetComponent<ModalCalendarDayItem>();
            dayItem.UpdateDayItem(previousMonthDateTime, selectedDateTime > DateTime.Today);
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
