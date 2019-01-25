using System;
using UnityEngine;
using UnityEngine.UI;

public class ModalCalendarDayItem : MonoBehaviour
{
    public Text dayText;

    private Image modalCalendarDayItemImage;
    private Color modalCalendarDayItemImageColor;
    private Button modalCalendarDayItemButton;
    private DateTime modalCalendarDayItemDateTime;

    public void Initialize(Action<DateTime> callback)
    {
        modalCalendarDayItemImage = GetComponent<Image>();
        modalCalendarDayItemButton = GetComponent<Button>();
        modalCalendarDayItemImageColor = modalCalendarDayItemImage.color;
        GetComponent<Button>().onClick.AddListener(() => callback(modalCalendarDayItemDateTime));
    }

    public void UpdateModalDayItem(DateTime dateTime, bool isInteractableDay)
    {
        modalCalendarDayItemDateTime = dateTime;
        modalCalendarDayItemImage.color = isInteractableDay ? modalCalendarDayItemImageColor : Color.gray;
        modalCalendarDayItemButton.interactable = isInteractableDay;
        dayText.text = dateTime.Day.ToString();
    }
}
