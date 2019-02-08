using System;
using System.Collections;
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
    private Transform roomScreenTransform = null;
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
    private List<IReservation> roomReservationList = new List<IReservation>();

    public void UpdateReservationFields(IReservation reservation)
    {
        currentReservation = reservation;
        customerNameInputField.text = string.IsNullOrEmpty(currentReservation.CustomerName) ? Constants.defaultCustomerName : currentReservation.CustomerName;
        IProperty property = PropertyDataManager.GetProperty(reservation.PropertyID);
        propertyTitleField.text = property.Name ?? Constants.defaultProperyAdminScreenName;
        roomTitleField.text = property.GetRoom(reservation.RoomID).Name ?? Constants.defaultRoomAdminScreenName;
        string startPeriod = reservation.Period.Start.ToString(Constants.DateTimePrintFormat);
        string endPeriod = reservation.Period.End.ToString(Constants.DateTimePrintFormat);
        roomReservationList = GetRoomReservations(currentReservation).OrderBy(res => res.Period.Start).ToList();
        reservationPeriodText.text = currentReservation.Period.Start != DateTime.MinValue ? startPeriod + Constants.AndDelimiter + endPeriod : "Seteaza Perioda";
    }

    public void ShowModalCalendar()
    {
        modalCalendarDialog.Show(currentReservation, roomReservationList, ShowReservationPeriod);
    }
    
    public void OnValueChanged(string value)
    {
        currentReservation.CustomerName = string.IsNullOrEmpty(value) ? Constants.defaultCustomerName : value;
    }

    public void DeleteReservation()
    {
        confirmationDialog.Show(() => {
            ReservationDataManager.DeleteReservation(currentReservation.ID);
            navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
            roomScreenTransform.GetComponent<RoomScreen>().InstantiateReservations();
        }, null);
    }

    private void ShowReservationPeriod()
    {
        string startPeriod = currentReservation.Period.Start.ToString(Constants.DateTimePrintFormat);
        string endPeriod = currentReservation.Period.End.ToString(Constants.DateTimePrintFormat);
        reservationPeriodText.text = startPeriod + Constants.AndDelimiter + endPeriod;
    }
    
    private List<IReservation> GetRoomReservations(IReservation currentReservation)
    {
        List<IReservation> roomReservationList = new List<IReservation>();
        if (currentReservation != null)
        {
            foreach (var reservation in ReservationDataManager.GetReservations())
            {
                if (reservation.RoomID == currentReservation.RoomID)
                {
                    roomReservationList.Add(reservation);
                }
            }
        }
        return roomReservationList;
    }

}
