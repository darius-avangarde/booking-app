using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarScreen : MonoBehaviour
{
    [SerializeField]
    private Text filterInfoButtonText = null;
    [SerializeField]
    private Text filterInfoButtonTextInDayScreen = null;
    [HideInInspector]
    public List<IRoom> roomList = new List<IRoom>();
    [SerializeField]
    private CalendarFilterDialog modalCalendarFilterDialog = null;
    private RoomFilter filter = null;
    // Start is called before the first frame update
    void Start()
    {
        filter = new RoomFilter();
        UpdateFilterButtonText();
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            foreach (IRoom room in property.Rooms)
            {
                roomList.Add(room);
            }
        }
    }

    public void ShowModalCalendar()
    {
        modalCalendarFilterDialog.Show(filter, (updatedFilter) => {
            FilterList(updatedFilter);
        });
    }

    public void UpdateFilterButtonText()
    {
        string propertyInfo = "";
        string roomCapacityInfo = "    Persoane:    " + filter.RoomCapacity.ToString();
        string singleBedInfo = "    Paturi single:  " + filter.SingleBeds;
        string doubleBedInfo = "    Paturi duble:   " + filter.DoubleBeds;

        if (!string.IsNullOrEmpty(filter.PropertyID))
        {
            propertyInfo = "Proprietate: " + PropertyDataManager.GetProperty(filter.PropertyID).Name;
        }

        filterInfoButtonText.text = propertyInfo + roomCapacityInfo + singleBedInfo + doubleBedInfo;
        filterInfoButtonTextInDayScreen.text = filterInfoButtonText.text;
    }

    private void FilterList(RoomFilter updatedFilter)
    {
        roomList = new List<IRoom>();
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            foreach (IRoom room in property.Rooms)
            {
                roomList.Add(room);
            }
        }
        roomList = updatedFilter.Apply(roomList);
    }
}
