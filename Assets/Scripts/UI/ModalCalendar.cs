using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalCalendar : MonoBehaviour
{
    [SerializeField]
    private EasyTween easyTween = null;

    private Action<DateTime, DateTime> DoneCallback;
    [SerializeField]
    private Text monthName = null;
    [SerializeField]
    private Transform modalDayItemsInCalendarPanel = null;
    private DateTime selectedDateTime = DateTime.Today;
    private DateTime newReservationStartDateTime;
    private DateTime newReservationEndDateTime;
    private bool isSetStartDay = false;
    private IReservation currentReservation;
    private IRoom currentRoom;
    private List<IReservation> roomReservationList = new List<IReservation>();
    private Color selectedItemColor = Color.cyan;
    private DateTime currentReservationStartPeriod;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateModalCalendarDayItems();
        UpdateCalendar(selectedDateTime);
    }

    private void InstantiateModalCalendarDayItems()
    {
        for (int dayItemIndex = 0; dayItemIndex < modalDayItemsInCalendarPanel.childCount; dayItemIndex++)
        {
            ModalCalendarDayItem dayItem = modalDayItemsInCalendarPanel.GetChild(dayItemIndex).GetComponent<ModalCalendarDayItem>();
            dayItem.Initialize(SetReservationPeriod);
        }
    }
    
    public void Show(IReservation reservation, IRoom room, List<IReservation> reservationList, Action<DateTime, DateTime> doneCallback)
    {
        currentReservation = reservation;
        currentRoom = room;
        roomReservationList = reservationList;
        easyTween.OpenCloseObjectAnimation();
        DoneCallback = doneCallback;

        if (currentReservation != null)
        {
            selectedDateTime = currentReservation.Period.Start;
        }
        UpdateCalendar(selectedDateTime);
    }

    public void ShowPreviousMonth()
    {
        if (selectedDateTime > DateTime.Today && selectedDateTime.Month != DateTime.Today.Month)
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

    private void CloseModalCalendar()
    {
        DoneCallback?.Invoke(newReservationStartDateTime, newReservationEndDateTime);
        DoneCallback = null;
        easyTween.OpenCloseObjectAnimation();
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
            bool isAvailableDay = true;
            bool isReservedDay = IsDayReservedInRoomReservations(nextMonthDateTime);
            bool isReservedDayAvailable = IsReservedDayAvailableInRoomReservations(nextMonthDateTime);
            dayItem.UpdateModalDayItem(nextMonthDateTime, currentReservation, isAvailableDay, isReservedDay, isReservedDayAvailable);
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
            ModalCalendarDayItem dayItem = modalDayItemsInCalendarPanel.GetChild(i).GetComponent<ModalCalendarDayItem>();
            bool isAvailableDay = selectedDateTime >= DateTime.Today;
            bool isReservedDay = IsDayReservedInRoomReservations(selectedDateTime);
            bool isReservedDayAvailable = IsReservedDayAvailableInRoomReservations(selectedDateTime);
            dayItem.UpdateModalDayItem(selectedDateTime, currentReservation, isAvailableDay, isReservedDay, isReservedDayAvailable);
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
            bool isAvailableDay = selectedDateTime > DateTime.Today && selectedDateTime.Month != DateTime.Today.Month;
            bool isReservedDay = IsDayReservedInRoomReservations(previousMonthDateTime);
            bool isReservedDayAvailable = IsReservedDayAvailableInRoomReservations(previousMonthDateTime);
            dayItem.UpdateModalDayItem(previousMonthDateTime, currentReservation, isAvailableDay, isReservedDay, isReservedDayAvailable);
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

    private void SetReservationPeriod(DateTime dateTime, Image modalCalendarDayItemImage, bool isReserved, bool isReservedDayAvailable)
    {
        bool isDateTimeInReservationPeriod = false;
        if (currentReservation != null)
        {
            isDateTimeInReservationPeriod = dateTime >= currentReservation.Period.Start && dateTime <= currentReservation.Period.End;
        }
        
        if (isDateTimeInReservationPeriod || !isReserved || isReservedDayAvailable)
        {
            if (!isSetStartDay && !IsDateTimeStartOfAnyReservationPeriod(dateTime))
            {
                SetStartPeriodDateTime(dateTime);
                ShowSelectedDayItem(modalCalendarDayItemImage);
            }
            else if (isSetStartDay && dateTime > newReservationStartDateTime && IsEndPeriodAvailableOnSelect(dateTime))
            {
                SetEndPeriodDateTime(dateTime);
                ShowSelectedDayItem(modalCalendarDayItemImage);
            }
        }
    }

    private void ShowSelectedDayItem(Image modalCalendarDayItemImage)
    {
        modalCalendarDayItemImage.color = selectedItemColor;
    }

    private void SetEndPeriodDateTime(DateTime dateTime)
    {
        newReservationEndDateTime = dateTime;
        CloseModalCalendar();
        isSetStartDay = false;
    }

    private void SetStartPeriodDateTime(DateTime dateTime)
    {
        newReservationStartDateTime = dateTime;
        isSetStartDay = true;
    }

    private bool IsDayReservedInRoomReservations(DateTime itemDateTime)
    {
        foreach (var reservation in roomReservationList)
        {
            bool isDateTimeInReservationPriod = itemDateTime >= reservation.Period.Start && itemDateTime <= reservation.Period.End;
            if (isDateTimeInReservationPriod)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsReservedDayAvailableInRoomReservations(DateTime itemDateTime)
    {
        foreach (var reservation in roomReservationList)
        {
            if (reservation != currentReservation)
            {
                if (reservation.Period.Start == itemDateTime || reservation.Period.End == itemDateTime)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsDateTimeStartOfAnyReservationPeriod(DateTime itemDateTime)
    {
        foreach (var reservation in roomReservationList)
        {
            if (reservation != currentReservation)
            {
                if (itemDateTime == reservation.Period.Start)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsEndPeriodAvailableOnSelect(DateTime itemDateTime)
    {
        if (roomReservationList.Count == 0)
        {
            return true;
        }
        for (int i = 0; i < roomReservationList.Count; i++)
        {
            if (newReservationStartDateTime < roomReservationList[0].Period.Start 
             && itemDateTime <= roomReservationList[0].Period.Start)
            {
                return true;
            }

            if (newReservationStartDateTime >= roomReservationList[roomReservationList.Count-1].Period.End 
             && itemDateTime > roomReservationList[roomReservationList.Count-1].Period.End)
            {
                return true;
            }
            
            if (newReservationStartDateTime >= roomReservationList[i].Period.End && itemDateTime > roomReservationList[i].Period.End
                && itemDateTime <= roomReservationList[i + 1].Period.Start)
            {
                return true;
            }

            //if (currentReservation == roomReservationList[i])
            //{
            //    return true;
            //}
        }
        return false;
    }
}
