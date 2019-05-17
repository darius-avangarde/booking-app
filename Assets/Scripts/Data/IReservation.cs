using System;

public interface IReservation
{
    string ID { get; }
    string CustomerName { get; set; }
    string CustomerID { get; set; }
    string PropertyID { get; }
    string RoomID { get; }
    IDateTimePeriod Period { get; }

    void EditReservation(IRoom room, string _customerID, DateTime start, DateTime end);
}
