using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarDayItem : MonoBehaviour
{
    [SerializeField]
    private Image todayImage = null;
    [SerializeField]
    private Text dayText = null;
    [SerializeField]
    private Image dayReservationStatusImage = null;

    private DateTime dayItemDateTime;
    private List<IReservation> currentDayReservationList;
    private List<IRoom> filteredRoomList;

    public void AddListener(Action<DateTime> callback)
    {
        GetComponent<Button>().onClick.AddListener(() => callback(dayItemDateTime));
    }

    public void UpdateDayItem(DateTime dateTime, bool isSelectedMonth, List<IRoom> filteredRooms, List<IRoom> reservedRoomsInCurrentDay)
    {
        dayItemDateTime = dateTime;
        dayText.color = isSelectedMonth ? Constants.darkTextColor : Constants.lightTextColor;
        int alpha = dateTime == DateTime.Today ? 1 : 0;
        todayImage.color = new Color(todayImage.color.r, todayImage.color.g, todayImage.color.b, alpha);
        dayText.text = dateTime.Day.ToString();
        ShowDayItemStatus(filteredRooms, reservedRoomsInCurrentDay);
    }

    private void ShowDayItemStatus(List<IRoom> filteredRooms, List<IRoom> reservedRoomsInCurrentDay)
    {
        filteredRoomList = filteredRooms;

        if (reservedRoomsInCurrentDay.Count == 0)
        {
            dayReservationStatusImage.color = Constants.availableItemColor;
        }
        else if (reservedRoomsInCurrentDay.Count < filteredRoomList.Count)
        {
            dayReservationStatusImage.color = Constants.reservedAvailableItemColor;
        }
        else
        {
            dayReservationStatusImage.color = Constants.reservedUnavailableItemColor;
        }
    }
}
