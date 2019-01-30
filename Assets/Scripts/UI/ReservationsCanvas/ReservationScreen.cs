using System.Collections;
using System.Collections.Generic;
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
    private ModalCalendar modalCalendarDialog = null;
    private IReservation currentReservation;
    
    public void UpdateReservationFields(IReservation reservation)
    {
        currentReservation = reservation;
        customerNameInputField.text = string.IsNullOrEmpty(currentReservation.CustomerName) ? Constants.defaultCustomerName : currentReservation.CustomerName;
        IProperty property = PropertyDataManager.GetProperty(reservation.PropertyID);
        propertyTitleField.text = property.Name ?? Constants.defaultProperyAdminScreenName;
        roomTitleField.text = property.GetRoom(reservation.RoomID).Name ?? Constants.defaultRoomAdminScreenName;
    }

    public void ShowModalCalendar()
    {
        modalCalendarDialog.Show(() => {
            
        }, null);
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
}
