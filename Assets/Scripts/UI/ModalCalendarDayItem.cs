using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalCalendarDayItem : MonoBehaviour
{
    public Text dayText;

    private Image modalCalendarDayItemImage;
    private Color modalCalendarDayItemImageColor;
    private Color unavailableColor = Color.gray;
    private Color reservedColor = Color.blue;
    private Button modalCalendarDayItemButton;
    private DateTime modalCalendarDayItemDateTime;
    private bool isReserved = false;

    public void Initialize(Action<DateTime> callback)
    {
        modalCalendarDayItemImage = GetComponent<Image>();
        modalCalendarDayItemButton = GetComponent<Button>();
        modalCalendarDayItemImageColor = modalCalendarDayItemImage.color;
        GetComponent<Button>().onClick.AddListener(() => callback(modalCalendarDayItemDateTime));
        callback += ColorSelectedDayItem;
    }

    public void UpdateModalDayItem(DateTime dateTime, bool isInteractableDay, bool isReservedDay, IReservation currentReservation)
    {
        modalCalendarDayItemDateTime = dateTime;
        modalCalendarDayItemImage.color = isInteractableDay ? modalCalendarDayItemImageColor : unavailableColor;
        modalCalendarDayItemButton.interactable = isInteractableDay;
        isReserved = isReservedDay;
        modalCalendarDayItemImage.color = isReservedDay ? reservedColor : modalCalendarDayItemImage.color;
        dayText.text = dateTime.Day.ToString();
        ShowCurrentReservationReservedPeriod(currentReservation);
    }

    private void ColorSelectedDayItem(DateTime modalCalendarDayItemDateTime)
    {
        modalCalendarDayItemImage.color = modalCalendarDayItemButton.interactable ? Color.cyan : modalCalendarDayItemImageColor;
    }
    
    private void ShowCurrentReservationReservedPeriod(IReservation currentReservation)
    {
        if (currentReservation != null)
        {
            if (modalCalendarDayItemDateTime >= currentReservation.Period.Start && modalCalendarDayItemDateTime <= currentReservation.Period.End)
            {
                modalCalendarDayItemImage.color = Color.red;
            }
        }
    }
}
