﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayScreenPropertyItem : MonoBehaviour
{
    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Text roomName = null;
    [SerializeField]
    private Text bedQuantity = null;
    [SerializeField]
    private Image itemStatusImage = null;
    private Color itemStatusImageColor;

    public void Initialize(IRoom room, bool isReserved, Action callback)
    {
        itemStatusImageColor = itemStatusImage.color;
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.defaultRoomAdminScreenName : room.Name;
        bedQuantity.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        GetComponent<Button>().onClick.AddListener(() => callback());
        itemStatusImage.color = isReserved ? Constants.reservedUnavailableItemColor : itemStatusImageColor;
    }
}
