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
    private RectTransform thisRect;
    [SerializeField]
    private Image backgroundImage;

    private List<CalendarDayColumnObject> dayPool = new List<CalendarDayColumnObject>();
    private DateTime objectDate;
    private Vector3 lastPosition;
    private CalendarDayHeaderObject header;
    private ScrollRect parentScrollrect;


    private void Start()
    {
        ThemeManager.Instance.AddItems(backgroundImage);
    }

    public void Initialize(DateTime date, List<IRoom> rooms, UnityAction<DateTime,IRoom> tapAction, CalendarDayHeaderObject linkedHeader, ScrollRect dayColumnScrolrect)
    {
        parentScrollrect = dayColumnScrolrect;
        objectDate = new DateTime(date.Date.Ticks);
        parentScrollrect.onValueChanged.AddListener(ManageVisible);

        ManagePool(rooms);

        for (int r = 0; r < rooms.Count; r++)
        {
            dayPool[r].UpdateDayObject(objectDate, rooms[r], tapAction);
        }

        header = linkedHeader;
    }

    public void UpdateRooms(List<IRoom> rooms, UnityAction<DateTime,IRoom> tapAction)
    {
        thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rooms.Count * dayColumnObjectRect.rect.height);
        ManagePool(rooms);

        for (int r = 0; r < rooms.Count; r++)
        {
            dayPool[r].UpdateDayObject(objectDate, rooms[r], tapAction);
        }

        ManageVisible(Vector2.zero);
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
                dayPool[i].Disable();
            }
        }
    }

    private void ManageVisible(Vector2 pos)
    {
        for (int i = 0; i < dayPool.Count; i++)
        {
            if(dayPool[i].HasRoom)
            {
                if (dayPool[i].DayRectTransform.position.y < 0
                || dayPool[i].DayRectTransform.position.y > parentScrollrect.viewport.position.y + dayColumnObjectRect.rect.height)
                {
                    dayPool[i].gameObject.layer = 8;
                    // dayPool[i].gameObject.SetActive(false);
                }
                else
                {
                    dayPool[i].gameObject.layer = 5;
                    // dayPool[i].gameObject.SetActive(true);
                }
            }
        }
    }

    private void CreateDayColumnObject()
    {
        //dayPool.Add(Instantiate(dayColumnObjectPrefab, transform).GetComponent<CalendarDayColumnObject>());
        dayPool.Add(Instantiate(dayColumnObjectPrefab, thisRect).GetComponent<CalendarDayColumnObject>());
        dayPool[dayPool.Count -1].DayRectTransform.localPosition = ItemPosition(dayPool.Count - 1);
    }

    private Vector3 ItemPosition(int itemNumber)
    {
        return - Vector2.up * dayColumnObjectRect.rect.height * itemNumber;
    }
}
