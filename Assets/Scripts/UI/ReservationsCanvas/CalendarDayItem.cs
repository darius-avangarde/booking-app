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
    private Image dayReservationStatusImage;

    private Image dayItemImage;
    private Color dayItemImageColor;
    private Color dayReservationStatusImageColor;
    private Button dayItemButton;
    private DateTime dayItemDateTime;
    private List<IReservation> currentDayReservationList;
    private List<IRoom> filteredRoomList;

    private void Start()
    {
        dayItemImage = GetComponent<Image>();
        dayItemImageColor = dayItemImage.color;
        dayReservationStatusImage = dayReservationStatusImage.GetComponent<Image>();
        dayReservationStatusImageColor = dayReservationStatusImage.color;
        dayItemButton = GetComponent<Button>();
    }

    public void Initialize(Action<DateTime> callback)
    {
        dayItemButton.onClick.AddListener(() => callback(dayItemDateTime));
    }

    public void UpdateDayItem(DateTime dateTime, bool isSelectedMonth, List<IRoom> filteredRooms, List<IRoom> reservedRoomsInCurrentDay)
    {
        dayItemDateTime = dateTime;
        dayItemImage.color = isSelectedMonth ? dayItemImageColor : Constants.unavailableItemColor;
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
            dayReservationStatusImage.color = dayReservationStatusImageColor;
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
