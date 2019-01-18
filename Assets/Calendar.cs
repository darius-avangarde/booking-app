using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : MonoBehaviour
{
    [SerializeField]
    private Transform dayItemsInCalendarPanel = null;
    private DateTime dateTime;
    private DateTime now = DateTime.Now;
    // Start is called before the first frame update
    void Start()
    {
        int d = 1;
        DateTime firstDay = new DateTime(now.Year, now.Month, 1);
        for (int i = GetDay(firstDay.DayOfWeek); i <= DateTime.DaysInMonth(now.Year, now.Month) + 1; i++)
        {
            dayItemsInCalendarPanel.GetChild(i).GetComponent<DayItem>().dayText.text = (d++).ToString();
        }
    }
    
    private void UpdateCalendar()
    {
        
    }

    private int GetDay(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
            case DayOfWeek.Sunday: return 0;
        }

        return 0;
    }
}
