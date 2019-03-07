using System;
using System.Collections.Generic;
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

        var watch = System.Diagnostics.Stopwatch.StartNew();
        // the code that you want to measure comes here
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
            case 3:
                ShowAggregateGraphRoomType(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            default:
                print("Something Wrong");
                break;
        }
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        print(elapsedMs);
    }

    private void ShowAggregateGraphWeek(List<IRoom> filteredRoomList, DateTime selectedStartDateTime, DateTime selectedEndDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<string> daysOfWeekList = new List<string>();
        int daysOfWeek = 7;
        List<IReservation> filteredReservationList = GetFilteredReservationList(filteredRoomList, reservationList);

        Dictionary<int, int> roomCountInDayOfWeek = new Dictionary<int, int>();

        foreach (IReservation resItem in filteredReservationList)
        {
            DateTime start = resItem.Period.Start;
            DateTime end = resItem.Period.End;

            if (resItem.Period.Start >= selectedStartDateTime && resItem.Period.End < selectedEndDateTime)
            {
                for (DateTime datetime = start; datetime.Date < end; datetime = datetime.AddDays(1))
                {
                    if (!roomCountInDayOfWeek.ContainsKey(GetIndexDayOfWeek(datetime.DayOfWeek)))
                    {
                        roomCountInDayOfWeek.Add(GetIndexDayOfWeek(datetime.DayOfWeek), 0);
                    }
                    roomCountInDayOfWeek[GetIndexDayOfWeek(datetime.DayOfWeek)]++;
                }
            }
        }
        bool isDenominatorNonZero = filteredRoomList.Count != 0;
        for (int i = 1; i <= daysOfWeek; i++)
        {
            float roomsPercentInThisDay = 0;
            if (roomCountInDayOfWeek.ContainsKey(i))
            {
                roomsPercentInThisDay = isDenominatorNonZero ? (float)roomCountInDayOfWeek[i] / filteredRoomList.Count : 0;
            }
            data.Add(roomsPercentInThisDay);
            daysOfWeekList.Add(Constants.DayOfWeekNamesDict[i].Substring(0,3));
        }
        SetDataInGraph(data, daysOfWeekList);
    }
    
    private void ShowAggregateGraphMonth(List<IRoom> filteredRoomList, DateTime selectedStartDateTime, DateTime selectedEndDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<string> daysOfMonthList = new List<string>();
        int daysOfMonth = 31;
        List<IReservation> filteredReservationList = GetFilteredReservationList(filteredRoomList, reservationList);

        Dictionary<int, int> roomCountInDayOfMonth = new Dictionary<int, int>();

        foreach (IReservation resItem in filteredReservationList)
        {
            DateTime start = resItem.Period.Start;
            DateTime end = resItem.Period.End;

            if (resItem.Period.Start >= selectedStartDateTime && resItem.Period.End < selectedEndDateTime)
            {
                for (DateTime datetime = start; datetime.Date < end; datetime = datetime.AddDays(1))
                {
                    if (!roomCountInDayOfMonth.ContainsKey(datetime.Day))
                    {
                        roomCountInDayOfMonth.Add(datetime.Day, 0);
                    }
                    roomCountInDayOfMonth[datetime.Day]++;
                }
            }
        }
        bool isDenominatorNonZero = filteredRoomList.Count != 0;
        for (int i = 1; i <= daysOfMonth; i++)
        {
            float roomsPercentInThisDay = 0;
            if (roomCountInDayOfMonth.ContainsKey(i))
            {
                roomsPercentInThisDay = isDenominatorNonZero ? (float)roomCountInDayOfMonth[i] / filteredRoomList.Count : 0;
            }
            data.Add(roomsPercentInThisDay);
            daysOfMonthList.Add(i.ToString());
        }
        SetDataInGraph(data, daysOfMonthList);
    }

    private void ShowAggregateGraphYear(List<IRoom> filteredRoomList, DateTime selectedStartDateTime, DateTime selectedEndDateTime, List<IReservation> reservationList)
    {
        //List<float> data = new List<float>();
        //List<string> monthOfYearList = new List<string>();
        //int monthOfYear = 12;
        //List<IReservation> filteredReservationList = GetFilteredReservationList(filteredRoomList, reservationList);

        //Dictionary<int, int> roomCountInMonthOfYear = new Dictionary<int, int>();

        //foreach (IReservation resItem in filteredReservationList)
        //{
        //    DateTime start = resItem.Period.Start;
        //    DateTime end = resItem.Period.End;

        //    if (resItem.Period.Start >= selectedStartDateTime && resItem.Period.End < selectedEndDateTime)
        //    {
        //        for (DateTime datetime = start; datetime.Date < end; datetime = datetime.AddDays(1))
        //        {
        //            if (!roomCountInMonthOfYear.ContainsKey(datetime.Month))
        //            {
        //                roomCountInMonthOfYear.Add(datetime.Month, 0);
        //            }
        //        }
        //    }
        //}
        //bool isDenominatorNonZero = filteredRoomList.Count != 0;
        //for (int i = 1; i <= monthOfYear; i++)
        //{
        //    float roomsPercentInThisDay = 0;
        //    if (roomCountInMonthOfYear.ContainsKey(i))
        //    {
        //        roomsPercentInThisDay = isDenominatorNonZero ? (float)roomCountInMonthOfYear[i] / filteredRoomList.Count : 0;
        //    }
        //    data.Add(roomsPercentInThisDay);
        //    monthOfYearList.Add(Constants.MonthNamesDict[i].Substring(0, 3));

        //}
        //SetDataInGraph(data, monthOfYearList);
    }
    
    private void ShowAggregateGraphRoomType(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        print("ShowAggregateGraphRoomType");
    }

    private void SetDataInGraph(List<float> data, List<string> roomNameList)
    {
        graph.XValue = roomNameList;
        graph.Data = data;
    }

    private int GetIndexDayOfWeek(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
            case DayOfWeek.Sunday: return 7;
        }

        return 0;
    }
    
    private static List<IReservation> GetFilteredReservationList(List<IRoom> filteredRoomList, List<IReservation> reservationList)
    {
        List<IReservation> filteredReservationList = new List<IReservation>();

        foreach (IReservation resItem in reservationList)
        {
            foreach (IRoom roomItem in filteredRoomList)
            {
                if (resItem.RoomID == roomItem.ID)
                {
                    filteredReservationList.Add(resItem);
                }
            }
        }

        return filteredReservationList;
    }
}
