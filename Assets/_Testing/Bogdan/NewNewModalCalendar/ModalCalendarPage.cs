using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModalCalendarPage : MonoBehaviour
{
    public RectTransform Rect => thisRect;

    [SerializeField]
    private RectTransform thisRect;

    [SerializeField]
    private GameObject rowPrefab = null;
    [SerializeField]
    private GameObject dayObjectPrefab = null;

    private List<ModalCalendarDayObject> dayObjects = new List<ModalCalendarDayObject>();
    private UnityAction<DateTime,bool> dateTapAction;
    private DateTime? selectedDate;

    public void UpdatePage(DateTime focalDateTime)
    {
        DateTime firstDayOfMonthInSelectedDate = new DateTime(focalDateTime.Year, focalDateTime.Month, 1, 0, 0, 0, DateTimeKind.Local);

        SetDayItemsForPreviousMonth(focalDateTime, firstDayOfMonthInSelectedDate);

        SetDayItemsForCurrentMonth(focalDateTime, firstDayOfMonthInSelectedDate);

        SetDayItemsForNextMonth(focalDateTime, firstDayOfMonthInSelectedDate);

        UpdateSelections(selectedDate);
    }

    public void UpdateSelections(DateTime? selectedDateTime)
    {
        selectedDate = selectedDateTime;

        foreach (ModalCalendarDayObject day in dayObjects)
        {
            if(selectedDate != null && selectedDate.Value.Date == day.ObjDate.Date)
            {
                day.UpdateSpriteAndColor(Placeholder_ThemeManager.Instance.selectedDaySprite, Placeholder_ThemeManager.Instance.currentReservationColor);
            }
            else if(day.ObjDate.Date == DateTime.Today.Date)
            {
                day.UpdateSpriteAndColor(Placeholder_ThemeManager.Instance.currentDaySprite, Placeholder_ThemeManager.Instance.CalendarCurrentColor);
            }
            else
            {
                day.UpdateSpriteAndColor(Placeholder_ThemeManager.Instance.selectedDaySprite, Color.clear);
            }
        }
    }

    public void CreateDayItems(UnityAction<DateTime,bool> tapAction)
    {
        dateTapAction = tapAction;
        for (int r = 0; r < 6; r++)//rows
        {
            Transform row = Instantiate(rowPrefab, transform).transform;
            for (int i = 0; i < 7; i++) //days
            {
                dayObjects.Add(Instantiate(dayObjectPrefab,row).GetComponent<ModalCalendarDayObject>());
            }
        }
    }

    private void SetDayItemsForNextMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        DateTime nextMonthDateTime = selectedDateTime.AddMonths(1);
        int daysInSelectedMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int firstDayFromNextMonthIndex = daysInSelectedMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromNextMonth = 1;

        for (int d = firstDayFromNextMonthIndex; d < dayObjects.Count; d++)
        {
            nextMonthDateTime = new DateTime(nextMonthDateTime.Year, nextMonthDateTime.Month, dayVisibleFromNextMonth, 0, 0, 0, DateTimeKind.Local);
            dayObjects[d].UpdateDayObject();
            dayVisibleFromNextMonth++;
        }
    }

    private void SetDayItemsForCurrentMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        //monthName.text = Constants.MonthNamesDict[selectedDateTime.Month] + " " + selectedDateTime.Year;
        int daysInMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int daysVisibleInCurrentMonth = daysInMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromSelectedMonth = 1;

        for (int d = daysVisibleFromPreviousMonth; d < daysVisibleInCurrentMonth; d++)
        {
            selectedDateTime = new DateTime(selectedDateTime.Year, selectedDateTime.Month, dayVisibleFromSelectedMonth, 0, 0, 0, DateTimeKind.Local);
            dayObjects[d].UpdateDayObject(selectedDateTime.Date, dateTapAction);
            dayVisibleFromSelectedMonth++;
        }
    }

    private void SetDayItemsForPreviousMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        DateTime previousMonthDateTime = selectedDateTime.AddMonths(-1);
        int dayVisibleFromPreviousMonth = DateTime.DaysInMonth(previousMonthDateTime.Year, previousMonthDateTime.Month);

        for (int d = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek) - 1; d >= 0; --d)
        {
            previousMonthDateTime = new DateTime(previousMonthDateTime.Year, previousMonthDateTime.Month, dayVisibleFromPreviousMonth, 0, 0, 0, DateTimeKind.Local);
            dayObjects[d].UpdateDayObject();
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
