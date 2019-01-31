using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarDayItem : MonoBehaviour
{
    [SerializeField]
    private Image showIsTodayImage = null;
    [SerializeField]
    private Text dayText = null;
    [SerializeField]
    private Image dayReservationStatusImage;

    private Image dayItemImage;
    private Color dayItemImageColor;
    private Color dayReservationStatusImageColor;
    private Button dayItemButton;
    private DateTime dayItemDateTime;
    private List<IReservation> currentDayReservations;
    private List<IRoom> filteredRooms;

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
        dayItemImage.color = isSelectedMonth ? dayItemImageColor : Color.gray;
        showIsTodayImage.gameObject.SetActive(dateTime == DateTime.Today);
        dayText.text = dateTime.Day.ToString();
        ShowDayItemStatus(filteredRooms);
    }

    private void ShowDayItemStatus(List<IRoom> rooms)
    {
        filteredRooms = rooms;
        currentDayReservations = new List<IReservation>();
        foreach (var reservation in ReservationDataManager.GetReservations())
        {
            bool isDayReserved = dayItemDateTime >= reservation.Period.Start && dayItemDateTime <= reservation.Period.End;

            if (isDayReserved)
            {
                currentDayReservations.Add(reservation);
            }
        }

        if (GetReservedRoomsInCurrentDay().Count == 0)
        {
            dayReservationStatusImage.color = dayReservationStatusImageColor;
        }
        else if (GetReservedRoomsInCurrentDay().Count < filteredRooms.Count)
        {
            dayReservationStatusImage.color = Color.yellow;
        }
        else
        {
            dayReservationStatusImage.color = Color.red;
        }
    }

    private List<IRoom> GetReservedRoomsInCurrentDay()
    {
        List<IRoom> reservedRoomsInCurrentDay = filteredRooms.FindAll(room =>
        {
            List<IReservation> reservationsForThisRoom = currentDayReservations.FindAll(reservation => reservation.RoomID == room.ID);
            return reservationsForThisRoom.Exists(reservation =>
            {
                bool reservationIsForCurrentDay = dayItemDateTime >= reservation.Period.Start && dayItemDateTime <= reservation.Period.End;
                return reservationIsForCurrentDay;
            });
        });
        return reservedRoomsInCurrentDay;
    }
}
