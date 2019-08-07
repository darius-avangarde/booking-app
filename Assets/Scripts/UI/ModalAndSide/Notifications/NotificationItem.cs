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

    private void OnDisable()
    {
        menuButton.onClick.RemoveAllListeners();
    }

    public void Initialize(bool newNotification, ThemeManager themeManagerComponent,  IReservation reservation, Action<IReservation> setReservation)
    {
        currentReservation = reservation;
        clientName.text = ClientDataManager.GetClient(currentReservation.CustomerID).Name;
        startDate.text = $"{currentReservation.Period.Start.Day}.{currentReservation.Period.Start.Month}.{currentReservation.Period.Start.Year}";
        endDate.text = $"{currentReservation.Period.End.Day}.{currentReservation.Period.End.Month}.{currentReservation.Period.End.Year}";
        currentProperty = PropertyDataManager.GetProperty(currentReservation.PropertyID);
        if(reservation.RoomIDs.Count > 1)
        {
            currentRoom = null;
            roomName.text = $"{reservation.RoomIDs.Count} {LocalizedText.Instance.RoomNumbers}";
            roomTypeImage.gameObject.SetActive(false);
        }
        else
        {
            currentRoom = currentProperty.GetRoom(currentReservation.RoomID);
            roomName.text = $"{LocalizedText.Instance.RoomItem} {currentRoom.Name}";
            for (int i = 0; i < roomTypeIcons.Length; i++)
            {
                if (roomTypeIcons[i].name.ToLower() == currentRoom.RoomType.ToString().ToLower())
                {
                    roomTypeImage.sprite = roomTypeIcons[i];
                }
            }
        }
        propertyName.text = currentProperty.Name;
        addedDate.text = $" { LocalizedText.Instance.AddedDateText} {currentReservation.CreatedDateTime.Day}.{currentReservation.CreatedDateTime.Month}.{currentReservation.CreatedDateTime.Year}, {currentReservation.CreatedDateTime.Hour}:{currentReservation.CreatedDateTime.Minute}";
        
        menuButton.onClick.AddListener(() => setReservation(reservation));
        newNotificationMarker.SetActive(newNotification);

        themeManagerComponent.AddItems(clientName.GetComponent<Graphic>(), startDate.GetComponent<Graphic>(), endDate.GetComponent<Graphic>(), propertyName.GetComponent<Graphic>(), roomName.GetComponent<Graphic>(), addedDate.GetComponent<Graphic>(), roomTypeImage.GetComponent<Graphic>(), notificationBackground, clientUnderline, menuButtonImage, startDateIcon, endDateIcon);
    }
}
