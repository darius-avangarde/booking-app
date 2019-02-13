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
    private CalendarFilterDialog modalCalendarFilterDialog = null;

    [Header("Filter Button Text Components In Calendar Screen")]
    [SerializeField]
    private Text propertyInfoText = null;
    [SerializeField]
    private Text roomCapacityText = null;
    [SerializeField]
    private Text singleBedText = null;
    [SerializeField]
    private Text doubleBedText = null;

    [Header("Filter Button Text Components In Day Screen")]
    [SerializeField]
    private Text propertyInfoDayScreenText = null;
    [SerializeField]
    private Text roomCapacityDayScreenText = null;
    [SerializeField]
    private Text singleBedDayScreenText = null;
    [SerializeField]
    private Text doubleBedDayScreenText = null;

    private List<IRoom> filteredRoomList = new List<IRoom>();
    private RoomFilter filter = new RoomFilter();

    // Start is called before the first frame update
    void Start()
    {
        UpdateFilterButtonText();
    }

    public List<IRoom> GetFilteredRooms()
    {
        return filteredRoomList;
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
        string roomCapacityInfo = Constants.Persoane + filter.RoomCapacity.ToString();
        string singleBedInfo = Constants.SingleBed + filter.SingleBeds;
        string doubleBedInfo = Constants.DoubleBed + filter.DoubleBeds;

        if (!string.IsNullOrEmpty(filter.PropertyID))
        {
            propertyInfo = Constants.Proprietate + PropertyDataManager.GetProperty(filter.PropertyID).Name;
        }

        propertyInfoText.text = propertyInfo;
        roomCapacityText.text = roomCapacityInfo;
        singleBedText.text = singleBedInfo;
        doubleBedText.text = doubleBedInfo;

        propertyInfoDayScreenText.text = propertyInfo;
        roomCapacityDayScreenText.text = roomCapacityInfo;
        singleBedDayScreenText.text = singleBedInfo;
        doubleBedDayScreenText.text = doubleBedInfo;
    }

    public List<IRoom> GetRoomsInFilteredRoomsList()
    {
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            filteredRoomList.AddRange(property.Rooms);
        }
        return filteredRoomList;
    }

    private void FilterList(RoomFilter updatedFilter)
    {
        filteredRoomList = new List<IRoom>();
        foreach (IProperty property in PropertyDataManager.GetProperties())
        {
            filteredRoomList.AddRange(property.Rooms);
        }
        filteredRoomList = updatedFilter.Apply(filteredRoomList);
        calendar.UpdateCalendarAfterFiltering();
    }

    public List<IRoom> GetReservedRoomsInCurrentDay(DateTime dayItemDateTime)
    {
        List<IReservation> currentDayReservationList = new List<IReservation>();
        foreach (var reservation in ReservationDataManager.GetReservations())
        {
            bool isDayReserved = dayItemDateTime >= reservation.Period.Start && dayItemDateTime <= reservation.Period.End;

            if (isDayReserved)
            {
                currentDayReservationList.Add(reservation);
            }
        }

        List<IRoom> reservedRoomsInCurrentDay = filteredRoomList.FindAll(room =>
        {
            List<IReservation> reservationsForThisRoom = currentDayReservationList.FindAll(reservation => reservation.RoomID == room.ID);
            return reservationsForThisRoom.Exists(reservation =>
            {
                bool reservationIsForCurrentDay = dayItemDateTime >= reservation.Period.Start && dayItemDateTime <= reservation.Period.End;
                return reservationIsForCurrentDay;
            });
        });
        return reservedRoomsInCurrentDay;
    }
}
