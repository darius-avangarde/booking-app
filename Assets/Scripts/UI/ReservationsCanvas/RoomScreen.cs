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
    private IRoom currentRoom;
    private IReservation currentReservation;
    private List<IReservation> roomReservations = new List<IReservation>();

    // TODO: RoomScreen should initialize its fields on NavScreen.Showing, not when it receives Reservation and Room data
    public void UpdateRoomDetailsFields(IRoom room)
    {
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        string propertyName = property.Name ?? Constants.defaultProperyAdminScreenName;
        string roomName = room.Name ?? Constants.defaultRoomAdminScreenName;
        propertyAndRoomScreenTitle.text = propertyName + Constants.AndDelimiter + roomName;
        roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();

        InstantiateReservations();
    }

    public void CreateNewReservation()
    {
        // TODO: RoomScreen shouldn't have to tell ReservationScreen to open the calendar modal and create a new reservation
        // that logic should be in this script
        reservationScreen.UpdateReservationScreen(null, currentRoom);
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
            // TODO: it's possible to replace the annonymous function with OpenReservationScreen
            reservationButton.GetComponent<ReservationItem>().Initialize(reservation, () => OpenReservationScreen(reservation));
            reservationButtonList.Add(reservationButton);
        }
    }

    private void OpenReservationScreen(IReservation reservation)
    {
        reservationScreen.GetComponent<ReservationScreen>().UpdateReservationScreen(reservation, currentRoom);
        navigator.GoTo(reservationScreen.GetComponent<NavScreen>());
    }
}
