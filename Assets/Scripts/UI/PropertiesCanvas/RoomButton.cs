using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public DisponibilityScreen DisponibilitySccreenComponent { get; set; }
    public bool selected = false;

    [SerializeField]
    private Text roomName = null;
    //[SerializeField]
    //private Text roomPrice = null;
    //[SerializeField]
    //private Text roomBeds = null;
    //[SerializeField]
    //private Text personsNumber = null;
    [SerializeField]
    private Button roomButton = null;
    [SerializeField]
    private Image disponibilityMarker = null;
    [SerializeField]
    private Image selectMarker = null;

    private IRoom currentRoom;
    private DateTime dateTimeStart = DateTime.Today.Date;
    private DateTime dateTimeEnd = DateTime.Today.AddDays(1).Date;
    private bool reservations = false;

    public void InitializeDateTime(DateTime dateTimeStart, DateTime dateTimeEnd)
    {
        this.dateTimeStart = dateTimeStart;
        this.dateTimeEnd = dateTimeEnd;
    }

    public void Initialize(IRoom room, DisponibilityScreen disponibilityScript, Action<IRoom> roomCallback)
    {
        selectMarker.gameObject.SetActive(false);
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.NEW_ROOM : room.Name;
        DisponibilitySccreenComponent = disponibilityScript;
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
        reservations = ReservationDataManager.GetReservationsBetween(dateTimeStart, dateTimeEnd)
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
        if(disponibilityScript != null)
        {
            if (disponibilityScript.selectedRooms.Any(r => r.ID == currentRoom.ID))
            {
                SelectToggleMark();
            }
        }
    }

    public void StartPressing()
    {
        if (DisponibilitySccreenComponent != null)
        {
            StartCoroutine(SelectionMode());
        }
    }

    public void SelectToggleMark()
    {
        if (selected)
        {
            selected = false;
            selectMarker.gameObject.SetActive(false);
            if (DisponibilitySccreenComponent.selectedRooms.Any(r => r.ID == currentRoom.ID))
            {
                DisponibilitySccreenComponent.selectedRooms.Remove(currentRoom);
            }
        }
        else
        {
            selected = true;
            selectMarker.gameObject.SetActive(true);
            disponibilityMarker.color = Color.white;
            if (!DisponibilitySccreenComponent.selectedRooms.Any(r => r.ID == currentRoom.ID))
            {
                DisponibilitySccreenComponent.selectedRooms.Add(currentRoom);
            }
            DisponibilitySccreenComponent.CheckRoomsSelection();
        }
    }

    private IEnumerator SelectionMode()
    {
        if (!DisponibilitySccreenComponent.roomSelection)
        {
            double timer = 0;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        SelectToggleMark();
    }
}
