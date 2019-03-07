using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: something like RoomButton might better indicate what this component does
public class RoomItem : MonoBehaviour
{
    [SerializeField]
    private Text roomName = null;
    [SerializeField]
    private Text personsNumber = null;

    public void Initialize(IRoom room, Action callback)
    {
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.defaultRoomAdminScreenName : room.Name;
        personsNumber.text = personsNumber.text + " " + room.Persons.ToString();
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
