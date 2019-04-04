using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphComponent : MonoBehaviour
{
    [SerializeField]
    private Graph graph = null;

    private void Start()
    {
        graph.XAxisLabel = Constants.XAxisDict[0];
        graph.YAxisLabel = "Procente";
    }

    public void UpdateGraph(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList, int xAxisTypeValueIndex)
    {
        if (startDateTime >= endDateTime)
        {
            Debug.LogError("Start date should be before end date.");
            return;
        }

        graph.XAxisLabel = Constants.XAxisDict[xAxisTypeValueIndex];
        switch (xAxisTypeValueIndex)
        {
            case 0:
                ShowGraphWithXAxisTime(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            case 1:
                ShowGraphWithXAxisLocation(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            case 2:
                ShowGraphWithXAxisRoomCategoryByPersons(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            case 3:
                ShowGraphWithXAxisDaysReservationsRoom(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            default:
                print("Something Wrong");
                break;
        }
    }

    private void ShowGraphWithXAxisTime(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<string> dateTimeDayList = new List<string>();

        int daysInSelectedPeriod = (endDateTime - startDateTime).Days;
        bool isSelectedPeriodLessYear = daysInSelectedPeriod > 90 && daysInSelectedPeriod < 365;
        bool isSelectedPeriodLessSeason = daysInSelectedPeriod <= 90;

        for (DateTime datetime = startDateTime; datetime.Date < endDateTime; datetime = datetime.AddDays(1))
        {
            int roomsQuantityInThisDay = 0;
            foreach (IReservation resItem in reservationList)
            {
                foreach (IRoom roomItem in filteredRoomList)
                {
                    if (datetime >= resItem.Period.Start && datetime < resItem.Period.End && resItem.RoomID == roomItem.ID)
                    {
                        roomsQuantityInThisDay++;
                    }
                }
            }
            bool isDenominatorNonZero = filteredRoomList.Count != 0;
            float roomsPercentInThisDay = isDenominatorNonZero ? (float)roomsQuantityInThisDay / filteredRoomList.Count : 0;
            data.Add(roomsPercentInThisDay);

            if (isSelectedPeriodLessYear)
            {
                dateTimeDayList.Add(datetime.Day.ToString() + " " + Constants.MonthNamesDict[datetime.Month].Substring(0, 3));
            }
            else if (isSelectedPeriodLessSeason)
            {
                dateTimeDayList.Add(datetime.Day.ToString());
            }
            else
            {
                dateTimeDayList.Add(Constants.MonthNamesDict[datetime.Month].Substring(0, 3) + " " + datetime.Year.ToString());
            }
        }

        SetDataInGraph(data, dateTimeDayList, true);
    }

    private void ShowGraphWithXAxisLocation(List<IRoom> filteredRooms, DateTime start, DateTime end, List<IReservation> reservations)
    {
        List<float> data = new List<float>();
        var properties = PropertyDataManager.GetProperties();
        List<string> propertyNames = properties.Select(p => p.Name).ToList();

        foreach (var property in properties)
        {
            var rooms = filteredRooms.Where(r => r.PropertyID.Equals(property.ID));
            if (rooms.Count() == 0)
            {
                data.Add(0f);
                continue;
            }

            var slots = GetReservationSlots(rooms, reservations, start, end);
            float reservedSlots = slots.Count(s => s.Reservation != null);
            data.Add(reservedSlots / slots.Count);
        }

        SetDataInGraph(data, propertyNames, true);
    }

    private void ShowGraphWithXAxisRoomCategoryByPersons(List<IRoom> filteredRooms, DateTime start, DateTime end, List<IReservation> reservations)
    {
        List<float> data = new List<float>();
        var roomCategories = filteredRooms.Select(room => room.Persons).Distinct();
        List<string> roomCategoryLabels = roomCategories.Select(c => c.ToString()).ToList();

        foreach (var personsCount in roomCategories)
        {
            var rooms = filteredRooms.Where(r => r.Persons == personsCount);
            if (rooms.Count() == 0)
            {
                data.Add(0f);
                continue;
            }

            var slots = GetReservationSlots(rooms, reservations, start, end);
            float reservedSlots = slots.Count(s => s.Reservation != null);
            data.Add(reservedSlots / slots.Count);
        }

        SetDataInGraph(data, roomCategories.Select(c => c.ToString()).ToList(), true);
    }

    private void ShowGraphWithXAxisDaysReservationsRoom(List<IRoom> filteredRooms, DateTime start, DateTime end, List<IReservation> reservations)
    {
        List<float> data = new List<float>();
        var roomNames = filteredRooms.Select(r => r.Name).ToList();

        foreach (var room in filteredRooms)
        {
            var slots = GetReservationSlots(new List<IRoom>(){ room }, reservations, start, end);
            float reservedSlots = slots.Count(s => s.Reservation != null);
            data.Add(reservedSlots / slots.Count);
        }

        SetDataInGraph(data, roomNames, true);
    }

    private List<ReservationSlot> GetReservationSlots(IEnumerable<IRoom> rooms, IEnumerable<IReservation> reservations, DateTime startDay, DateTime endDay)
    {
        List<ReservationSlot> slots = new List<ReservationSlot>();

        foreach (var room in rooms)
        {
            IEnumerable<IReservation> roomReservations = reservations.Where(r => r.RoomID.Equals(room.ID));

            for (DateTime day = startDay; day < endDay; day = day.AddDays(1))
            {
                var reservation = roomReservations.FirstOrDefault(r => r.Period.Includes(day));
                ReservationSlot slot = new ReservationSlot(day, room, reservation);
                slots.Add(slot);
            }
        }

        return slots;
    }

    private void SetDataInGraph(List<float> data, List<string> roomNameList, bool hasAlternativeColors)
    {
        graph.HasAlternativeColors = hasAlternativeColors;
        graph.XValue = roomNameList;
        graph.Data = data;
    }

    private class ReservationSlot
    {
        public DateTime Day { get; set; }
        public IRoom Room { get; set; }
        public IReservation Reservation { get; set; }

        public ReservationSlot(DateTime day, IRoom room, IReservation reservation)
        {
            Day = day;
            Room = room;
            Reservation = reservation;
        }
    }
}
