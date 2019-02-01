using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarScreen : MonoBehaviour
{
    [SerializeField]
    private Calendar calendar = null;
    [SerializeField]
    private Text filterInfoButtonText = null;
    [SerializeField]
    private Text filterInfoButtonTextInDayScreen = null;
    [HideInInspector]
    public List<IRoom> filteredRooms = new List<IRoom>();
    [SerializeField]
    private CalendarFilterDialog modalCalendarFilterDialog = null;
    private RoomFilter filter = new RoomFilter();

    // Start is called before the first frame update
    void Start()
    {
        UpdateFilterButtonText();
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
        string singleBedInfo = Constants.SingleBed + filter.SingleBeds;
        string doubleBedInfo = Constants.DoubleBed + filter.DoubleBeds;

        if (!string.IsNullOrEmpty(filter.PropertyID))
        {
            propertyInfo = "Proprietate: " + PropertyDataManager.GetProperty(filter.PropertyID).Name;
        }

        filterInfoButtonText.text = propertyInfo + roomCapacityInfo + singleBedInfo + doubleBedInfo;
        filterInfoButtonTextInDayScreen.text = filterInfoButtonText.text;
    }

    public List<IRoom> GetRoomsInFilteredRoomsList()
    {
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            foreach (IRoom room in property.Rooms)
            {
                filteredRooms.Add(room);
            }
        }
        return filteredRooms;
    }

    private void FilterList(RoomFilter updatedFilter)
    {
        filteredRooms = new List<IRoom>();
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            foreach (IRoom room in property.Rooms)
            {
                filteredRooms.Add(room);
            }
        }
        filteredRooms = updatedFilter.Apply(filteredRooms);
        calendar.UpdateCalendarAfterFiltering();
    }
}
