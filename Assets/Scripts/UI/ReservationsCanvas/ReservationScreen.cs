using System;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class ReservationScreen : MonoBehaviour
{
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Calendar calendar = null;
    [SerializeField]
    private InputField customerNameInputField = null;
    [SerializeField]
    private Text propertyTitleField = null;
    [SerializeField]
    private Text roomTitleField = null;
    [SerializeField]
    private Text reservationPeriodText = null;
    [SerializeField]
    private ModalCalendar modalCalendarDialog = null;
    private IReservation currentReservation;
    private IRoom currentRoom;
    private List<IReservation> roomReservationList = new List<IReservation>();
    private string reservationCustomerName;
    private DateTime startPeriod;
    private DateTime endPeriod;

    // TODO: ReservationScreen should initialize its fields on NavScreen.Showing, not when it receives Reservation and Room data
    // making this change might need us to change how we create new reservations
    // Let's discuss before working on this part
    public void UpdateReservationScreen(IReservation reservation, IRoom room)
    {
        currentReservation = reservation;
        currentRoom = room;
        roomReservationList = GetRoomReservations().OrderBy(res => res.Period.Start).ToList();
        if (currentReservation != null)
        {
            UpdateCurrentReservationFields(reservation);
        }
        else
        {
            modalCalendarDialog.Show(currentReservation, currentRoom, roomReservationList, SaveAndShowUpdatedReservationPeriod);
            UpdateNewReservationFields();
        }
    }

    private void UpdateNewReservationFields()
    {
        customerNameInputField.text = Constants.defaultCustomerName;
        propertyTitleField.text = PropertyDataManager.GetProperty(currentRoom.PropertyID).Name ?? Constants.defaultProperyAdminScreenName;
        roomTitleField.text = currentRoom.Name ?? Constants.defaultRoomAdminScreenName;
    }

    private void UpdateCurrentReservationFields(IReservation reservation)
    {
        customerNameInputField.text = string.IsNullOrEmpty(reservation.CustomerName) ? Constants.defaultCustomerName : currentReservation.CustomerName;
        propertyTitleField.text = PropertyDataManager.GetProperty(currentRoom.PropertyID).Name ?? Constants.defaultProperyAdminScreenName;
        roomTitleField.text = currentRoom.Name ?? Constants.defaultRoomAdminScreenName;
        string startPeriod = reservation.Period.Start.ToString(Constants.DateTimePrintFormat);
        string endPeriod = reservation.Period.End.ToString(Constants.DateTimePrintFormat);
        reservationPeriodText.text = startPeriod + Constants.AndDelimiter + endPeriod;
    }

    public void ShowModalCalendar()
    {
        roomReservationList = GetRoomReservations()
                            .Where(res => res != currentReservation)
                            .OrderBy(res => res.Period.Start).ToList();
        modalCalendarDialog.Show(currentReservation, currentRoom, roomReservationList, SaveAndShowUpdatedReservationPeriod);
    }

    public void SaveReservation()
    {
        if (currentReservation == null)
        {
            AddNewReservation(startPeriod, endPeriod);
        }
    }

    // TODO: prefer naming event handlers HandleSomething. OnSomething should either be an event or a method that raises an event
    public void OnValueChanged(string value)
    {
        if (currentReservation != null)
        {
            currentReservation.CustomerName = string.IsNullOrEmpty(value) ? Constants.defaultCustomerName : value;
        }
        else
        {
            reservationCustomerName = string.IsNullOrEmpty(value) ? Constants.defaultCustomerName : value;
        }
    }

    private void AddNewReservation(DateTime start, DateTime end)
    {
        IReservation reservation = ReservationDataManager.AddReservation(currentRoom);
        reservation.CustomerName = reservationCustomerName;
        reservation.Period.Start = start;
        reservation.Period.End = end;
        currentReservation = reservation;
    }

    public void DeleteReservation()
    {
        if (currentReservation != null)
        {
            // TODO: second parameter is optional
            confirmationDialog.Show(() => {
                ReservationDataManager.DeleteReservation(currentReservation.ID);
                navigator.GoBack();
                // TODO: we shouldn't need to do this, Calendar itself or its container screen (CalendarScreen) should update it
                calendar.UpdateItemsStatusOnCalendarAndDayScreenOnReservationChange();
            }, null);
        }
        else
        {
            // TODO: second parameter is optional
            confirmationDialog.Show(() => {
                navigator.GoBack();
            }, null);
        }
    }

    private void SaveAndShowUpdatedReservationPeriod(DateTime start, DateTime end)
    {
        startPeriod = start;
        endPeriod = end;
        if (currentReservation != null)
        {
            currentReservation.Period.Start = startPeriod;
            currentReservation.Period.End = endPeriod;
        }
        reservationPeriodText.text = startPeriod.ToString(Constants.DateTimePrintFormat)
                                   + Constants.AndDelimiter
                                   + endPeriod.ToString(Constants.DateTimePrintFormat);
    }

    // TODO: we can use IEnumerable.Where instead of iterating ourselves
    // it may be worth replacing calls to this method with ReservationDataManager.GetReservations().Where(...)
    private List<IReservation> GetRoomReservations()
    {
        List<IReservation> roomReservationList = new List<IReservation>();
        foreach (var reservation in ReservationDataManager.GetReservations())
        {
            if (reservation.RoomID == currentRoom.ID)
            {
                roomReservationList.Add(reservation);
            }
        }
        return roomReservationList;
    }

}
