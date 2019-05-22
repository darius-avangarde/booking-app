using System;
using System.Collections;
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
    private IRoom currentRoom;
    private Action<IRoom> currentCallback;
    //[SerializeField]
    //private Text personsNumber = null;

    public void Initialize(IRoom room, Action<IRoom> callback)
    {
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.defaultRoomAdminScreenName : room.Name;
        roomName.text = string.IsNullOrEmpty(room.Price) ? Constants.defaultRoomAdminScreenName : ("Pret: " + room.Price + " ron");
        roomBeds.text = " ";
        if (room.SingleBeds != 0)
        {
            roomBeds.text += "Paturi single: " + room.SingleBeds;
        }
        if (room.SingleBeds != 0 && room.DoubleBeds != 0)
        {
            roomBeds.text += ", ";
        }
        if (room.DoubleBeds != 0)
        {
            roomBeds.text += "Paturi duble: " + room.DoubleBeds;
        }
        currentCallback = callback;
        currentRoom = room;
        editRoomButton.onClick.AddListener(() => callback(room));
    }

    public void OpenRoom()
    {
        //roomButton.onClick.AddListener(() => currentCallback(currentRoom));
    }
}
