using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarDayColumn : MonoBehaviour
{
    public DateTime ObjectDate => objectDate;
    public CalendarDayHeaderObject LinkedHeader => header;
    public List<CalendarDayColumnObject> ActiveDayColumnsObjects => dayPool.FindAll(a => a.gameObject.activeSelf);

    [SerializeField]
    private GameObject dayColumnObjectPrefab;
    [SerializeField]
    private Image backgroundImage;

    private List<CalendarDayColumnObject> dayPool = new List<CalendarDayColumnObject>();
    private DateTime objectDate;
    private Vector3 lastPosition;
    private CalendarDayHeaderObject header;


    private void Start()
    {
        ThemeManager.Instance.AddItems(backgroundImage);
    }

    public void Initialize(DateTime date, int initialItemCount, UnityAction<DateTime,IRoom> tapAction, CalendarDayHeaderObject linkedHeader)
    {
        objectDate = new DateTime(date.Date.Ticks);

        for (int i = 0; i < initialItemCount; i++)
        {
            CreateDayColumnObject();
        }
        // ManagePool(rooms);

        // for (int r = 0; r < rooms.Count; r++)
        // {
        //     dayPool[r].UpdateEnableDayObject(objectDate, rooms[r], tapAction);
        // }

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

        LinkedHeader.UpdateUI(date);
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
            for (int i = dayPool.Count - 1; i > rooms.Count - 1; i--)
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
