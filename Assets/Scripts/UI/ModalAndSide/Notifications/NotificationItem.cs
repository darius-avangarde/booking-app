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

    [SerializeField]
    private Graphic notificationBackground = null;
    [SerializeField]
    private Graphic clientUnderline = null;
    [SerializeField]
    private Graphic menuButtonImage = null;
    [SerializeField]
    private Graphic startDateIcon = null;
    [SerializeField]
    private Graphic endDateIcon = null;

    private IReservation currentReservation;
    private IProperty currentProperty;
    private IRoom currentRoom;

    public void Initialize(bool newNotification, ThemeManager themeManagerComponent,  IReservation reservation, Action<IReservation> setReservation)
    {
        currentReservation = reservation;
        clientName.text = ClientDataManager.GetClient(currentReservation.CustomerID).Name;
        startDate.text = $"{currentReservation.Period.Start.Day}.{currentReservation.Period.Start.Month}.{currentReservation.Period.Start.Year}";
        endDate.text = $"{currentReservation.Period.End.Day}.{currentReservation.Period.End.Month}.{currentReservation.Period.End.Year}";
        currentProperty = PropertyDataManager.GetProperty(currentReservation.PropertyID);
        currentRoom = currentProperty.GetRoom(currentReservation.RoomID);
        propertyName.text = currentProperty.Name;
        roomName.text = currentRoom.Name;
        addedDate.text = $"Adăugat la {currentReservation.CreatedDateTime.Day}.{currentReservation.CreatedDateTime.Month}.{currentReservation.CreatedDateTime.Year}, {currentReservation.CreatedDateTime.Hour}:{currentReservation.CreatedDateTime.Minute}";
        foreach (Sprite icon in roomTypeIcons)
        {
            if (icon.name.ToLower() == currentRoom.RoomType.ToString().ToLower())
            {
                roomTypeImage.sprite = icon;
            }
        }
        menuButton.onClick.AddListener(() => setReservation(reservation));
        newNotificationMarker.SetActive(newNotification);

        themeManagerComponent.AddItems(clientName.GetComponent<Graphic>());
        themeManagerComponent.AddItems(startDate.GetComponent<Graphic>());
        themeManagerComponent.AddItems(endDate.GetComponent<Graphic>());
        themeManagerComponent.AddItems(propertyName.GetComponent<Graphic>());
        themeManagerComponent.AddItems(roomName.GetComponent<Graphic>());
        themeManagerComponent.AddItems(addedDate.GetComponent<Graphic>());
        themeManagerComponent.AddItems(roomTypeImage.GetComponent<Graphic>());
        themeManagerComponent.AddItems(notificationBackground);
        themeManagerComponent.AddItems(clientUnderline);
        themeManagerComponent.AddItems(menuButtonImage);
        themeManagerComponent.AddItems(startDateIcon);
        themeManagerComponent.AddItems(endDateIcon);
    }
}
