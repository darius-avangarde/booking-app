﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public DisponibilityScreen DisponibilitySccreenComponent { get; set; }
    public Image disponibilityMarker;
    public bool Selected { get; set; } = false;

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
    private ScrollButton roomScrollButton = null;
    //[SerializeField]
    //private Image disponibilityMarker = null;
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

    public void Initialize(IRoom room, DisponibilityScreen disponibilityScript, Action<IRoom> roomCallback, Action<DateTime, DateTime, List<IRoom>> reservationCallback)
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
        currentRoom = room;
        if (disponibilityScript != null)
        {
            roomButton.enabled = false;
            roomScrollButton.enabled = true;
            if (reservationCallback != null)
            {
                roomScrollButton.OnClick.AddListener(() => reservationCallback(dateTimeStart, dateTimeEnd, SendCurrentRoom()));
            }
            else
            {
                roomScrollButton.OnClick.AddListener(() => roomCallback(room));
            }
            roomScrollButton.OnPointerDownEvent.AddListener(() => StartCoroutine(SelectionMode()));
            roomScrollButton.OnPointerUpEvent.AddListener(() => StopAllCoroutines());
            roomScrollButton.OnDragEvent.AddListener(() => StopAllCoroutines());
            if (disponibilityScript.selectedRooms.Any(r => r.ID == currentRoom.ID))
            {
                SelectToggleMark();
            }
        }
        else
        {
            roomScrollButton.enabled = false;
            roomButton.enabled = true;
            roomButton.onClick.AddListener(() => roomCallback(room));
        }
    }

    private List<IRoom> SendCurrentRoom()
    {
        List<IRoom> currentRoomList = new List<IRoom>();
        currentRoomList.Add(currentRoom);
        return currentRoomList;
    } 

    public void SelectToggleMark()
    {
        if (Selected)
        {
            Selected = false;
            selectMarker.gameObject.SetActive(false);
            if (DisponibilitySccreenComponent.selectedRooms.Any(r => r.ID == currentRoom.ID))
            {
                DisponibilitySccreenComponent.selectedRooms.Remove(currentRoom);
            }
        }
        else
        {
            Selected = true;
            selectMarker.gameObject.SetActive(true);
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
            while (timer < 0.6f)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        SelectToggleMark();
    }
}
