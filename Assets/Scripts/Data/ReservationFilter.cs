using System;
using System.Collections.Generic;
using UnityEngine;

public class ReservationFilter
{
    public string ReservationID { get; set; } = null;
    public string PropertyID { get; set; } = null;
    public string RoomID { get; set; } = null;
    public string CustomerName { get; set; } = null;
    public DateTime? StartTime { get; set; } = null;
    public DateTime? EndTime { get; set; } = null;

    public List<IReservation> Apply(List<IReservation> reservations)
    {
        return reservations.FindAll(r =>
        {
            if (ReservationID != null && !r.ID.Equals(ReservationID))
            {
                return false;
            }

            if (PropertyID != null && !r.PropertyID.Equals(PropertyID))
            {
                return false;
            }

            if (RoomID != null && !r.RoomID.Equals(RoomID))
            {
                return false;
            }

            if (CustomerName != null && !r.ClientName.Equals(CustomerName))
            {
                return false;
            }

            if (StartTime != null && r.Period.End < StartTime.Value)
            {
                return false;
            }

            if (EndTime != null && r.Period.End > EndTime.Value)
            {
                return false;
            }

            return true;
        });
    }

    public void Reset()
    {
        ReservationID = null;
        PropertyID = null;
        RoomID = null;
        CustomerName = null;
        StartTime = null;
        EndTime = null;
    }
}
