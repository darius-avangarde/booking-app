using System;
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

    private List<CalendarDayColumn> columnObjects = new List<CalendarDayColumn>();
    private int totalItemCount = 0;



    private void Start()
    {
        dayHeaderInfScroll.onMoveItem = MoveDayHeader;
        dayColumnInfScroll.onMoveItem = MoveDayRoom;
        CreateDayItems();
        dayHeaderInfScroll.Init();
        dayColumnInfScroll.Init();
    }


    public void SelectProperty(IProperty property)
    {
        UpdateRoomColumn(property);
    }

    private void UpdateRoomColumn(IProperty property)
    {
        currentProperty = property;
        currentRooms = property.Rooms.ToList();
        roomColumn.UpdateRooms(currentRooms, null);
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

            List<IRoom> rl = PropertyDataManager.GetProperties().ToList()[1].Rooms.ToList();

            //Create day columns
            CalendarDayColumn dayColumn = Instantiate(dayColumnPrefab, dayColumnScrollrect.content).GetComponent<CalendarDayColumn>();
            dayColumn.UpdateDays(DateTime.Today.AddDays(d), rl, null);

        }
    }

    private void SetProperty(IProperty prop)
    {
        currentProperty = prop;
        currentRooms = prop.Rooms.ToList();

        UpdateRoomItems();
    }

    private void UpdateRoomItems()
    {

    }

}
