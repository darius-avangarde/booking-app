using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ModalCalendarNew : MonoBehaviour, IClosable
{
    [SerializeField]
    private EasyTween easyTween = null;
    [SerializeField]
    private Text monthName = null;
    [SerializeField]
    private Text selectionText;
    [SerializeField]
    private Text selectionDayCountText;
    [SerializeField]
    private Button confirmButton;

    [Space]
    [SerializeField]
    private Transform rowParent;
    [SerializeField]
    private GameObject rowPrefab;
    [SerializeField]
    private GameObject dayObjectPrefab;

    [Space]
    #region Colors
    [SerializeField]
    private Color availableColor;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color unavailableColor;
    [SerializeField]
    private Color currentColor;
    [SerializeField]
    private Color pastColor;
    #endregion

    private Action<DateTime, DateTime> DoneCallback;
    private DateTime focusDateTime = DateTime.Today;
    private IReservation currentReservation;
    private List<IReservation> roomReservationList = new List<IReservation>();
    private List<ModalDayObject> dayObjects = new List<ModalDayObject>();
    private bool isSelectingEnd = false;
    private bool showSelection = false;
    private bool allowSigleDate = false;
    private DateTime selectedStart;
    private DateTime selectedEnd;

    private void Start()
    {
        CreateDayItems();
    }

    ///<summary>
    ///Opens the calendar in selection mode focused on the given datetime.
    ///<para>Done callback returns either the selected datetime and the day after if only one date is selected, or the selected start and end date</para>
    ///</summary>
    internal void OpenCallendar(DateTime focusedDay, Action<DateTime, DateTime> doneCallback)
    {
        focusDateTime = focusedDay;
        DoneCallback = doneCallback;
        allowSigleDate = true;
        selectionDayCountText.text = string.Empty;
        Show(focusDateTime, null, null, doneCallback);
    }

    ///<summary>
    ///Opens the calendar in reservation edit mode for the given IReservation, and room reservations list.
    ///<para>Done callback returns the selected start and end date</para>
    ///</summary>
    internal void OpenCallendar(IReservation r, List<IReservation> roomReservations, Action<DateTime, DateTime> doneCallback)
    {
        focusDateTime = (r != null) ? r.Period.Start : DateTime.Today;
        allowSigleDate = false;
        UpdateDayCountText();
        Show(focusDateTime, r, roomReservations, doneCallback);
    }

    public void ShowPreviousMonth()
    {
        if (focusDateTime.Date > DateTime.Today.Date)
        {
            focusDateTime = focusDateTime.AddMonths(-1);
            UpdateCalendar(focusDateTime);
        }
    }

    public void ShowNextMonth()
    {
        focusDateTime = focusDateTime.AddMonths(1);
        UpdateCalendar(focusDateTime);
    }

    public void Close()
    {
        CloseModalCalendar(true);
    }

    public void CancelSelectedDateTimeOnModalCalendar()
    {
        CloseModalCalendar(false);
    }


    private void CreateDayItems()
    {
        for (int r = 0; r < 6; r++)//rows
        {
            Transform row = Instantiate(rowPrefab, rowParent).transform;
            for (int i = 0; i < 7; i++) //days
            {
                ModalDayObject m = Instantiate(dayObjectPrefab,row).GetComponent<ModalDayObject>();
                m.SetListener(HandleClickAction);
                dayObjects.Add(m);
            }
        }
    }

    private void Show(DateTime initialDateTime, IReservation reservation, List<IReservation> reservationList, Action<DateTime, DateTime> doneCallback)
    {
        focusDateTime = initialDateTime;
        currentReservation = reservation;
        roomReservationList = reservationList;
        easyTween.OpenCloseObjectAnimation();
        InputManager.CurrentlyOpenClosable = this;
        DoneCallback = doneCallback;

        if(currentReservation != null)
        {
            selectionText.text = currentReservation.Period.Start.ToString(Constants.DateTimePrintFormat) + Constants.AndDelimiter + currentReservation.Period.End.ToString(Constants.DateTimePrintFormat);
            confirmButton.interactable = true;
        }
        else
        {
            selectionText.text = string.Empty;
            confirmButton.interactable = false;
        }

        UpdateCalendar(focusDateTime);
    }

    private void HandleClickAction(ModalDayObject dayObj)
    {
        if(!dayObj.IsReserved && !dayObj.IsStart && !isSelectingEnd)
        {
            selectedStart = dayObj.ObjDate.Date;
            selectedEnd = dayObj.ObjDate.Date;
            isSelectingEnd = true;
            showSelection = true;
            UpdateCalendar(focusDateTime);
            selectionText.text = selectedStart.ToString(Constants.DateTimePrintFormat);
            confirmButton.interactable = allowSigleDate;
        }
        else if(!dayObj.IsReserved && isSelectingEnd)
        {
            selectedEnd = dayObj.ObjDate.Date;
            if(selectedStart.Date > selectedEnd.Date)
            {
                selectedEnd = selectedStart;
                selectedStart = dayObj.ObjDate;
            }

            if(!OverlapsOtherReservation(selectedStart, selectedEnd) && selectedStart != selectedEnd)
            {
                showSelection = true;
                selectionText.text = selectedStart.ToString(Constants.DateTimePrintFormat) + Constants.AndDelimiter + selectedEnd.ToString(Constants.DateTimePrintFormat);
                confirmButton.interactable = true;
            }
            else
            {
                selectedEnd = default;
                selectedStart = default;
                selectionText.text = string.Empty;
                confirmButton.interactable = false;
            }
            UpdateCalendar(focusDateTime);
            isSelectingEnd = false;
        }
        else
        {
            showSelection = false;
            isSelectingEnd = false;
            UpdateCalendar(focusDateTime);
            selectionText.text = string.Empty;
            confirmButton.interactable = false;
        }
        UpdateDayCountText();
    }

    private void CloseModalCalendar(bool doCallback)
    {
        if(selectedEnd.Date == selectedStart.Date)
        {
            selectedEnd = selectedEnd.AddDays(1);
        }

        if(doCallback)
        {
            DoneCallback?.Invoke(selectedStart.Date, selectedEnd.Date);
        }
        DoneCallback = null;
        easyTween.OpenCloseObjectAnimation();
        InputManager.CurrentlyOpenClosable = null;
        showSelection = false;
        isSelectingEnd = false;
        allowSigleDate = false;
        currentReservation = null;
        roomReservationList = new List<IReservation>();
    }

    private void UpdateCalendar(DateTime selectedDateTime)
    {
        DateTime firstDayOfMonthInSelectedDate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, 1, 0, 0, 0, DateTimeKind.Local);

        SetDayItemsForPreviousMonth(selectedDateTime, firstDayOfMonthInSelectedDate);

        SetDayItemsForCurrentMonth(selectedDateTime, firstDayOfMonthInSelectedDate);

        SetDayItemsForNextMonth(selectedDateTime, firstDayOfMonthInSelectedDate);

        UpdateCurentReservationPeriod();

        if(showSelection)
        {
            UpdatePeriodSelection();
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
            SetReservationType(dayObjects[d], nextMonthDateTime);
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
            selectedDateTime = new DateTime(selectedDateTime.Year, selectedDateTime.Month, dayVisibleFromSelectedMonth, 0, 0, 0, DateTimeKind.Local);

            SetReservationType(dayObjects[i], selectedDateTime);
            dayVisibleFromSelectedMonth++;
        }
    }

    private void SetDayItemsForPreviousMonth(DateTime selectedDateTime, DateTime firstDayOfMonthInSelectedDate)
    {
        DateTime previousMonthDateTime = selectedDateTime.AddMonths(-1);
        int dayVisibleFromPreviousMonth = DateTime.DaysInMonth(previousMonthDateTime.Year, previousMonthDateTime.Month);

        for (int p = GetDaysVisibleFromPreviousMonth(firstDayOfMonthInSelectedDate.DayOfWeek) - 1; p >= 0; --p)
        {
            previousMonthDateTime = new DateTime(previousMonthDateTime.Year, previousMonthDateTime.Month, dayVisibleFromPreviousMonth, 0, 0, 0, DateTimeKind.Local);
            SetReservationType(dayObjects[p], previousMonthDateTime);
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

    private void SetReservationType(ModalDayObject dayObj, DateTime dateTime)
    {
        dayObj.UpdateDayObject(dateTime);

        string currentReservationID = (currentReservation != null) ? currentReservation.ID : Constants.defaultCustomerName;

        dayObj.UpdateDayColors(availableColor, availableColor, availableColor);
        dayObj.IsReserved = false;
        dayObj.IsEnd = false;
        dayObj.IsStart = false;

        if(dateTime.Date < DateTime.Today.Date)
        {
            dayObj.UpdateDayColors(pastColor, pastColor, pastColor);
            dayObj.IsReserved = true;
            return;
        }

        if(roomReservationList == null || roomReservationList.Count == 0)
        {
            return;
        }

        if(roomReservationList.Any(r => r.Period.Start.Date == dateTime.Date))
        {
            dayObj.UpdateDayColors(null, unavailableColor, null);
            dayObj.IsStart = true;
        }

        if(roomReservationList.Any(r => r.Period.End.Date == dateTime.Date))
        {
            dayObj.UpdateDayColors(null, null, unavailableColor);
            dayObj.IsEnd = true;
        }

        if(roomReservationList.Any(r => dateTime.Date < r.Period.End.Date && dateTime.Date > r.Period.Start.Date))
        {
            dayObj.UpdateDayColors(unavailableColor, unavailableColor, unavailableColor);
            dayObj.IsReserved = true;
        }
    }

    private bool OverlapsOtherReservation(DateTime start, DateTime end)
    {
        if(roomReservationList == null || roomReservationList.Count == 0)
        {
            return false;
        }

        return roomReservationList
            .Any(r => ((currentReservation != null) ? r.ID != currentReservation.ID : r.ID != Constants.defaultCustomerName)
            && ((start.Date > r.Period.Start && start.Date < r.Period.End.Date) //after start and before end
            || r.Period.Start.Date > start.Date && r.Period.End.Date < end.Date
            || r.Period.Start.Date == selectedStart.Date || r.Period.End.Date == selectedEnd.Date
            ));
    }

    private void UpdatePeriodSelection()
    {
        if(selectedStart.Date == selectedEnd.Date)
        {
            if(dayObjects.Any(d => d.ObjDate.Date == selectedStart.Date))
            {
                dayObjects.Find(d => d.ObjDate.Date == selectedStart.Date).UpdateDayColors(null,selectedColor, null);
            }
            return;
        }

        for (int d = 0; d < dayObjects.Count; d++)
        {
            if(dayObjects[d].ObjDate.Date > selectedStart.Date && dayObjects[d].ObjDate.Date < selectedEnd.Date)
            {
                dayObjects[d].UpdateDayColors(selectedColor, selectedColor, selectedColor);
            }
            else
            {
                if(dayObjects[d].ObjDate == selectedStart)
                {
                    dayObjects[d].UpdateDayColors(null,selectedColor, null);
                }

                if(dayObjects[d].ObjDate == selectedEnd)
                {
                    dayObjects[d].UpdateDayColors(null, null, selectedColor);
                }
            }

        }
    }

    private void UpdateCurentReservationPeriod()
    {
        if(currentReservation != null)
        {
            for (int d = 0; d < dayObjects.Count; d++)
            {
                if(dayObjects[d].ObjDate.Date < DateTime.Today.Date)
                {
                    dayObjects[d].UpdateDayColors(pastColor, pastColor, pastColor);
                    dayObjects[d].IsReserved = true;
                }
                else
                {
                    if(dayObjects[d].ObjDate.Date > currentReservation.Period.Start.Date && dayObjects[d].ObjDate.Date < currentReservation.Period.End.Date)
                    {
                        dayObjects[d].UpdateDayColors(currentColor, currentColor, currentColor);
                    }
                    else
                    {
                        if(dayObjects[d].ObjDate.Date == currentReservation.Period.Start.Date)
                        {
                            dayObjects[d].UpdateDayColors(null,currentColor, null);
                        }

                        if(dayObjects[d].ObjDate.Date == currentReservation.Period.End.Date)
                        {
                            dayObjects[d].UpdateDayColors(null, null, currentColor);
                        }
                    }
                }
            }
        }
    }

    private void UpdateDayCountText()
    {
        int days = (int)(selectedEnd - selectedStart).TotalDays;
        selectionDayCountText.text = String.Format("{0} {1} {2}", Constants.DAY_COUNT_PREF, days, (days == 1) ? Constants.DAY_COUNT_SUFF_SN : Constants.DAY_COUNT_SUFF_PL);
    }
}
