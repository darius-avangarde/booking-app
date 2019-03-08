using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class DayScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private CalendarScreen calendarScreen = null;
    [SerializeField]
    private Transform filteredPropertiesContent = null;
    [SerializeField]
    private GameObject dayScreenItemPrefab = null;
    [SerializeField]
    private Transform roomScreen = null;
    [SerializeField]
    private Text dayScreenTitle = null;
    private List<GameObject> dayScreenItemList = new List<GameObject>();
    private DateTime dayDateTime;

    public void UpdateDayScreenInfo(DateTime dateTime)
    {
        dayDateTime = dateTime;
        dayScreenTitle.text = dateTime.Day + " " + Constants.MonthNamesDict[dateTime.Month] + " " + dateTime.Year;
        UpdateFilteredDayScreenPropertyItemsContent();
    }

    // TODO: ideally DayScreen would initialize itself on NavScreen.Showing
    public void UpdateFilteredDayScreenPropertyItemsContent()
    {
        foreach (var dayScreenItem in dayScreenItemList)
        {
            Destroy(dayScreenItem);
        }

        // TODO: DayScreen is coupled to CalendarScreen. It should not depend on it for data
        // one way to decouple them is to have each perform calculations separately to obtain derived data.
        // Because the operations can be expensive and we may want to cache data we can move the calculations and caching to the data layer
        // but the first step should be to decouple these two screen.
        // We should probably discuss this further when we refactor this code.
        foreach (IRoom room in calendarScreen.GetFilteredRooms())
        {
            GameObject dayScreenItem = Instantiate(dayScreenItemPrefab, filteredPropertiesContent);
            // TODO: we can pass OpenRoomReservationScreen directly instead of creating an anonymous function
            dayScreenItem.GetComponent<DayScreenPropertyItem>()
                .Initialize(room, calendarScreen.GetReservedRoomsInCurrentDay(dayDateTime), () => OpenRoomReservationScreen(room));
            dayScreenItemList.Add(dayScreenItem);
        }
    }

    private void OpenRoomReservationScreen(IRoom room)
    {
        roomScreen.GetComponent<RoomScreen>().UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }
}
