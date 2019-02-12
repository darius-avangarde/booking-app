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
    private Color itemStatusImageColor;
    
    public void Initialize(IRoom room, List<IRoom> reservedRooms, Action callback)
    {
        itemStatusImageColor = itemStatusImage.color;
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.defaultRoomAdminScreenName : room.Name;
        bedQuantity.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        GetComponent<Button>().onClick.AddListener(() => callback());

        if (IsRoomReserved(room, reservedRooms))
        {
            itemStatusImage.color = Constants.reservedUnavailableItemColor;
        }
        else
        {
            itemStatusImage.color = itemStatusImageColor;
        }
    }

    private bool IsRoomReserved(IRoom currentRoom, List<IRoom> reservedRoomList)
    {
        foreach (var item in reservedRoomList)
        {
            if (currentRoom.Equals(item))
            {
                return true;
            }
        }
        return false;
    }
}
