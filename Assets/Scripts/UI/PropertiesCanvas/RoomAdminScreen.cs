﻿using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomAdminScreen : MonoBehaviour
{
    public IProperty currentProperty;
    private IRoom currentRoom;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private Text roomAdminScreenTitle = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private InputField roomSingleBedQuantityInputField = null;
    [SerializeField]
    private InputField roomDoubleBedQuantityInputField = null;

    public void DeleteRoom()
    {
        confirmationDialog.Show(() => {
            PropertyDataManager.GetProperty(currentProperty.ID).DeleteRoom(currentRoom.ID);
            navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
            propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().InstantiateRooms();
        }, null);
    }

    public void UpdateRoomFields(IProperty property, IRoom room)
    {
        currentProperty = property;
        currentRoom = room;
        roomNameInputField.text = currentRoom.Name ?? "";
        roomSingleBedQuantityInputField.text = room.SingleBeds.ToString();
        roomDoubleBedQuantityInputField.text = room.DoubleBeds.ToString();
        roomAdminScreenTitle.text = currentRoom.Name ?? Constants.defaultRoomAdminScreenName;
    }

    public void OnRoomNameValueChanged(string value)
    {
        roomAdminScreenTitle.text = value;
        currentRoom.Name = string.IsNullOrEmpty(value) ? Constants.defaultRoomAdminScreenName : value;
    }

    public void OnRoomSingleBedQuantityValueChanged(string value)
    {
        if (value == "-")
        {
            roomSingleBedQuantityInputField.text = "";
            return;
        }
        currentRoom.SingleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void OnRoomDoubleBedQuantityValueChanged(string value)
    {
        if (value == "-")
        {
            roomDoubleBedQuantityInputField.text = "";
            return;
        }
        currentRoom.DoubleBeds = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public void ChangeSingleBedQuantity(string sign)
    {
        if (sign == "+")
        {
            roomSingleBedQuantityInputField.text = (++currentRoom.SingleBeds).ToString();
        }
        else
        {
            if (roomSingleBedQuantityInputField.text != "0")
            {
                roomSingleBedQuantityInputField.text = (--currentRoom.SingleBeds).ToString();
            }
        }
    }

    public void ChangeDoubleBedQuantity(string sign)
    {
        if (sign == "+")
        {
            roomDoubleBedQuantityInputField.text = (++currentRoom.DoubleBeds).ToString();
        }
        else
        {
            if (roomDoubleBedQuantityInputField.text != "0")
            {
                roomDoubleBedQuantityInputField.text = (--currentRoom.DoubleBeds).ToString();
            }
        }
    }
}