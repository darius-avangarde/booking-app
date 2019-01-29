using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReservationScreen : MonoBehaviour
{
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
        IProperty property = PropertyDataManager.GetProperty(reservation.PropertyID);
        propertyTitleField.text = property.Name ?? Constants.defaultProperyAdminScreenName;
        roomTitleField.text = property.GetRoom(reservation.RoomID).Name ?? Constants.defaultRoomAdminScreenName;
    }

    public void ShowModalCalendar()
    {
        modalCalendarDialog.Show(() => {
            
        }, null);
    }
}
