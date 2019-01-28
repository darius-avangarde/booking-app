using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomScreen : MonoBehaviour
{
    [SerializeField]
    private Text propertyAndRoomScreenTitle = null;
    [SerializeField]
    private Text roomDetails = null;
    
    public void UpdatePropertyFields(IRoom room)
    {
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        string propertyName = property.Name ?? Constants.defaultProperyAdminScreenName;
        string roomName = room.Name ?? Constants.defaultRoomAdminScreenName;
        propertyAndRoomScreenTitle.text = propertyName + "-" + roomName;
        roomDetails.text = room.SingleBeds.ToString() + " paturi single" + " si " + room.DoubleBeds.ToString() + " paturi duble";
        InstantiateReservations();
    }

    private void InstantiateReservations()
    {

    }
}
