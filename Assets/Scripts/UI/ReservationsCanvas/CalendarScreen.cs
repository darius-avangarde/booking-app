using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarScreen : MonoBehaviour
{
    [HideInInspector]
    public List<IRoom> roomList = new List<IRoom>();
    [SerializeField]
    private CalendarFilterDialog modalCalendarFilterDialog = null;
    private RoomFilter filter = null;
    // Start is called before the first frame update
    void Start()
    {
        filter = new RoomFilter();
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            foreach (IRoom room in property.Rooms)
            {
                roomList.Add(room);
            }
        }
    }

    public void ShowModalCalendar()
    {
        modalCalendarFilterDialog.Show(filter, (updatedFilter) => {
            roomList = updatedFilter.Apply(roomList);
        });
    }
}
