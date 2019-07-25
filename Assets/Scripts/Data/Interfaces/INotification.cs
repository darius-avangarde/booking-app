using System;
using System.Collections.Generic;

public interface INotification 
{
    List<string> NewNotifications { get; }

    void SetReservationIDs(List<IReservation> reservationIDs);
}
