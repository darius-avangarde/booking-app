using System;

public interface IReservation_New
{
    string ID { get; }
    string CustomerID { get; }
    string PropertyID { get; }
    string RoomID { get; }
    IDateTimePeriod Period { get; }

    void EditReservation(IRoom room, string _customerID, DateTime start, DateTime end);
}
