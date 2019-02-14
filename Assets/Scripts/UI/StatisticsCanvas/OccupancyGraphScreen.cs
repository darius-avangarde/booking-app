using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OccupancyGraphScreen : MonoBehaviour
{
    [SerializeField]
    private ModalCalendarStatistics modalCalendarStatisticsDialog = null;
    [SerializeField]
    private Text statisticsPeriodText = null;

    public void ShowModalCalendar()
    {
        modalCalendarStatisticsDialog.Show(ShowUpdatedStatisticsPeriod);
    }
    
    private void ShowUpdatedStatisticsPeriod(DateTime startDateTime, DateTime endDateTime)
    {
        statisticsPeriodText.text = startDateTime.ToString(Constants.DateTimePrintFormat)
                                   + Constants.AndDelimiter
                                   + endDateTime.ToString(Constants.DateTimePrintFormat);
    }
}
