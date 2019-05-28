using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void UpdateFilteredDayScreenPropertyItemsContent()
    {
        foreach (var dayScreenItem in dayScreenItemList)
        {
            Destroy(dayScreenItem);
        }

        var reservations = ReservationDataManager.GetReservations()
            .ToList().FindAll(reservation => reservation.Period.Includes(dayDateTime));

        foreach (IRoom room in calendarScreen.FilteredRooms)
        {
            GameObject dayScreenItem = Instantiate(dayScreenItemPrefab, filteredPropertiesContent);
            bool isReserved = reservations.Exists(reservation => room.ID.Equals(reservation.RoomID));
            dayScreenItem.GetComponent<DayScreenPropertyItem>()
                .Initialize(room, isReserved, () => OpenRoomReservationScreen(room));
            dayScreenItemList.Add(dayScreenItem);
        }
    }

    private void OpenRoomReservationScreen(IRoom room)
    {
        //roomScreen.GetComponent<RoomScreen>().UpdateRoomDetailsFields(dayDateTime, room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }
}
