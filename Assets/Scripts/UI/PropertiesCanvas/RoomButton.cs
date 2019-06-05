using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    [SerializeField]
    private Text roomName = null;
    //[SerializeField]
    //private Text roomPrice = null;
    //[SerializeField]
    //private Text roomBeds = null;
    [SerializeField]
    private Button roomButton;
    [SerializeField]
    private Image disponibilityMarker = null;

    private IRoom currentRoom;
    private DateTime dateTimeStart = DateTime.Today.Date;
    private DateTime dateTimeEnd = DateTime.Today.AddDays(1).Date;
    //[SerializeField]
    //private Text personsNumber = null;

    public void InitializeDateTime(DateTime dateTimeStart, DateTime dateTimeEnd)
    {
        this.dateTimeStart = dateTimeStart;
        this.dateTimeEnd = dateTimeEnd;
    }

    public void Initialize(IRoom room, Action<IRoom> roomCallback)
    {
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.NEW_ROOM : room.Name;
        //roomPrice.text = string.IsNullOrEmpty(room.Price) ? Constants.PRICE : ("Pret: " + room.Price + " ron");
        //roomBeds.text = null;
        //if (room.SingleBeds != 0)
        //{
        //    roomBeds.text += Constants.SingleBed + room.SingleBeds;
        //}
        //if (room.SingleBeds != 0 && room.DoubleBeds != 0)
        //{
        //    roomBeds.text += " & ";
        //}
        //if (room.DoubleBeds != 0)
        //{
        //    roomBeds.text += Constants.DoubleBed + room.DoubleBeds;
        //}
        bool reservations = ReservationDataManager.GetReservationsBetween(dateTimeStart, dateTimeEnd)
                .Any(r => r.RoomID == room.ID);
        if (reservations)
        {
            disponibilityMarker.color = Constants.reservedUnavailableItemColor;
        }
        else
        {
            disponibilityMarker.color = Constants.availableItemColor;
        }
        roomButton.onClick.AddListener(() => roomCallback(room));
        currentRoom = room;
    }
}
