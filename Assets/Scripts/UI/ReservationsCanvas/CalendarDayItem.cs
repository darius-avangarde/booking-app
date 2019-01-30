using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarDayItem : MonoBehaviour
{
    [SerializeField]
    private Image showIsTodayImage;
    [SerializeField]
    private Text dayText;
    [SerializeField]
    private Image dayReservationStatusImage;
    [SerializeField]
    private CalendarScreen calendarScreen;

    private Image dayItemImage;
    private Color dayItemImageColor;
    private Color dayReservationStatusImageColor;
    private Button dayItemButton;
    private DateTime dayItemDateTime;
    private List<IReservation> reservationsWithDayInPeriod;

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

    public void UpdateDayItem(DateTime dateTime, bool isSelectedMonth)
    {
        dayItemDateTime = dateTime;
        dayItemImage.color = isSelectedMonth? dayItemImageColor : Color.gray;
        showIsTodayImage.gameObject.SetActive(dateTime == DateTime.Today);
        dayText.text = dateTime.Day.ToString();

        reservationsWithDayInPeriod = new List<IReservation>();

        foreach (var reservation in ReservationDataManager.GetReservations())
        {
            bool isDayReserved = dayItemDateTime >= reservation.Period.Start && dayItemDateTime <= reservation.Period.End;
            bool areFreeRoomsInThisDay = AreFreeRoomsInThisDay();
            if (isDayReserved)
            {
                reservationsWithDayInPeriod.Add(reservation);
            }
            else
            {
                dayReservationStatusImage.color = dayReservationStatusImageColor;
            }
        }

        foreach (var item in reservationsWithDayInPeriod)
        {
            dayReservationStatusImage.color = Color.red;
        }
    }

    private bool AreFreeRoomsInThisDay()
    {
        return true;
    }

}
