using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphComponent : MonoBehaviour
{
    [SerializeField]
    private Graph graph;
    private int totalDaysPeriod = 0;
    private int maxItemsForShowingAbout = 20;

    private void Start()
    {
        graph.XAxisLabel = Constants.XAxisDict[0];
        graph.YAxisLabel = "Procente";
    }

    public void UpdateGraph(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList, int xAxisTypeValueIndex)
    {
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
                ShowGraphWithXAxisRoomCategory(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            case 3:
                print("Camera");
                break;
            default:
                print("Something Wrong");
                break;
        }
    }

    private void ShowGraphWithXAxisTime(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        for (DateTime datetime = startDateTime; datetime.Date < endDateTime; datetime = datetime.AddDays(1))
        {
            float roomsQuantityInThisDay = 0;
            foreach (IReservation resItem in reservationList)
            {
                foreach (var roomItem in filteredRoomList)
                {
                    if (datetime >= resItem.Period.Start && datetime < resItem.Period.End && resItem.RoomID == roomItem.ID)
                    {
                        roomsQuantityInThisDay++;
                    }
                }
            }
            float roomsPercentInThisDay = (filteredRoomList.Count != 0) ? roomsQuantityInThisDay / filteredRoomList.Count : 0;
            data.Add(roomsPercentInThisDay);
        }
        graph.Data = data;
    }
    
    private void ShowGraphWithXAxisLocation(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<IProperty> propertyList = new List<IProperty>();
        propertyList.AddRange(PropertyDataManager.GetProperties());

        List<IReservation> reservationInSelectedPeriodList = new List<IReservation>();

        for (DateTime datetime = startDateTime; datetime.Date <= endDateTime; datetime = datetime.AddDays(1))
        {
            foreach (IReservation resItem in reservationList)
            {
                if (datetime >= resItem.Period.Start && datetime < resItem.Period.End)
                {
                    reservationInSelectedPeriodList.Add(resItem);
                }
            }
        }

        foreach (var propertyItem in propertyList)
        {
            List<IReservation> reservationFilteredRoomsList = reservationInSelectedPeriodList.FindAll(reservation =>
            {
                return filteredRoomList.Exists(room =>
                {
                    bool isRoomInReservation = room.ID == reservation.RoomID
                                               && reservation.PropertyID == propertyItem.ID;
                    return isRoomInReservation;
                });
            });
            int totalReservations = reservationList.Count;
            int reservationsInThisPropery = reservationFilteredRoomsList.Count;
            float reservationsPercentInThisPropery = (totalReservations != 0) ? reservationsInThisPropery / totalReservations : 0;
            data.Add(reservationsPercentInThisPropery);
        }
        graph.Data = data;
    }

    private void ShowGraphWithXAxisRoomCategory(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        List<IReservation> reservationInSelectedPeriodList = new List<IReservation>();

        for (DateTime datetime = startDateTime; datetime.Date <= endDateTime; datetime = datetime.AddDays(1))
        {
            foreach (IReservation resItem in reservationList)
            {
                if (datetime >= resItem.Period.Start && datetime < resItem.Period.End)
                {
                    reservationInSelectedPeriodList.Add(resItem);
                }
            }
        }
        foreach (var item in filteredRoomList)
        {

        }
    }
}
