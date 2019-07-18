using System;
using UnityEngine;

public class ReservationFilter
{
    public DateTime? startDate;
    public DateTime? endDate;
    public PropertyDataManager.RoomType? roomType;
    public Vector2Int? beds;
    public IClient client;

    public ReservationFilter()
    {
        startDate   = null;
        endDate     = null;
        roomType    = null;
        beds        = null;
        client      = null;
    }
}
