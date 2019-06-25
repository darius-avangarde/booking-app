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

    void EditReservation(List<IRoom> rooms, IClient client, DateTime start, DateTime end);
    bool ContainsRoom(string roomID);
}
