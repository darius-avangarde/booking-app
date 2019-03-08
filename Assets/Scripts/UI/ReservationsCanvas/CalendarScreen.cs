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
    private FilterDialog modalFilterDialog = null;

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

    // TODO: it would be better to do all the initialization for our screen in a method subscribed to NavScreen.Showing
    // Start is run only once, but NavScreen.Showing is raised each time the user navigates to the screen
    // this way can make sure that we have the latest data
    void Start()
    {
        UpdateFilterButtonText();
    }

    // TODO: DayScreen is coupled to CalendarScreen. It should not depend on it for data
    // one way to decouple them is to have each perform calculations separately to obtain derived data.
    // Because the operations can be expensive and we may want to cache data we can move the calculations and caching to the data layer
    // but the first step should be to decouple these two screen.
    // We should probably discuss this further when we refactor this code.
    public List<IRoom> GetFilteredRooms()
    {
        return filteredRoomList;
    }

    public void ShowFilterDialog()
    {
        // TODO: we can replace the annonymous function with FilterList directly, since FilterList matches Action<RoomFilter>
        modalFilterDialog.Show(filter, (updatedFilter) => {
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

    // TODO: this doesn't seem to be accurately named, it gets all the rooms
    // it is also called from Calendar, this code could reasonably be moved into Calendar, without needing to have it called from here
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
        // TODO: Calendar.UpdateCalendarAfterFiltering gets the filtered rooms from CalendarScreen
        // since the room list is actually created here here it would be simpler to have a method like Calendar.SetRooms(List<IRoom>)
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
            // TODO: this check seems unnecessary since currentDayReservationList is already filtered to containe only reservations for the selected day
            return reservationsForThisRoom.Exists(reservation =>
            {
                bool reservationIsForCurrentDay = dayItemDateTime >= reservation.Period.Start && dayItemDateTime <= reservation.Period.End;
                return reservationIsForCurrentDay;
            });
        });
        return reservedRoomsInCurrentDay;
    }
}
