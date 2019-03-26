using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<IRoom> filteredRooms = new List<IRoom>();
    public List<IRoom> FilteredRooms => filteredRooms;
    private RoomFilter filter = new RoomFilter();

    public void Initialize()
    {
        filterButtonCalendarScreen.UpdateFilterButtonText(filter);
        filterButtonDayScreen.UpdateFilterButtonText(filter);

        var properties = PropertyDataManager.GetProperties();
        var rooms = properties.SelectMany(property => property.Rooms);
        filteredRooms = filter.Apply(rooms.ToList());
        calendar.SetRooms(filteredRooms);
    }

    public void UpdateFilterButtonText()
    {
        filterButtonCalendarScreen.UpdateFilterButtonText(filter);
        filterButtonDayScreen.UpdateFilterButtonText(filter);
    }

    public void ShowFilterDialog()
    {
        modalFilterDialog.Show(filter, (updatedFilter) => {
            var properties = PropertyDataManager.GetProperties();
            var rooms = properties.SelectMany(property => property.Rooms);
            filteredRooms = filter.Apply(rooms.ToList());
            calendar.SetRooms(filteredRooms);
        });
    }

    public List<IRoom> GetReservedRoomsInCurrentDay(DateTime dayItemDateTime)
    {
        var currentDayReservationList = ReservationDataManager.GetReservations()
            .ToList().FindAll(reservation => reservation.Period.Includes(dayItemDateTime));

        return filteredRooms.FindAll(room => currentDayReservationList.Exists(reservation => reservation.RoomID.Equals(room.ID)));
    }
}
