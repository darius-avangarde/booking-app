using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject barPrefab = null;
    [SerializeField]
    private Transform barsContainer = null;
    private int totalDaysPeriod = 0;
    private int maxItemsForShowingAbout = 20;

    public void UpdateGraph(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList, int xAxisTypeValueIndex)
    {
        totalDaysPeriod = (endDateTime - startDateTime).Days;

        foreach (Transform child in barsContainer)
        {
            GameObject.Destroy(child.gameObject);
        }
        switch (xAxisTypeValueIndex)
        {
            case 0:
                ShowGraphWithXAxisTime(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            case 1:
                ShowGraphWithXAxisLocation(filteredRoomList, startDateTime, endDateTime, reservationList);
                break;
            case 2:
                print("Categorie de camera");
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
        for (DateTime datetime = startDateTime; datetime.Date < endDateTime; datetime = datetime.AddDays(1))
        {
            int roomsQuantityInThisDay = 0;
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
            InstantiateBarsByTime(datetime, roomsQuantityInThisDay, filteredRoomList.Count);
        }
    }

    private void InstantiateBarsByTime(DateTime dateTime, int roomsQuantityInThisDay, int filteredRoomsCount)
    {
        float roomsPercentInThisDay = (filteredRoomsCount != 0) ? 100 * roomsQuantityInThisDay / filteredRoomsCount : 0;
        GameObject bar = Instantiate(barPrefab, barsContainer);
        float barWidth = barsContainer.GetComponent<RectTransform>().rect.width / totalDaysPeriod;
        float barHeight = barsContainer.GetComponent<RectTransform>().rect.height * roomsPercentInThisDay / 100;
        bar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight);
        Color barColor = new Color(
              UnityEngine.Random.Range(0f, 1f),
              UnityEngine.Random.Range(0f, 1f),
              UnityEngine.Random.Range(0f, 1f)
          );
        bar.GetComponent<Image>().color = barColor;
        if (totalDaysPeriod < maxItemsForShowingAbout)
        {
            bar.GetComponentInChildren<Text>().text = dateTime.Day.ToString();
            bar.transform.GetChild(1).GetComponent<Text>().text = roomsPercentInThisDay + "%";
        }
        else
        {
            bar.GetComponentInChildren<Text>().text = "";
            bar.transform.GetChild(1).GetComponent<Text>().text = "";
        }
    }


    private void ShowGraphWithXAxisLocation(List<IRoom> filteredRoomList, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList)
    {
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

            InstantiateBarsByLocation(propertyItem, propertyList.Count, reservationFilteredRoomsList.Count, reservationList.Count);
        }
    }

    private void InstantiateBarsByLocation(IProperty property, int propertiesCount, int reservationsInThisPropery, int totalReservations)
    {
        float reservationsPercentInThisPropery = (totalReservations != 0) ? 100 * reservationsInThisPropery / totalReservations : 0;
        GameObject bar = Instantiate(barPrefab, barsContainer);
        float barWidth = barsContainer.GetComponent<RectTransform>().rect.width / propertiesCount;
        float barHeight = barsContainer.GetComponent<RectTransform>().rect.height * reservationsPercentInThisPropery / 100;
        bar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight);
        Color barColor = new Color(
              UnityEngine.Random.Range(0f, 1f),
              UnityEngine.Random.Range(0f, 1f),
              UnityEngine.Random.Range(0f, 1f)
          );
        bar.GetComponent<Image>().color = barColor;
        if (propertiesCount < maxItemsForShowingAbout)
        {
            bar.GetComponentInChildren<Text>().text = property.Name;
            bar.transform.GetChild(1).GetComponent<Text>().text = reservationsPercentInThisPropery + "%";
        }
        else
        {
            bar.GetComponentInChildren<Text>().text = "";
            bar.transform.GetChild(1).GetComponent<Text>().text = "";
        }
            
    }
}
