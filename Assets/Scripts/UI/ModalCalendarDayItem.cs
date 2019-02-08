using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalCalendarDayItem : MonoBehaviour
{
    [SerializeField]
    private Text dayText = null;

    private Image modalCalendarDayItemImage;
    private Color modalCalendarDayItemImageColor;
    private Color reservedItemColor = Color.blue;
    private Button modalCalendarDayItemButton;
    private DateTime modalCalendarItemDateTime;
    private bool isReserved = false;
    private bool isReservedDayAvailable = false;
    private IReservation currentReservation;

    public void Initialize(Action<DateTime,Image, bool, bool> callback)
    {
        modalCalendarDayItemImage = GetComponent<Image>();
        modalCalendarDayItemButton = GetComponent<Button>();
        modalCalendarDayItemImageColor = modalCalendarDayItemImage.color;
        GetComponent<Button>().onClick.AddListener(() => 
        callback(modalCalendarItemDateTime, modalCalendarDayItemImage, isReserved, isReservedDayAvailable));
    }

    public void UpdateModalDayItem(DateTime dateTime, IReservation reservation, bool isAvailableDay, bool isReservedDay, bool isReservedAvailable)
    {
        SetLocalFields(dateTime, reservation, isReservedDay, isReservedAvailable);

        modalCalendarDayItemImage.color = isAvailableDay ? modalCalendarDayItemImageColor : Constants.unavailableItemColor;
        modalCalendarDayItemButton.interactable = isAvailableDay;
        modalCalendarDayItemImage.color = isReservedDay ? Constants.reservedUnavailableItemColor : modalCalendarDayItemImage.color;
        ShowCurrentReservationReservedPeriod();
    }

    private void SetLocalFields(DateTime dateTime, IReservation reservation, bool isReservedDay, bool isReservedAvailable)
    {
        currentReservation = reservation;
        isReserved = isReservedDay;
        isReservedDayAvailable = isReservedAvailable;
        modalCalendarItemDateTime = dateTime;
        dayText.text = dateTime.Day.ToString();
    }
    
    private void ShowCurrentReservationReservedPeriod()
    {
        if (currentReservation != null)
        {
            bool isDateTimeItemInReservationPeriod = modalCalendarItemDateTime >= currentReservation.Period.Start 
                                                     && modalCalendarItemDateTime <= currentReservation.Period.End;
            if (isDateTimeItemInReservationPeriod)
            {
                modalCalendarDayItemImage.color = reservedItemColor;
            }
        }
    }
}
