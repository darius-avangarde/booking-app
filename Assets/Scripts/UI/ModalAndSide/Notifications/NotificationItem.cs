using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications.Android;

public class NotificationItem : MonoBehaviour
{
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
    [SerializeField]
    private Button menuButton = null;
    [SerializeField]
    private Sprite[] roomTypeIcons = null;

    private IReservation currentReservation;
    private IProperty currentProperty;
    private IRoom currentRoom;
    private bool highlight = false;

    public void Initialize(IReservation reservation, Action<IReservation> setReservation)
    {
        currentReservation = reservation;
        clientName.text = ClientDataManager.GetClient(currentReservation.CustomerID).Name;
        startDate.text = currentReservation.Period.Start.Date.ToString();
        endDate.text = currentReservation.Period.End.Date.ToString();
        currentProperty = PropertyDataManager.GetProperty(currentReservation.PropertyID);
        currentRoom = currentProperty.GetRoom(currentReservation.RoomID);
        propertyName.text = currentProperty.Name;
        roomName.text = currentRoom.Name;
        //addedDate.text = currentReservation.AddedDate;
        foreach (Sprite icon in roomTypeIcons)
        {
            if(icon.name.ToLower() == currentRoom.RoomType.ToString().ToLower())
            {
                roomTypeImage.sprite = icon;
            }
        }
        menuButton.onClick.AddListener(() => setReservation(reservation));
    }
}
