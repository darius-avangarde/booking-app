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

    void EditReservation(IRoom room, IClient client, DateTime start, DateTime end);
    void EditReservation(List<IRoom> rooms, IClient client, DateTime start, DateTime end);
}
