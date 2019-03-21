﻿using System;
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

    public void ShowModalCalendar()
    {
        roomReservationList = GetRoomReservations()
                            .Where(res => res != currentReservation)
                            .OrderBy(res => res.Period.Start).ToList();
        modalCalendarDialog.Show(currentReservation, currentRoom, roomReservationList, SaveAndShowUpdatedReservationPeriod);
    }

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

    public void ShowDeleteConfirmationDialog()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Anulați rezervarea?",
            ConfirmText = "Anulați",
            CancelText = "Nu anulați",
            ConfirmCallback = DeleteReservation,
            CancelCallback = null
        });
    }

    private void DeleteReservation()
    {
        if (currentReservation != null)
        {
            ReservationDataManager.DeleteReservation(currentReservation.ID);
            navigator.GoBack();
            calendar.UpdateItemsStatusOnCalendarAndDayScreenOnReservationChange();
        }
        else
        {
            navigator.GoBack();
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

    private void AddNewReservation(DateTime start, DateTime end)
    {
        IReservation reservation = ReservationDataManager.AddReservation(currentRoom);
        reservation.CustomerName = reservationCustomerName;
        reservation.Period.Start = start;
        reservation.Period.End = end;
        currentReservation = reservation;
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
        else
        {
            if (start == default)
            {
                navigator.GoBack();
            }
            else
            {
                AddNewReservation(startPeriod, endPeriod);
            }
        }
        reservationPeriodText.text = startPeriod.ToString(Constants.DateTimePrintFormat)
                                   + Constants.AndDelimiter
                                   + endPeriod.ToString(Constants.DateTimePrintFormat);
    }

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
