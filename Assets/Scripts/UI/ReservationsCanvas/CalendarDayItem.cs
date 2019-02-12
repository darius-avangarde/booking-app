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

    public void Initialize(Action<DateTime, List<IRoom>> callback)
    {
        dayItemButton.onClick.AddListener(() => callback(dayItemDateTime, GetReservedRoomsInCurrentDay()));
    }

    public void UpdateDayItem(DateTime dateTime, bool isSelectedMonth, List<IRoom> filteredRooms)
    {
        dayItemDateTime = dateTime;
        dayItemImage.color = isSelectedMonth ? dayItemImageColor : Constants.unavailableItemColor;
        todayImage.gameObject.SetActive(dateTime == DateTime.Today);
        dayText.text = dateTime.Day.ToString();
        ShowDayItemStatus(filteredRooms);
    }

    private void ShowDayItemStatus(List<IRoom> rooms)
    {
        filteredRoomList = rooms;
        currentDayReservationList = new List<IReservation>();
        foreach (var reservation in ReservationDataManager.GetReservations())
        {
            bool isDayReserved = dayItemDateTime >= reservation.Period.Start && dayItemDateTime <= reservation.Period.End;

            if (isDayReserved)
            {
                currentDayReservationList.Add(reservation);
            }
        }

        if (GetReservedRoomsInCurrentDay().Count == 0)
        {
            dayReservationStatusImage.color = dayReservationStatusImageColor;
        }
        else if (GetReservedRoomsInCurrentDay().Count < filteredRoomList.Count)
        {
            dayReservationStatusImage.color = Constants.reservedAvailableItemColor;
        }
        else
        {
            dayReservationStatusImage.color = Constants.reservedUnavailableItemColor;
        }
    }

    private List<IRoom> GetReservedRoomsInCurrentDay()
    {
        List<IRoom> reservedRoomsInCurrentDay = filteredRoomList.FindAll(room =>
        {
            List<IReservation> reservationsForThisRoom = currentDayReservationList.FindAll(reservation => reservation.RoomID == room.ID);
            return reservationsForThisRoom.Exists(reservation =>
            {
                bool reservationIsForCurrentDay = dayItemDateTime >= reservation.Period.Start && dayItemDateTime <= reservation.Period.End;
                return reservationIsForCurrentDay;
            });
        });
        return reservedRoomsInCurrentDay;
    }
}
