using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CalendarDayColumn : MonoBehaviour
{
    public DateTime ObjectDate => objectDate;
    public CalendarDayHeaderObject LinkedHeader => header;

    [SerializeField]
    private GameObject dayColumnObjectPrefab;
    [SerializeField]
    private RectTransform thisRectTransform;

    private List<CalendarDayColumnObject> dayPool = new List<CalendarDayColumnObject>();
    private DateTime objectDate;

    private Vector3 lastPosition;
    private UnityAction<DateTime,CalendarDayColumn> onMonthChange;
    private CalendarDayHeaderObject header;

    //TODO: Find better way to determine focal month and trigger month change
    private void Update()
    {
        if(ReservationsCalendarManager.FocalDate.Date != objectDate.Date && transform.position != lastPosition)
        {
            lastPosition = transform.position;
            if(transform.position.x - transform.parent.parent.position.x > 0 && transform.position.x - transform.parent.parent.position.x < thisRectTransform.rect.width * 4)
            {
                onMonthChange?.Invoke(objectDate, this);
            }
        }
    }

    private void OnDestroy()
    {
        onMonthChange = null;
    }

    public void Initialize(DateTime date, List<IRoom> rooms, UnityAction<DateTime,IRoom> tapAction, UnityAction<DateTime,CalendarDayColumn> setMonth, CalendarDayHeaderObject linkedHeader)
    {
        objectDate = new DateTime(date.Date.Ticks);

        ManagePool(rooms);

        for (int r = 0; r < rooms.Count; r++)
        {
            dayPool[r].UpdateEnableDayObject(objectDate, rooms[r], tapAction);
        }

        onMonthChange = null;
        onMonthChange = setMonth;
        header = linkedHeader;
    }

    public void UpdateRooms(List<IRoom> rooms, UnityAction<DateTime,IRoom> tapAction)
    {
        ManagePool(rooms);

        for (int r = 0; r < rooms.Count; r++)
        {
            dayPool[r].UpdateEnableDayObject(objectDate, rooms[r], tapAction);
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

    public void SetDate(DateTime date)
    {
        objectDate = new DateTime(date.Date.Ticks);

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
