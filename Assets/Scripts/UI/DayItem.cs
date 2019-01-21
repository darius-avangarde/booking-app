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
    public DateTime dayItemDateTime;

    public void UpdateDayItem(DateTime dateTime)
    {
        showIsTodayImage.gameObject.SetActive(false);

        if (dateTime.Day == DateTime.Now.Day && dateTime.Month == DateTime.Now.Month && dateTime.Year == DateTime.Now.Year)
        {
            showIsTodayImage.gameObject.SetActive(true);
        }
        dayItemDateTime = dateTime;
        dayText.text = dateTime.Day.ToString();
    }
    public void Clear()
    {
        dayText.text = "*";
    }
}
