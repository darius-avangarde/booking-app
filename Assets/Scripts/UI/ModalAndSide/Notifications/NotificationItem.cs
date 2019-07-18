using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications.Android;

public class NotificationItem : MonoBehaviour
{
    [SerializeField]
    private NotificationDropdown notificationDropdown = null;
    [SerializeField]
    private Text clientName = null;
    [SerializeField]
    private Text startDate = null;
    [SerializeField]
    private Text endDate = null;
    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Text roomName = null;
    [SerializeField]
    private Text addedDate = null;
    [SerializeField]
    private Image roomTypeImage = null;

    private IReservation currentReservation;
    private IProperty currentProperty;
    private IRoom currentRoom;
    private bool canceled = false;

    public void Initialize(IReservation reservation)
    {
        currentReservation = reservation;
        //canceled = currentReservation.Canceled;
        clientName.text = ClientDataManager.GetClient(currentReservation.CustomerID).Name;
        startDate.text = currentReservation.Period.Start.Date.ToString();
        endDate.text = currentReservation.Period.End.Date.ToString();
        currentProperty = PropertyDataManager.GetProperty(currentReservation.PropertyID);
        currentRoom = currentProperty.GetRoom(currentReservation.RoomID);
        propertyName.text = currentProperty.Name;
        roomName.text = currentRoom.Name;
        //addedDate.text = currentReservation.AddedDate;
        //roomTypeImage = currentRoom.Type
        notificationDropdown.Initialize(currentReservation);
    }
}
