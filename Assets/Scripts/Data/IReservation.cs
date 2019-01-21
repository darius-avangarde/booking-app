using System;
using System.Collections.Generic;

public interface IReservation
{
    string ID { get; }
    string CustomerName { get; set; }
    string PropertyID { get; }
    string RoomID { get; }
    IDateTimePeriod Period { get; }
}

public interface IDateTimePeriod
{
    DateTime Start { get; set; }
    DateTime End { get; set; }
}
