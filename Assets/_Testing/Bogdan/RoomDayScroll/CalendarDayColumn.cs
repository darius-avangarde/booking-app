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

    public void UpdateDays(DateTime date, List<IRoom> rooms, UnityAction<DateTime> action)
    {
        objectDate = new DateTime(date.Date.Ticks);
        if(rooms.Count != dayPool.Count)
        {
            ManagePool(rooms.Count);
        }

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

    private void ManagePool(int roomCount)
    {
        //CreateNewObjects as needed
        while(dayPool.Count < roomCount)
        {
            CreateDayColumnObject();
        }

        //Disable unused objects
        for (int i = dayPool.Count - 1; i > roomCount; i--)
        {
            dayPool[i].gameObject.SetActive(false);
        }
    }

    private void CreateDayColumnObject()
    {
        dayPool.Add(Instantiate(dayColumnObjectPrefab, transform).GetComponent<CalendarDayColumnObject>());
    }
}
