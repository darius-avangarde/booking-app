using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModalMonthPage : MonoBehaviour
{
    public RectTransform Rect => rectTransform;

    [Space]
    [SerializeField]
    private Transform rowParent = null;
    [SerializeField]
    private GameObject rowPrefab = null;
    [SerializeField]
    private GameObject dayObjectPrefab = null;
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private ModalCalendarNew calendar;

    private List<ModalDayObject> dayObjects = new List<ModalDayObject>();
    private DateTime currentDate;


    public void UpdatePage(DateTime selectedDateTime)
    {
        DateTime firstDayOfMonthInSelectedDate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, 1, 0, 0, 0, DateTimeKind.Local);

        currentDate = selectedDateTime.Date;

        SetDayItemsForPreviousMonth(selectedDateTime, firstDayOfMonthInSelectedDate);

        SetDayItemsForCurrentMonth(selectedDateTime, firstDayOfMonthInSelectedDate);

        SetDayItemsForNextMonth(selectedDateTime, firstDayOfMonthInSelectedDate);

        UpdateCurentReservationPeriod();

        if(calendar.ShowSelection)
        {
            UpdatePeriodSelection();
        }
    }

    public void CreateDayItems()
    {
        for (int r = 0; r < 6; r++)//rows
        {
            Transform row = Instantiate(rowPrefab, rowParent).transform;
            for (int i = 0; i < 7; i++) //days
            {
                ModalDayObject m = Instantiate(dayObjectPrefab,row).GetComponent<ModalDayObject>();
                m.SetListener(calendar.HandleClickAction);
                dayObjects.Add(m);
            }
        }
    }

    private void SetDayItemsForNextMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        DateTime nextMonthDateTime = selectedDateTime.AddMonths(1);
        int daysInSelectedMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = calendar.GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int firstDayFromNextMonthIndex = daysInSelectedMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromNextMonth = 1;

        for (int d = firstDayFromNextMonthIndex; d < dayObjects.Count; d++)
        {
            nextMonthDateTime = new DateTime(nextMonthDateTime.Year, nextMonthDateTime.Month, dayVisibleFromNextMonth, 0, 0, 0, DateTimeKind.Local);
            SetReservationType(dayObjects[d], nextMonthDateTime);
            dayVisibleFromNextMonth++;
        }
    }

    private void SetDayItemsForCurrentMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        //monthName.text = Constants.MonthNamesDict[selectedDateTime.Month] + " " + selectedDateTime.Year;
        int daysInMonth = DateTime.DaysInMonth(selectedDateTime.Year, selectedDateTime.Month);
        int daysVisibleFromPreviousMonth = calendar.GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek);
        int daysVisibleInCurrentMonth = daysInMonth + daysVisibleFromPreviousMonth;
        int dayVisibleFromSelectedMonth = 1;

        for (int i = daysVisibleFromPreviousMonth; i < daysVisibleInCurrentMonth; i++)
        {
            selectedDateTime = new DateTime(selectedDateTime.Year, selectedDateTime.Month, dayVisibleFromSelectedMonth, 0, 0, 0, DateTimeKind.Local);

            SetReservationType(dayObjects[i], selectedDateTime);
            dayVisibleFromSelectedMonth++;
        }
    }

    private void SetDayItemsForPreviousMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        DateTime previousMonthDateTime = selectedDateTime.AddMonths(-1);
        int dayVisibleFromPreviousMonth = DateTime.DaysInMonth(previousMonthDateTime.Year, previousMonthDateTime.Month);

        for (int p = calendar.GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek) - 1; p >= 0; --p)
        {
            previousMonthDateTime = new DateTime(previousMonthDateTime.Year, previousMonthDateTime.Month, dayVisibleFromPreviousMonth, 0, 0, 0, DateTimeKind.Local);
            SetReservationType(dayObjects[p], previousMonthDateTime);
            dayVisibleFromPreviousMonth--;
        }
    }

    private void SetReservationType(ModalDayObject dayObj, DateTime dateTime)
    {
        dayObj.UpdateDayObject(dateTime);

        string currentReservationID = (calendar.CurrentReservation != null) ? calendar.CurrentReservation.ID : Constants.defaultCustomerName;

        dayObj.UpdateDayColors(calendar.AvailableColor, calendar.AvailableColor, calendar.AvailableColor);
        dayObj.IsReserved = false;
        dayObj.IsEnd = false;
        dayObj.IsStart = false;

        if(dateTime.Date < DateTime.Today.Date)
        {
            dayObj.UpdateDayColors(calendar.PastColor, calendar.PastColor, calendar.PastColor);
            dayObj.IsReserved = true;
            return;
        }

        if(dateTime.Date.Month != currentDate.Month)
        {
            dayObj.UpdateDayColors(calendar.NotMonthColor, calendar.NotMonthColor, calendar.NotMonthColor);
        }

        if(calendar.RoomReservationList == null || calendar.RoomReservationList.Count == 0)
        {
            return;
        }

        if(calendar.RoomReservationList.Any(r => r.Period.Start.Date == dateTime.Date))
        {
            dayObj.UpdateDayColors(null, calendar.UnavailableColor, null);
            dayObj.IsStart = true;
        }

        if(calendar.RoomReservationList.Any(r => r.Period.End.Date == dateTime.Date))
        {
            dayObj.UpdateDayColors(null, null, calendar.UnavailableColor);
            dayObj.IsEnd = true;
        }

        if(calendar.RoomReservationList.Any(r => dateTime.Date < r.Period.End.Date && dateTime.Date > r.Period.Start.Date))
        {
            dayObj.UpdateDayColors(calendar.UnavailableColor, calendar.UnavailableColor, calendar.UnavailableColor);
            dayObj.IsReserved = true;
        }
    }

    private void UpdatePeriodSelection()
    {
        if(calendar.SelectedStart.Date == calendar.SelectedEnd.Date)
        {
            if(dayObjects.Any(d => d.ObjDate.Date == calendar.SelectedStart.Date))
            {
                dayObjects.Find(d => d.ObjDate.Date == calendar.SelectedStart.Date).UpdateDayColors(null,calendar.SelectedColor, null);
            }
            return;
        }

        for (int d = 0; d < dayObjects.Count; d++)
        {
            if(dayObjects[d].ObjDate.Date > calendar.SelectedStart.Date && dayObjects[d].ObjDate.Date < calendar.SelectedEnd.Date)
            {
                dayObjects[d].UpdateDayColors(calendar.SelectedColor, calendar.SelectedColor, calendar.SelectedColor);
            }
            else
            {
                if(dayObjects[d].ObjDate == calendar.SelectedStart)
                {
                    dayObjects[d].UpdateDayColors(null,calendar.SelectedColor, null);
                }

                if(dayObjects[d].ObjDate == calendar.SelectedEnd)
                {
                    dayObjects[d].UpdateDayColors(null, null, calendar.SelectedColor);
                }
            }
        }
    }

    private void UpdateCurentReservationPeriod()
    {
        if(calendar.CurrentReservation != null)
        {
            for (int d = 0; d < dayObjects.Count; d++)
            {

                if(dayObjects[d].ObjDate.Date < DateTime.Today.Date)
                {
                    dayObjects[d].UpdateDayColors(calendar.PastColor, calendar.PastColor, calendar.PastColor);
                    dayObjects[d].IsReserved = true;
                }
                else
                {
                    if(dayObjects[d].ObjDate.Date > calendar.CurrentReservation.Period.Start.Date && dayObjects[d].ObjDate.Date < calendar.CurrentReservation.Period.End.Date)
                    {
                        dayObjects[d].UpdateDayColors(OverlapCol(dayObjects[d].IsReserved || dayObjects[d].IsStart || dayObjects[d].IsEnd), OverlapCol(dayObjects[d].IsStart || dayObjects[d].IsReserved), OverlapCol(dayObjects[d].IsEnd || dayObjects[d].IsReserved));
                    }
                    else
                    {
                        if(dayObjects[d].ObjDate.Date == calendar.CurrentReservation.Period.Start.Date)
                        {
                            dayObjects[d].UpdateDayColors(null, OverlapCol(dayObjects[d].IsStart || dayObjects[d].IsReserved), null);
                        }

                        if(dayObjects[d].ObjDate.Date == calendar.CurrentReservation.Period.End.Date)
                        {
                            dayObjects[d].UpdateDayColors(null, null,  OverlapCol(dayObjects[d].IsEnd || dayObjects[d].IsReserved));
                        }
                    }
                }
            }
        }
    }

    private Color OverlapCol(bool isOverlap)
    {
        return isOverlap ? calendar.OverlapColor : calendar.CurrentColor;
    }
}
