using System;
using System.Collections.Generic;

public interface IReservation
{
    string ID { get; }
    string CustomerName { get; set; }
    string CustomerID { get; set; }
    string PropertyID { get; }
    string RoomID { get; }
    List<string> RoomIDs { get; }
    IDateTimePeriod Period { get; }
    DateTime CreatedDateTime { get; }
    int NotificationID { get; set;}
    bool Canceled {get;}

    void EditReservation(List<IRoom> rooms, IClient client, DateTime start, DateTime end);
    void UpdateReservationNotificationID(int notitficationID);
    void CancelReservation();
    bool ContainsRoom(string roomID);
}
