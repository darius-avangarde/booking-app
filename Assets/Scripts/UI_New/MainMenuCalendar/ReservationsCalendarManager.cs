using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ReservationsCalendarManager : MonoBehaviour
{
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
    [SerializeField]
    private ReservationFilterScreen filterScreen;
    [SerializeField]
    private ReservationOptionsDropdown reservationOptions;
    [SerializeField]
    private RoomAdminScreen roomEditScreen;

    [SerializeField]
    private ReservationEditScreen_New reservationEditScreen;

    [SerializeField]
    private Text noFilterMatchText;
    [SerializeField]
    private GameObject noFilterMatchTextParent;

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
    public int FocalDayColumnIndex => dayColumns.Count(d => d.transform.position.x < dayColumnScrollrect.viewport.position.x - 5);

    private static DateTime focalDate;

    private CalendarDayColumn focalDayColumn;
    private bool infiniteScrollsInitialized = false;
    private ReservationFilter currentFilter = null;


     private void Start()
    {
        propertyDropdown.OnSelectProperty = SelectProperty;

        dayColumnInfScroll.onMoveItem = UpdateDayColumnDate;
        CreateDayItems();
        LayoutRebuilder.ForceRebuildLayoutImmediate(dayColumnScrollrect.content);

        //Load first property to initialize day columns/items;
        SelectProperty(PropertyDataManager.GetProperties().ToList()[0]);

        //Infinite scrollrects need to initialize after the day column items/ day header items are spawned
        StartCoroutine(DelayInfiniteScrollrectInitialization());
    }

    private IEnumerator DelayInfiniteScrollrectInitialization()
    {
        yield return null;
        dayHeaderInfScroll.Init();
        dayColumnInfScroll.Init();
    }

    #region FilerActions
        public void OpenFilterScreen()
        {
            filterScreen.OpenFilterScreen(ApplyFilers, currentFilter);
        }

        public void ApplyFilers(ReservationFilter filter)
        {
            currentFilter = filter;
            SelectProperty(currentProperty);
        }

    #endregion



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

        reservationManager.SweepUpdateReservations(currentProperty, reservationOptions.OpenReservationMenu);

        topCalendar.CloseDropdownCalendar();
    }

    private List<IRoom> FilteredRooms()
    {
        if(currentFilter != null)
        {
            if(currentFilter.Client == null)
            {
                IEnumerable<IRoom> filteredRooms = currentProperty.Rooms;

                if(currentFilter.RoomBeds.HasValue)
                {
                    filteredRooms = filteredRooms.Where(r => r.SingleBeds == currentFilter.RoomBeds.Value.x && r.DoubleBeds == currentFilter.RoomBeds.Value.y);
                }
                if(currentFilter.RoomType != null)
                {
                    filteredRooms = filteredRooms.Where(r => r.RoomType == currentFilter.RoomType);
                }
                if(currentFilter.StartDate.HasValue)
                {
                    filteredRooms = ReservationDataManager.GetFreeRoomsInInterval(filteredRooms, currentFilter.StartDate.Value, currentFilter.EndDate.Value);
                    JumpToDate(currentFilter.StartDate.Value);
                }

                ShowNoFilterMatchText(currentRooms.Count == 0 ? "Nu există camere care să îndeplinească filtrele aplicate" : null);
                currentRooms = filteredRooms.ToList();
            }
            else
            {
                if(ReservationDataManager.GetActiveClientReservations(currentFilter.Client.ID).Where(r => r.PropertyID == currentProperty.ID).ToList().Count > 0)
                {
                    JumpToDate(ReservationDataManager.GetActiveClientReservations(currentFilter.Client.ID).Where(r => r.PropertyID == currentProperty.ID).ToList()[0].Period.Start.Date);
                }
                else
                {
                    ShowNoFilterMatchText("Clientul selectat nu are rezervari pe această proprietate");
                }
                currentRooms = currentProperty.Rooms.ToList();
            }
        }
        else
        {
            currentRooms = currentProperty.Rooms.ToList();
            ShowNoFilterMatchText(currentRooms.Count == 0 ? "Nu există camere care să îndeplinească filtrele aplicate" : null);
        }

        return currentRooms;
    }

    public void SelectProperty(IProperty property)
    {
        currentProperty = property;
        currentRooms = FilteredRooms();
        roomColumn.UpdateRooms(currentRooms, roomEditScreen.OpenRoomAdminScreen);

        foreach(CalendarDayColumn dayColumn in dayColumns)
        {
            dayColumn.UpdateRooms(currentRooms, NewReservationFromUnreservedDay);
            dayColumn.LinkedHeader.UpdateProperty(currentProperty);
        }

        //Resize the day column content rect size to fit the number of rooms
        dayColumnScrollrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentRooms.Count * dayColumnObjectTransform.rect.height);

        reservationManager.SweepUpdateReservations(currentProperty, reservationOptions.OpenReservationMenu);
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

    //Create enough day headers and day columns to cover the screen in landscape mode
    private void CreateDayItems()
    {
        // float maxScreen = Mathf.Max(Screen.width, Screen.height);
        // totalItemCount = Mathf.RoundToInt((maxScreen * 1.3f)/dayHeaderPrefab.GetComponent<RectTransform>().rect.width);
        // Debug.Log(totalItemCount);
        totalItemCount = 19;

        for (int d = 0; d < totalItemCount; d++)
        {
            //Create day header
            CalendarDayHeaderObject header = Instantiate(dayHeaderPrefab, dayHeaderContent).GetComponent<CalendarDayHeaderObject>();
            header.name = $"Day header {d}";
            header.UpdateDayObject(DateTime.Today.Date.AddDays(d), currentProperty, NewReservationFromHeader);
            dayHeaders.Add(header);

            //Create day columns
            CalendarDayColumn dayColumn = Instantiate(dayColumnPrefab, dayColumnScrollrect.content).GetComponent<CalendarDayColumn>();
            dayColumn.name = $"Day column {d}";
            dayColumn.Initialize(DateTime.Today.Date.AddDays(d), new List<IRoom>(), NewReservationFromUnreservedDay, header);
            dayColumns.Add(dayColumn);
        }

        focalDate = dayColumns[0].ObjectDate;
        focalDayColumn = dayColumns[0];

        LayoutRebuilder.ForceRebuildLayoutImmediate(dayColumnScrollrect.content);
    }

    private void ShowNoFilterMatchText(string message)
    {
        if(string.IsNullOrEmpty(message))
        {
            noFilterMatchTextParent.SetActive(false);
        }
        else
        {
            noFilterMatchText.text = message;
            noFilterMatchTextParent.SetActive(true);
        }
    }


    private void EditReservation(IReservation reservation)
    {
        if(reservation.Period.End > DateTime.Today.Date)
            reservationEditScreen.OpenEditScreen((r) => JumpToDate(r.Period.Start.Date), reservation);
    }

    private void NewReservationFromHeader(DateTime date, IProperty property)
    {
        reservationEditScreen.OpenEditScreen((r) => JumpToDate(r.Period.Start.Date), property, date);
    }

    private void NewReservationFromRoomColumn(IRoom room)
    {
        reservationEditScreen.OpenEditScreen((r) => JumpToDate(r.Period.Start.Date), currentProperty, room);
    }

    private void NewReservationFromUnreservedDay(DateTime date, IRoom room)
    {
        reservationEditScreen.OpenEditScreen((r) => JumpToDate(r.Period.Start.Date), currentProperty, room, date);
    }

}
