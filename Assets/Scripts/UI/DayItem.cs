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

    public void UpdateDayItem(DateTime dateTime, bool interactableState)
    {
        GetComponent<Button>().interactable = interactableState;
        showIsTodayImage.gameObject.SetActive(false);

        if (dateTime == DateTime.Today)
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
