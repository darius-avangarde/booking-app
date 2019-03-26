using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ReservationScreen reservationScreen = null;
    [SerializeField]
    private Text propertyAndRoomScreenTitle = null;
    [SerializeField]
    private Text roomDetails = null;
    [SerializeField]
    private GameObject reservationPrefabButton = null;
    [SerializeField]
    private Transform reservationsContent = null;
    private List<GameObject> reservationButtonList = new List<GameObject>();
    private DateTime dayDateTime = DateTime.Today;
    private IRoom currentRoom;
    private IReservation currentReservation;
    private List<IReservation> roomReservations = new List<IReservation>();

    public void UpdateRoomDetailsFields(DateTime date, IRoom room)
    {
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        dayDateTime = date;
        currentRoom = room;
        string propertyName = property.Name ?? Constants.defaultProperyAdminScreenName;
        string roomName = room.Name ?? Constants.defaultRoomAdminScreenName;
        propertyAndRoomScreenTitle.text = roomName;
        roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();

        InstantiateReservations();
    }

    public void CreateNewReservation()
    {
        reservationScreen.UpdateReservationScreen(dayDateTime, null, currentRoom);
    }

    public void InstantiateReservations()
    {
        List<IReservation> orderedRoomReservationList = ReservationDataManager.GetReservations()
                                                    .Where(res => res.RoomID == currentRoom.ID)
                                                    .OrderBy(res => res.Period.Start).ToList();

        foreach (var reservationButton in reservationButtonList)
        {
            Destroy(reservationButton);
        }

        foreach (var reservation in orderedRoomReservationList)
        {
            GameObject reservationButton = Instantiate(reservationPrefabButton, reservationsContent);
            reservationButton.GetComponent<ReservationItem>().Initialize(reservation, () => OpenReservationScreen(reservation));
            reservationButtonList.Add(reservationButton);
        }
    }

    private void OpenReservationScreen(IReservation reservation)
    {
        reservationScreen.GetComponent<ReservationScreen>().UpdateReservationScreen(dayDateTime, reservation, currentRoom);
        navigator.GoTo(reservationScreen.GetComponent<NavScreen>());
    }
}
