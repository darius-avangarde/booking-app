﻿using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class DayScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private CalendarScreen calendarScreen = null;
    [SerializeField]
    private Transform filteredPropertiesContent = null;
    [SerializeField]
    private GameObject dayScreenItemPrefab = null;
    [SerializeField]
    private Transform roomScreen = null;
    [SerializeField]
    private Text dayScreenTitle = null;
    private List<GameObject> dayScreenItems = new List<GameObject>();
    
    public void UpdateDayScreenInfo(DateTime dateTime)
    {
        dayScreenTitle.text = dateTime.Day + " " + Constants.monthNamesDict[dateTime.Month] + " " + dateTime.Year;
        UpdateFilteredPropertiesContent();
    }

    public void UpdateFilteredPropertiesContent()
    {
        foreach (var dayScreenItem in dayScreenItems)
        {
            Destroy(dayScreenItem);
        }

        foreach (IRoom room in calendarScreen.roomList)
        {
            GameObject dayScreenItem = Instantiate(dayScreenItemPrefab, filteredPropertiesContent);
            dayScreenItem.GetComponent<DayScreenPropertyItem>().Initialize(room, () => OpenRoomReservationScreen(room));
            dayScreenItems.Add(dayScreenItem);
        }
    }

    private void OpenRoomReservationScreen(IRoom room)
    {
        roomScreen.GetComponent<RoomScreen>().UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }
}
