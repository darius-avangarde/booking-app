using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
            dateTimeDayList.Add(datetime.Day.ToString());
        }

        SetDataInGraph(data, dateTimeDayList);
    }

    private static List<IReservation> GetReservationInSelectedPeriodList(DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
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
        reservationInSelectedPeriodList = reservationInSelectedPeriodList.Distinct().ToList();
        return reservationInSelectedPeriodList;
    }

    private void ShowGraphWithXAxisLocation(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<string> propertyNameList = new List<string>();

        List<IReservation> reservationInSelectedPeriodList = GetReservationInSelectedPeriodList(startDateTime, endDateTime, reservationList);
        
        List<IProperty> propertyList = new List<IProperty>();
        propertyList.AddRange(PropertyDataManager.GetProperties());

        foreach (var propertyItem in propertyList)
        {
            List<IReservation> reservationsInProperyList = reservationInSelectedPeriodList.FindAll(reservation =>
            {
                return filteredRoomList.Exists(room =>
                {
                    bool isRoomInReservation = room.ID == reservation.RoomID
                                               && reservation.PropertyID == propertyItem.ID;
                    return isRoomInReservation;
                });
            });

            int totalReservations = reservationList.Count;
            int reservationsInThisPropery = reservationsInProperyList.Count;
            bool isDenominatorNonZero = totalReservations != 0;
            float reservationsPercentInThisPropery = isDenominatorNonZero ? (float)reservationsInThisPropery / totalReservations : 0;
            data.Add(reservationsPercentInThisPropery);
            propertyNameList.Add(propertyItem.Name);
        }

        SetDataInGraph(data, propertyNameList);
    }
    
    private void ShowGraphWithXAxisRoomCategoryByPersons(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<string> roomCategoryList = new List<string>();

        List<IReservation> reservationInSelectedPeriodList = GetReservationInSelectedPeriodList(startDateTime, endDateTime, reservationList);

        int maxQuantityPersons = filteredRoomList.Count != 0 ? filteredRoomList.Max(room => room.Persons) : 0;
       
        for (int i = 1; i <= maxQuantityPersons; i++)
        {
            List<IReservation> reservationsInRoomCategoryList = reservationInSelectedPeriodList.FindAll(reservation =>
            {
                return filteredRoomList.Exists(room =>
                {
                    bool isRoomInReservation = room.ID == reservation.RoomID
                                               && room.Persons == i;
                    return isRoomInReservation;
                });
            });
            float reservationsInRoomCategory = reservationsInRoomCategoryList.Count;
            if (reservationsInRoomCategory != 0)
            {
                bool isDenominatorNonZero = reservationList.Count != 0;
                float reservationsPercentInRoomCategory = isDenominatorNonZero ? reservationsInRoomCategory / reservationList.Count : 0;
                roomCategoryList.Add(i.ToString());
                data.Add(reservationsPercentInRoomCategory);
            }
        }

        SetDataInGraph(data, roomCategoryList);
    }

    private void ShowGraphWithXAxisDaysReservationsRoom(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
        List<float> data = new List<float>();
        List<string> roomNameList = new List<string>();

        List<IReservation> reservationInSelectedPeriodList = GetReservationInSelectedPeriodList(startDateTime, endDateTime, reservationList);

        List<int> reservedDaysInRoomList = new List<int>();
        foreach (IRoom roomItem in filteredRoomList)
        {
            int reservedDaysInRoom = 0;
            foreach (IReservation reservationItem in reservationInSelectedPeriodList)
            {
                if (roomItem.ID == reservationItem.RoomID)
                {
                    reservedDaysInRoom += (reservationItem.Period.End - reservationItem.Period.Start).Days;
                }
            }
            roomNameList.Add(roomItem.Name);
            reservedDaysInRoomList.Add(reservedDaysInRoom);
        }

        int maxReservedDays = reservedDaysInRoomList.Count != 0 ? reservedDaysInRoomList.Max(d => d) : 0;
        float reservedDaysPercentInRoom = 0;
        bool isDenominatorNonZero = maxReservedDays != 0;
        foreach (var item in reservedDaysInRoomList)
        {
            reservedDaysPercentInRoom = isDenominatorNonZero ? (float)item / maxReservedDays : 0;
            data.Add(reservedDaysPercentInRoom);
        }

        SetDataInGraph(data, roomNameList);
    }

    private void SetDataInGraph(List<float> data, List<string> roomNameList)
    {
        graph.XValue = roomNameList;
        graph.Data = data;
    }
}
