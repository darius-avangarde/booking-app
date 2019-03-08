using System;

public interface IReservation
{
    string ID { get; }
    string CustomerName { get; set; }
    string PropertyID { get; }
    string RoomID { get; }
    IDateTimePeriod Period { get; }
}
