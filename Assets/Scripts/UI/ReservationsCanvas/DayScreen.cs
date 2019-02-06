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
    private List<IRoom> reservedRoomList = new List<IRoom>();

    public void UpdateDayScreenInfo(DateTime dateTime, List<IRoom> rooms)
    {
        dayScreenTitle.text = dateTime.Day + " " + Constants.MonthNamesDict[dateTime.Month] + " " + dateTime.Year;
        reservedRoomList = rooms;
        UpdateFilteredDayScreenPropertyItemsContent();
    }

    public void UpdateFilteredDayScreenPropertyItemsContent()
    {
        foreach (var dayScreenItem in dayScreenItemList)
        {
            Destroy(dayScreenItem);
        }

        foreach (IRoom room in calendarScreen.GetFilteredRooms())
        {
            GameObject dayScreenItem = Instantiate(dayScreenItemPrefab, filteredPropertiesContent);
            dayScreenItem.GetComponent<DayScreenPropertyItem>().Initialize(room, reservedRoomList, () => OpenRoomReservationScreen(room));
            dayScreenItemList.Add(dayScreenItem);
        }
    }

    private void OpenRoomReservationScreen(IRoom room)
    {
        roomScreen.GetComponent<RoomScreen>().UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }
}
