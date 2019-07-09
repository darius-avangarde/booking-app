using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CalendarDayColumn : MonoBehaviour
{
    public DateTime ObjectDate => objectDate;

    [SerializeField]
    private GameObject dayColumnObjectPrefab;

    private List<CalendarDayColumnObject> dayPool = new List<CalendarDayColumnObject>();
    private DateTime objectDate;


    public void Initialize(DateTime date, List<IRoom> rooms, UnityAction<DateTime> action)
    {
        objectDate = new DateTime(date.Date.Ticks);

        ManagePool(rooms);

        for (int r = 0; r < rooms.Count; r++)
        {
            dayPool[r].UpdateEnableDayObject(objectDate, rooms[r], null);
        }
    }

    public void UpdateRooms(List<IRoom> rooms)
    {
        ManagePool(rooms);

        for (int r = 0; r < rooms.Count; r++)
        {
            dayPool[r].UpdateEnableDayObject(objectDate, rooms[r], null);
        }
    }

    public void OnScrollReposition(int offsetDays)
    {
        objectDate = objectDate.AddDays(offsetDays);

        for (int r = 0; r < dayPool.Count; r++)
        {
            dayPool[r].UpdateDayObjectDate(objectDate);
        }
    }

    private void ManagePool(List<IRoom> rooms)
    {
        if(rooms.Count != dayPool.Count)
        {
            //CreateNewObjects as needed
            while(dayPool.Count < rooms.Count)
            {
                CreateDayColumnObject();
            }

            //Disable unused objects
            for (int i = dayPool.Count - 1; i > rooms.Count; i--)
            {
                dayPool[i].gameObject.SetActive(false);
            }
        }
    }

    private void CreateDayColumnObject()
    {
        dayPool.Add(Instantiate(dayColumnObjectPrefab, transform).GetComponent<CalendarDayColumnObject>());
    }
}
