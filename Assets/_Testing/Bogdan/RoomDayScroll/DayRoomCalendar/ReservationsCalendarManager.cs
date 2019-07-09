using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ReservationsCalendarManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dayHeaderPrefab;
    [SerializeField]
    private GameObject dayColumnPrefab;

    [SerializeField]
    private RectTransform dayColumnObjectTransform;

    [Space]
    [SerializeField]
    private ScrollRect dayColumnScrollrect;
    [SerializeField]
    private RectTransform dayHeaderContent;

    [Space]
    [SerializeField]
    private InfiniteDayScroll dayHeaderInfScroll;
    [SerializeField]
    private InfiniteDayScroll dayColumnInfScroll;

    [SerializeField]
    private CalendarRoomColumn roomColumn;

    private IProperty currentProperty;
    private List<IRoom> currentRooms;

    private List<CalendarDayColumn> dayColumns = new List<CalendarDayColumn>();
    private List<CalendarDayHeaderObject> dayHeaders = new List<CalendarDayHeaderObject>();
    private int totalItemCount = 0;



    private void Start()
    {
        dayHeaderInfScroll.onMoveItem = MoveDayHeader;
        dayColumnInfScroll.onMoveItem = MoveDayRoom;
        CreateDayItems();
        dayHeaderInfScroll.Init();
        dayColumnInfScroll.Init();

        StartCoroutine(DelayStartTest(2, 2));
        StartCoroutine(DelayStartTest(4, 0));
    }

    private IEnumerator DelayStartTest(float delay, int properyIndex)
    {
        yield return new WaitForSeconds(delay);
        SelectProperty(PropertyDataManager.GetProperties().ToList()[properyIndex]);
    }

    public void SelectProperty(IProperty property)
    {
        currentProperty = property;
        currentRooms = property.Rooms.ToList();
        UpdateRoomColumn();
        UpdateDayColumns();

        //Resize the day column content rect size to fit the number of rooms
        dayColumnScrollrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentRooms.Count * dayColumnObjectTransform.rect.height);
    }

    private void UpdateRoomColumn()
    {
        roomColumn.UpdateRooms(currentRooms, null);
    }

    private void UpdateDayColumns()
    {
        for (int i = 0; i < currentRooms.Count; i++)
        {
            foreach(CalendarDayColumn dayColumn in dayColumns)
            {
                dayColumn.UpdateRooms(currentRooms);
            }
        }
    }

    private void MoveDayRoom(Transform dayColumnTransform, bool isForward)
    {
        dayColumnTransform.GetComponent<CalendarDayColumn>().OnScrollReposition((isForward) ? totalItemCount : - totalItemCount);
    }

    private void MoveDayHeader(Transform dayHeaderTransform, bool isForward)
    {
        dayHeaderTransform.GetComponent<CalendarDayHeaderObject>().OnScrollReposition((isForward) ? totalItemCount : - totalItemCount);
    }

    //Create wnough day headers and day columns to cover the screen in landscape mode
    private void CreateDayItems()
    {
        float maxScreen = Mathf.Max(Screen.width, Screen.height);
        totalItemCount = Mathf.RoundToInt((maxScreen * 1.3f)/dayHeaderPrefab.GetComponent<RectTransform>().rect.width);

        for (int d = 0; d < totalItemCount; d++)
        {
            //Create day header
            CalendarDayHeaderObject header = Instantiate(dayHeaderPrefab, dayHeaderContent).GetComponent<CalendarDayHeaderObject>();
            header.UpdateDayObject(DateTime.Today.AddDays(d), null, null);
            dayHeaders.Add(header);

            //Create day columns
            CalendarDayColumn dayColumn = Instantiate(dayColumnPrefab, dayColumnScrollrect.content).GetComponent<CalendarDayColumn>();
            dayColumn.Initialize(DateTime.Today.AddDays(d), new List<IRoom>(), null);
            dayColumns.Add(dayColumn);
        }
    }
}
