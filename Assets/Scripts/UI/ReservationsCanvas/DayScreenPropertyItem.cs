using System;
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

    public void Initialize(IRoom room, bool isReserved, Action callback)
    {
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.NEW_PROPERTY : property.Name;
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.NEW_ROOM : room.Name;
        bedQuantity.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        GetComponent<Button>().onClick.AddListener(() => callback());
        itemStatusImage.color = isReserved ? Constants.reservedUnavailableItemColor : Constants.availableItemColor;
    }
}
