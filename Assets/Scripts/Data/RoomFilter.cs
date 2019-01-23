using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomFilter
{
    public string PropertyID { get; set; } = null;
    public string RoomID { get; set; } = null;
    public int SingleBeds { get; set; } = 0;
    public int DoubleBeds { get; set; } = 0;
    public int RoomCapacity { get; set; } = 0;

    public List<IRoom> Apply(List<IRoom> rooms)
    {
        return rooms.FindAll(r =>
        {
            if (PropertyID != null && !r.PropertyID.Equals(PropertyID))
            {
                return false;
            }

            if (RoomID != null && !r.ID.Equals(RoomID))
            {
                return false;
            }

            if (SingleBeds > 0 && !r.SingleBeds.Equals(SingleBeds))
            {
                return false;
            }

            if (DoubleBeds > 0 && !r.DoubleBeds.Equals(DoubleBeds))
            {
                return false;
            }

            if (RoomCapacity > 0 && !r.Persons.Equals(RoomCapacity))
            {
                return false;
            }

            return true;
        });
    }

    public void Reset()
    {
        PropertyID = null;
        RoomID = null;
        SingleBeds = 0;
        DoubleBeds = 0;
        RoomCapacity = 0;
    }
}
