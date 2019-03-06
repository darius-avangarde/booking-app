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
    }

    private void ShowAggregateGraphWeek(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<string> daysOfWeekList = new List<string>();
        int daysOfWeek = 7;
        for (int i = 1; i <= daysOfWeek; i++)
        {
            int roomsQuantityInThisDayOfWeek = 0;
            for (DateTime datetime = startDateTime; datetime.Date < endDateTime; datetime = datetime.AddDays(1))
            {
                foreach (IReservation resItem in reservationList)
                {
                    foreach (IRoom roomItem in filteredRoomList)
                    {
                        if (datetime >= resItem.Period.Start 
                            && datetime < resItem.Period.End 
                            && resItem.RoomID == roomItem.ID
                            && GetIndexDayOfWeek(datetime.DayOfWeek) == i)
                        {
                            roomsQuantityInThisDayOfWeek++;
                        }
                    }
                }
            }

            bool isDenominatorNonZero = filteredRoomList.Count != 0;
            float roomsPercentInThisDay = isDenominatorNonZero ? (float)roomsQuantityInThisDayOfWeek / filteredRoomList.Count : 0;
            data.Add(roomsPercentInThisDay);
            daysOfWeekList.Add(Constants.DayOfWeekNamesDict[i]);
        }
        

        SetDataInGraph(data, daysOfWeekList);
        print("ShowAggregateGraphWeek");
    }
    
    private void ShowAggregateGraphMonth(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        print("ShowAggregateGraphMonth");
    }

    private void ShowAggregateGraphYear(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        print("ShowAggregateGraphYear");
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
            case DayOfWeek.Monday: return 0;
            case DayOfWeek.Tuesday: return 1;
            case DayOfWeek.Wednesday: return 2;
            case DayOfWeek.Thursday: return 3;
            case DayOfWeek.Friday: return 4;
            case DayOfWeek.Saturday: return 5;
            case DayOfWeek.Sunday: return 6;
        }

        return 0;
    }
}
