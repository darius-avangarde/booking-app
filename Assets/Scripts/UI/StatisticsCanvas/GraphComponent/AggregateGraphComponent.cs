using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AggregateGraphComponent : MonoBehaviour
{
    [SerializeField]
    private Graph graph = null;

    private void Start()
    {
        graph.XAxisLabel = Constants.AggregateXAxisDict[0];
        graph.YAxisLabel = "Procente";
    }

    public void UpdateGraph(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList, int xAxisTypeValueIndex)
    {
        graph.XAxisLabel = Constants.AggregateXAxisDict[xAxisTypeValueIndex];
        
        switch (xAxisTypeValueIndex)
        {
            case 0:
                ShowAggregateGraphWeek(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            case 1:
                ShowAggregateGraphMonth(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            case 2:
                ShowAggregateGraphYear(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            default:
                break;
        }
    }

    private void ShowAggregateGraphWeek(List<IRoom> filteredRoomList, DateTime selectedStartDateTime, DateTime selectedEndDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>(new float[7]);
        List<IReservation> filteredReservationList = GetFilteredReservationList(filteredRoomList, reservationList, selectedStartDateTime, selectedEndDateTime);

        if (filteredReservationList.Count == 0)
        {
            SetDataInGraph(data, Constants.DayOfWeekNames, true);
            return;
        }

        Dictionary<DayOfWeek, int> reservedDayCount = new Dictionary<DayOfWeek, int>();
        
        foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
        {
            int count = 0;
            foreach (IReservation reservation in filteredReservationList)
            {
                DateTime start = reservation.Period.Start;
                DateTime end = reservation.Period.End;

                for (DateTime datetime = start; datetime < end; datetime = datetime.AddDays(1))
                {
                    bool isDayInSelectedPeriod = datetime >= selectedStartDateTime && datetime < selectedEndDateTime;
                    if (datetime.DayOfWeek == dayOfWeek && isDayInSelectedPeriod)
                    {
                        count++;
                    }
                }
            }
            reservedDayCount[dayOfWeek] = count;
        }

        float maxReservedDayCount = reservedDayCount.Values.Max();

        data[0] = reservedDayCount[DayOfWeek.Monday] / maxReservedDayCount;
        data[1] = reservedDayCount[DayOfWeek.Tuesday] / maxReservedDayCount;
        data[2] = reservedDayCount[DayOfWeek.Wednesday] / maxReservedDayCount;
        data[3] = reservedDayCount[DayOfWeek.Thursday] / maxReservedDayCount;
        data[4] = reservedDayCount[DayOfWeek.Friday] / maxReservedDayCount;
        data[5] = reservedDayCount[DayOfWeek.Saturday] / maxReservedDayCount;
        data[6] = reservedDayCount[DayOfWeek.Sunday] / maxReservedDayCount;
        SetDataInGraph(data, Constants.DayOfWeekNames, true);
    }

    private void ShowAggregateGraphMonth(List<IRoom> filteredRoomList, DateTime selectedStartDateTime, DateTime selectedEndDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<string> daysOfMonthList = new List<string>();
        int daysOfMonth = 31;
        List<IReservation> filteredReservationList = GetFilteredReservationList(filteredRoomList, reservationList, selectedStartDateTime, selectedEndDateTime);

        if (filteredReservationList.Count == 0)
        {
            for (int i = 1; i <= daysOfMonth; i++)
            {
                daysOfMonthList.Add(i.ToString());
            }
            SetDataInGraph(data, daysOfMonthList, true);
            return;
        }

        Dictionary<int, int> reservedDayCount = new Dictionary<int, int>();

        for (int i = 1; i <= daysOfMonth; i++)
        {
            int count = 0;
            foreach (IReservation reservation in filteredReservationList)
            {
                DateTime start = reservation.Period.Start;
                DateTime end = reservation.Period.End;

                for (DateTime datetime = start; datetime < end; datetime = datetime.AddDays(1))
                {
                    bool isDayInSelectedPeriod = datetime >= selectedStartDateTime && datetime < selectedEndDateTime;
                    if (datetime.Day == i && isDayInSelectedPeriod)
                    {
                        count++;
                    }
                }
            }
            reservedDayCount[i] = count;
        }

        float maxReservedDayCount = reservedDayCount.Values.Max();
        for (int i = 1; i <= daysOfMonth; i++)
        {
            data.Add(reservedDayCount[i] / maxReservedDayCount);
            daysOfMonthList.Add(i.ToString());
        }
        SetDataInGraph(data, daysOfMonthList, true);
    }

    private void ShowAggregateGraphYear(List<IRoom> filteredRoomList, DateTime selectedStartDateTime, DateTime selectedEndDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        int monthsOfYear = 12;

        List<IReservation> filteredReservationList = GetFilteredReservationList(filteredRoomList, reservationList, selectedStartDateTime, selectedEndDateTime);

        if (filteredReservationList.Count == 0)
        {
            SetDataInGraph(data, Constants.MonthNames, true);
            return;
        }

        Dictionary<int, int> reservedDayCount = new Dictionary<int, int>();
        for (int i = 1; i <= monthsOfYear; i++)
        {
            int count = 0;
            foreach (IReservation reservation in filteredReservationList)
            {
                DateTime start = reservation.Period.Start;
                DateTime end = reservation.Period.End;

                for (DateTime datetime = start; datetime < end; datetime = datetime.AddDays(1))
                {
                    bool isDayInSelectedPeriod = datetime >= selectedStartDateTime && datetime < selectedEndDateTime;
                    if (datetime.Month == i && isDayInSelectedPeriod)
                    {
                        count++;
                    }
                }
            }
            reservedDayCount[i] = count;
        }

        float maxReservedDayCount = reservedDayCount.Values.Max();
        for (int i = 1; i <= monthsOfYear; i++)
        {
            data.Add(reservedDayCount[i] / maxReservedDayCount);
        }
        SetDataInGraph(data, Constants.MonthNames, true);
    }
    
    private void SetDataInGraph(List<float> data, List<string> roomNameList, bool hasAlternativeColors)
    {
        graph.HasAlternativeColors = hasAlternativeColors;
        graph.XValue = roomNameList;
        graph.Data = data;
    }
    
    private static List<IReservation> GetFilteredReservationList(List<IRoom> filteredRoomList, List<IReservation> reservationList, DateTime start, DateTime end)
    {
        return reservationList.Where(reservation =>
        {
            bool isInSelectedPeriod = reservation.Period.Overlaps(start, end);
            return filteredRoomList.Exists(room => room.ID == reservation.RoomID) && isInSelectedPeriod;
        }).ToList();
    }
}
