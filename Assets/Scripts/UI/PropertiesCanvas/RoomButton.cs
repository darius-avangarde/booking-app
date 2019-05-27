using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    [SerializeField]
    private Text roomName = null;
    [SerializeField]
    private Text roomPrice = null;
    [SerializeField]
    private Text roomBeds = null;
    [SerializeField]
    private Button roomButton;
    [SerializeField]
    private Button editRoomButton;
    [SerializeField]
    private Button deleteRoomButton;
    [SerializeField]
    private Image disponibilityMarker = null;
    private IRoom currentRoom;
    private DateTime dateTimeFilter = DateTime.Today;
    //[SerializeField]
    //private Text personsNumber = null;

    public void Initialize(IRoom room, Action<IRoom> roomCallback, Action<IRoom> editCallback, Action<IRoom> deleteCallback)
    {
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.defaultRoomAdminScreenName : room.Name;
        roomPrice.text = string.IsNullOrEmpty(room.Price) ? Constants.Pret : ("Pret: " + room.Price + " ron");
        roomBeds.text = "Paturi ";
        if (room.SingleBeds != 0)
        {
            roomBeds.text += "single: " + room.SingleBeds;
        }
        if (room.SingleBeds != 0 && room.DoubleBeds != 0)
        {
            roomBeds.text += " & ";
        }
        if (room.DoubleBeds != 0)
        {
            roomBeds.text += "duble: " + room.DoubleBeds;
        }
        IEnumerable<IReservation> reservations = ReservationDataManager.GetActiveRoomReservations(room.ID)
                .Where(r => r.Period.Includes(dateTimeFilter));
        if (reservations.Count() == 0)
        {
            disponibilityMarker.color = Constants.availableItemColor;
        }
        else
        {
            disponibilityMarker.color = Constants.reservedUnavailableItemColor;
        }
        roomButton.onClick.AddListener(() => roomCallback(room));
        editRoomButton.onClick.AddListener(() => editCallback(room));
        deleteRoomButton.onClick.AddListener(() => deleteCallback(room));
        currentRoom = room;
    }
}
