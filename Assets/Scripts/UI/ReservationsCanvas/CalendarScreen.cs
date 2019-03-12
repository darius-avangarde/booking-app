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
    [SerializeField]
    private FilterButton filterButtonCalendarScreen = null;
    [SerializeField]
    private FilterButton filterButtonDayScreen = null;

    private List<IRoom> filteredRoomList = new List<IRoom>();
    private RoomFilter filter = new RoomFilter();

    // Start is called before the first frame update
    void Start()
    {
        filterButtonCalendarScreen.UpdateFilterButtonText(filter);
        filterButtonDayScreen.UpdateFilterButtonText(filter);
    }

    public void UpdateFilterButtonText()
    {
        filterButtonCalendarScreen.UpdateFilterButtonText(filter);
        filterButtonDayScreen.UpdateFilterButtonText(filter);
    }

    public List<IRoom> GetFilteredRooms()
    {
        return filteredRoomList;
    }

    public void ShowFilterDialog()
    {
        modalFilterDialog.Show(filter, (updatedFilter) => {
            FilterList(updatedFilter);
        });
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
