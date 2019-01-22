using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayItem : MonoBehaviour
{
    public Image showIsTodayImage;
    public Text dayText;
    public Image DayReservationStatusText;

    private Image dayItemImage;
    private Color dayItemImageColor;

    private void Start()
    {
        dayItemImage = GetComponent<Image>();
        dayItemImageColor = dayItemImage.color;
    }

    public void UpdateDayItem(DateTime dateTime, bool isSelectedMonth)
    {
        dayItemImage.color = isSelectedMonth? dayItemImageColor : Color.gray;
        
        showIsTodayImage.gameObject.SetActive(dateTime == DateTime.Today);
        dayText.text = dateTime.Day.ToString();
    }
}
