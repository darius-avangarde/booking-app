using System;
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

    public void Initialize(IRoom room, Action callback)
    {
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.defaultRoomAdminScreenName : room.Name;
        bedQuantity.text = room.SingleBeds.ToString() + " paturi single " + " si " + room.DoubleBeds.ToString() + " paturi duble ";
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
