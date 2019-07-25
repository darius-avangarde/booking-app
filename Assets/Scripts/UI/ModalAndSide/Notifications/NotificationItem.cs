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
    private GameObject newNotificationMarker = null;
    [SerializeField]
    private Sprite[] roomTypeIcons = null;

    private IReservation currentReservation;
    private IProperty currentProperty;
    private IRoom currentRoom;

    public void Initialize(bool newNotification, IReservation reservation, Action<IReservation> setReservation)
    {
        currentReservation = reservation;
        clientName.text = ClientDataManager.GetClient(currentReservation.CustomerID).Name;
        startDate.text = $"{currentReservation.Period.Start.Day}.{currentReservation.Period.Start.Month}.{currentReservation.Period.Start.Year}";
        endDate.text = $"{currentReservation.Period.End.Day}.{currentReservation.Period.End.Month}.{currentReservation.Period.End.Year}";
        currentProperty = PropertyDataManager.GetProperty(currentReservation.PropertyID);
        currentRoom = currentProperty.GetRoom(currentReservation.RoomID);
        propertyName.text = currentProperty.Name;
        roomName.text = currentRoom.Name;
        addedDate.text = $"Adăugat la {currentReservation.CreatedDateTime}";
        foreach (Sprite icon in roomTypeIcons)
        {
            if(icon.name.ToLower() == currentRoom.RoomType.ToString().ToLower())
            {
                roomTypeImage.sprite = icon;
            }
        }
        menuButton.onClick.AddListener(() => setReservation(reservation));
        newNotificationMarker.SetActive(newNotification);
    }
}
