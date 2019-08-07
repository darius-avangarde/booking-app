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
    private RectTransform dayColumnObjectRect;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private RectTransform thisRectTransform;

    private List<CalendarDayColumnObject> dayPool = new List<CalendarDayColumnObject>();
    private DateTime objectDate;
    private Vector3 lastPosition;
    private CalendarDayHeaderObject header;



    private void Start()
    {
        ThemeManager.Instance.AddItems(backgroundImage);
    }

    public void Initialize(DateTime date, List<CalendarRoomColumnObject> roomObjects, UnityAction<DateTime,IRoom> tapAction, CalendarDayHeaderObject linkedHeader)
    {
        objectDate = new DateTime(date.Date.Ticks);
        header = linkedHeader;

        for (int i = 0; i < roomObjects.Count; i++)
        {
            CalendarDayColumnObject dayObj = Instantiate(dayColumnObjectPrefab, transform).GetComponent<CalendarDayColumnObject>();
            dayPool.Add(dayObj);
            dayObj.SetObjectAction(date, tapAction, roomObjects[i]);
            roomObjects[i].AddLinkedDayObject(dayObj);
            //dayObj.DayRectTransform.localPosition = - (Vector3.up * dayColumnObjectRect.rect.height) * (dayPool.Count);
        }
    }

    public void UpdateRooms(List<IRoom> rooms)
    {
        thisRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rooms.Count * dayColumnObjectRect.rect.height);

        for (int i = 0; i < dayPool.Count; i++)
        {
            if(rooms.Count > i)
            {
                dayPool[i].SetPosition(true);
            }
            else
            {
                dayPool[i].DisableObject();
            }
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
            for (int i = dayPool.Count; i < rooms.Count; i++)
            {
                InitializeDayObject();
            }

            //Disable unused objects
            for (int i = dayPool.Count - 1; i > rooms.Count - 1; i--)
            {
                dayPool[i].gameObject.SetActive(false);
            }
        }
    }

    private void InitializeDayObject()
    {
        CalendarDayColumnObject dayObj = Instantiate(dayColumnObjectPrefab, transform).GetComponent<CalendarDayColumnObject>();
        dayPool.Add(dayObj);
        dayObj.DayRectTransform.localPosition = - (Vector3.up * dayColumnObjectRect.rect.height) * (dayPool.Count);
    }
}
