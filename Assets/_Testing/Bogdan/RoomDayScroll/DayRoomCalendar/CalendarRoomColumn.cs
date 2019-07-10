﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CalendarRoomColumn : MonoBehaviour
{
    [SerializeField]
    private GameObject roomColumnObjectPrefab;

    private List<CalendarRoomColumnObject> roomPool = new List<CalendarRoomColumnObject>();

    public void UpdateRooms(List<IRoom> rooms, UnityAction<IRoom> tapAction)
    {
        ManagePool(rooms);

        for (int r = 0; r < rooms.Count; r++)
        {
            roomPool[r].UpdateRoomObject(rooms[r], tapAction);
        }
    }

    private void ManagePool(List<IRoom> rooms)
    {
        if(rooms.Count != roomPool.Count)
        {
            //Create New Objects as needed
            while(roomPool.Count < rooms.Count)
            {
                CreateRoomColumnObject();
            }

            //Disable unused objects
            for (int i = roomPool.Count - 1; i > rooms.Count; i--)
            {
                roomPool[i].gameObject.SetActive(false);
            }
        }
    }

    private void CreateRoomColumnObject()
    {
        roomPool.Add(Instantiate(roomColumnObjectPrefab, transform).GetComponent<CalendarRoomColumnObject>());
    }
}