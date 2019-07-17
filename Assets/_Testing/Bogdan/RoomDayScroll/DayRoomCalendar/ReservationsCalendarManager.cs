using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ReservationsCalendarManager : MonoBehaviour
{
    [SerializeField]
    ModalCalendar m;

    public void TestOpenModalCalNULL()
    {
        m.OpenCallendar((d) => Debug.Log("callback " + d.ToString(Constants.DateTimePrintFormat)));
    }

    public void TestOpenModalCalTODAY()
    {
        m.OpenCallendar((d) => Debug.Log("callback " + d.ToString(Constants.DateTimePrintFormat)), DateTime.Today.Date);
    }


    public void TestOpenModalCal31Days()
    {
        m.OpenCallendar((d) => Debug.Log("callback " + d.ToString(Constants.DateTimePrintFormat)), DateTime.Today.AddDays(45).Date);
    }

    ///<summary>
    ///Returns a list of all calendar day columns ordered by heirarchy
    ///</summary>
    public List<CalendarDayColumn> OrderedDayColumns => dayColumns.OrderBy(a => a.transform.GetSiblingIndex()).ToList();

    [SerializeField]
    private TopCalendar topCalendar;
    [SerializeField]
    private ReservationObjectManager reservationManager;
    [SerializeField]
    private PropertyDropdownHandler propertyDropdown;

    [Space]
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
    private ScrollviewHandler scrollviewHandler;
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

    public static DateTime FocalDate => focalDate;
    private static DateTime focalDate;

    private CalendarDayColumn focalDayColumn;
    private bool infiniteScrollsInitialized = false;

    public int FocalDayColumnIndex => dayColumns.Count(d => d.transform.position.x < dayColumnScrollrect.viewport.position.x);


    private void Start()
    {
        propertyDropdown.OnSelectProperty = SelectProperty;

        dayColumnInfScroll.onMoveItem = UpdateDayColumnDate;
        CreateDayItems();

        //Load first property to initialize day columns/items;
        SelectProperty(PropertyDataManager.GetProperties().ToList()[0]);

        dayHeaderInfScroll.Init();
        dayColumnInfScroll.Init();
    }

    ///<summary>
    ///Updates the room/day calendar items such that the first visible date is set to the given datetime
    ///</summary>
    public void JumpToDate(DateTime date)
    {
        DateTime offsetDate = date.AddDays(-FocalDayColumnIndex).Date;
        dayColumns = OrderedDayColumns;

        for (int i = 0; i < dayColumns.Count; i++)
        {
            dayColumns[i].SetDate(offsetDate.AddDays(i));
        }

        //TODO: Replace debug with open reservation edit with prop and client
        reservationManager.SweepUpdateReservations(currentProperty, (r) => Debug.Log($"Edit res for: {r.CustomerName}"));

        topCalendar.OpenCloseDropdownCalendar();
    }

    //TODO: Replace debug logs with fuctions from reservation edit/new screen
    public void SelectProperty(IProperty property)
    {
        currentProperty = property;
        currentRooms = property.Rooms.ToList();
        roomColumn.UpdateRooms(currentRooms, (r) => Debug.Log($"Making reservation for room {r.Name}"));

        for (int i = 0; i < currentRooms.Count; i++)
        {
            foreach(CalendarDayColumn dayColumn in dayColumns)
            {
                dayColumn.UpdateRooms(currentRooms, (a,l) => Debug.Log($"Making reservation for day {a.ToString(Constants.DateTimePrintFormat)} property {l.Name}"));
                dayColumn.LinkedHeader.UpdateProperty(currentProperty);
            }
        }

        //Resize the day column content rect size to fit the number of rooms
        dayColumnScrollrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentRooms.Count * dayColumnObjectTransform.rect.height);

        reservationManager.SweepUpdateReservations(currentProperty, (r) => Debug.Log($"Edit res for: {r.CustomerName}"));
        LayoutRebuilder.ForceRebuildLayoutImmediate(dayColumnScrollrect.content);
    }

    //Triggered on infinite scroll moving a calendar day column.
    private void UpdateDayColumnDate(Transform dayColumnTransform, bool isForward)
    {
        CalendarDayColumn d = dayColumnTransform.GetComponent<CalendarDayColumn>();
        d.OnScrollReposition((isForward) ? totalItemCount : - totalItemCount);
        d.LinkedHeader.OnScrollReposition((isForward) ? totalItemCount : - totalItemCount);

        reservationManager.CreateReservationsForColumn(d, currentProperty, isForward);
        reservationManager.DisableUnseenReservations();

        //Update the top calndar FocalDate
        topCalendar.UpdateMonth(dayColumns.Find(c => c.transform.GetSiblingIndex() == FocalDayColumnIndex).ObjectDate);
    }

    //TODO: Replace debug logs with fuctions from reservation edit/new screen
    //Create enough day headers and day columns to cover the screen in landscape mode
    private void CreateDayItems()
    {
        float maxScreen = Mathf.Max(Screen.width, Screen.height);
        totalItemCount = Mathf.RoundToInt((maxScreen * 1.3f)/dayHeaderPrefab.GetComponent<RectTransform>().rect.width);

        for (int d = 0; d < totalItemCount; d++)
        {
            //Create day header
            CalendarDayHeaderObject header = Instantiate(dayHeaderPrefab, dayHeaderContent).GetComponent<CalendarDayHeaderObject>();
            header.name = $"Day header {d}";
            header.UpdateDayObject(DateTime.Today.Date.AddDays(d), currentProperty, (a,l) => Debug.Log($"Making reservation for day {a.ToString(Constants.DateTimePrintFormat)} property {l.Name}"));
            dayHeaders.Add(header);

            //Create day columns
            CalendarDayColumn dayColumn = Instantiate(dayColumnPrefab, dayColumnScrollrect.content).GetComponent<CalendarDayColumn>();
            dayColumn.name = $"Day column {d}";
            dayColumn.Initialize(DateTime.Today.Date.AddDays(d), new List<IRoom>(), (a,l) => Debug.Log($"Making reservation for day {a.ToString(Constants.DateTimePrintFormat)} property {l.Name}"), header);
            dayColumns.Add(dayColumn);
        }

        focalDate = dayColumns[0].ObjectDate;
        focalDayColumn = dayColumns[0];

        LayoutRebuilder.ForceRebuildLayoutImmediate(dayColumnScrollrect.content);
    }
}
