using System.Collections;
using System.Collections.Generic;
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
    private List<GameObject> reservationButtons = new List<GameObject>();
    private IRoom currentRoom;

    private void Start()
    {
        InstantiateReservations();
    }

    public void UpdateRoomDetailsFields(IRoom room)
    {
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        string propertyName = property.Name ?? Constants.defaultProperyAdminScreenName;
        string roomName = room.Name ?? Constants.defaultRoomAdminScreenName;
        propertyAndRoomScreenTitle.text = propertyName + "-" + roomName;
        roomDetails.text = room.SingleBeds.ToString() + " paturi single" + " si " + room.DoubleBeds.ToString() + " paturi duble";
        InstantiateReservations();
    }

    public void CreateNewReservation()
    {
        IReservation reservation = ReservationDataManager.AddReservation(currentRoom);
        reservationScreen.UpdateReservationFields(reservation);
    }

    public void InstantiateReservations()
    {
        foreach (var reservationyButton in reservationButtons)
        {
            Destroy(reservationyButton);
        }
        foreach (var reservation in ReservationDataManager.GetReservations())
        {
            GameObject reservationButton = Instantiate(reservationPrefabButton, reservationsContent);
            reservationButton.GetComponent<ReservationItem>().Initialize(reservation, () => OpenReservationScreen(reservation));
            reservationButtons.Add(reservationButton);
        }
    }

    private void OpenReservationScreen(IReservation reservation)
    {
        reservationScreen.GetComponent<ReservationScreen>().UpdateReservationFields(reservation);
        navigator.GoTo(reservationScreen.GetComponent<NavScreen>());
    }
}
