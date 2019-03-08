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
        // TODO: if we set dayItemImage and dayItemButton in the inspector it will be cleaner (references to components are all in one place, near declaration)
        // and we can get rid of initialization in Start()
        dayItemImage = GetComponent<Image>();
        // TODO: can we simply specify the color instead of getting it from the image? maybe have it be editable in the inspector
        dayItemImageColor = dayItemImage.color;
        // TODO: this seems unnecessary
        dayReservationStatusImage = dayReservationStatusImage.GetComponent<Image>();
        dayReservationStatusImageColor = dayReservationStatusImage.color;
        dayItemButton = GetComponent<Button>();
    }

    public void Initialize(Action<DateTime> callback)
    {
        dayItemButton.onClick.AddListener(() => callback(dayItemDateTime));
    }

    // TODO: we could merge the method above and this one so there is only one place CalendarDayItem is initialized
    // there is similar initialization logic in other button scripts, like RoomItem/RoomButton, DayScreenPropertyItem
    // we just need to take extra care with the onClick listeners
    public void UpdateDayItem(DateTime dateTime, bool isSelectedMonth, List<IRoom> filteredRooms, List<IRoom> reservedRoomsInCurrentDay)
    {
        dayItemDateTime = dateTime;
        dayItemImage.color = isSelectedMonth ? dayItemImageColor : Constants.unavailableItemColor;
        todayImage.gameObject.SetActive(dateTime == DateTime.Today);
        dayText.text = dateTime.Day.ToString();
        ShowDayItemStatus(filteredRooms, reservedRoomsInCurrentDay);
    }

    private void ShowDayItemStatus(List<IRoom> filteredRooms, List<IRoom> reservedRoomsInCurrentDay)
    {
        // TODO: we don't seem to be using filteredRoomList for anything else, can we remove it and just use filteredRooms instead?
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
