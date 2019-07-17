using System;

public class ReservationFilter
{
    public DateTime? startDate;
    public DateTime? endDate;

    //public ROOMTYPE?

    public int? singleBeds;
    public int? doubleBeds;

    public IClient client;

    public ReservationFilter()
    {
        startDate   = null;
        endDate     = null;
        //ROOMTYPE  = null;
        singleBeds  = null;
        doubleBeds  = null;
        client      = null;
    }
}
