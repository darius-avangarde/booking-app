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
        // TODO: does a new listener get added every time we Initialize this? we may need to remove old listeners before adding new ones
        // in most places where we add a listener to a button item, the item gets destroyed before the container is re-initialized, but DayScreenPropertyItem doesn't get destroyed
        GetComponent<Button>().onClick.AddListener(() => callback());

        // TODO: you can check if a list contains an object with List.Contains
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
