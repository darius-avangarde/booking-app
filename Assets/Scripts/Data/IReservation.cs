using System;

public interface IReservation
{
    string ID { get; }
    string ClientName { get; set; }
    string ClientID { get; set; }
    string PropertyID { get; }
    string RoomID { get; }
    IDateTimePeriod Period { get; }

    void EditReservation(IRoom room, IClient client, DateTime start, DateTime end);
}
