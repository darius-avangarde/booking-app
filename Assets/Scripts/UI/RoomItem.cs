﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField]
    private Text roomName;
    [SerializeField]
    private Text personsNumber;

    public void Initialize(IRoom room, Action callback)
    {
        roomName.text = room.Name;
        personsNumber.text = personsNumber.text + ": " + room.Persons.ToString();
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
