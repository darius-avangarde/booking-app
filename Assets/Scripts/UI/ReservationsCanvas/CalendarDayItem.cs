using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarDayItem : MonoBehaviour
{
    public Image showIsTodayImage;
    public Text dayText;
    public Image dayReservationStatusText;

    private Image dayItemImage;
    private Color dayItemImageColor;
    private Button dayItemButton;
    private DateTime dayItemDateTime;

    private void Start()
    {
        dayItemImage = GetComponent<Image>();
        dayItemImageColor = dayItemImage.color;
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
    }
}
