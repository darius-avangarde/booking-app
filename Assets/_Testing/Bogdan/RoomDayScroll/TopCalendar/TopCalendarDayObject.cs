using System;
using UnityEngine;
using UnityEngine.UI;

public class TopCalendarDayObject : MonoBehaviour
{
    [SerializeField]
    private Text dateText;
    [SerializeField]
    private Button dateButton;


    public void UpdateDayObject(string date, bool isCurrentDay = false)
    {
        dateText.text = date;
        dateButton.targetGraphic.color = isCurrentDay ? Placeholder_ThemeManager.Instance.CalendarCurrentColor : Color.clear;
    }

    public void UpdateDayObject()
    {
        dateText.text = string.Empty;
    }
}
