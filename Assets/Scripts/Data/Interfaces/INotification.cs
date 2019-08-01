using System;
using System.Collections.Generic;

public interface INotification 
{
    int NotificationID { get; set; }
    int PreAlertTime { get; set; }
    DateTime FireTime { get; set; }
    bool UnRead { get; set; }
    List<string> ReservationIDs { get; }

    void SetReservationIDs(List<IReservation> reservationsList);
}
