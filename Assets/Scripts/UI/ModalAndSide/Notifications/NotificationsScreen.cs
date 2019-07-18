using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ReservationEditScreen reservationEditScreen = null;
    [SerializeField]
    private GameObject notificationItemPrefab = null;
    [SerializeField]
    private Transform scrollViewContent = null;
    [SerializeField]
    private Button backButton = null;

    private Queue<NotificationItem> notificationItemPool = new Queue<NotificationItem>();
    private List<NotificationItem> activeNotificationItems = new List<NotificationItem>();
    private List<IReservation> currentMonthReservations = new List<IReservation>();
    private DateTime startDate = DateTime.Today.Date;
    private DateTime firstDay;
    private DateTime lastDay;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());

        for (int i = 0; i < 10; i++)
        {
            NotificationItem notificationItem = Instantiate(notificationItemPrefab, scrollViewContent).GetComponent<NotificationItem>();
            notificationItem.gameObject.SetActive(false);
            notificationItemPool.Enqueue(notificationItem);
        }
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        foreach (var item in activeNotificationItems)
        {
            item.gameObject.SetActive(false);
            notificationItemPool.Enqueue(item);
        }
        activeNotificationItems.Clear();
    }

    public void SetMonth(DateTime currentDate)
    {
        firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);
        lastDay = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
    }

    public void Initialize()
    {
        SetMonth(startDate);
        foreach (var reservation in ReservationDataManager.GetReservationsBetween(firstDay, lastDay))
        {
            try
            {
                NotificationItem notificationItem = notificationItemPool.Dequeue();
                notificationItem.gameObject.SetActive(true);
                notificationItem.Initialize(reservation);
                activeNotificationItems.Add(notificationItem);
            }
            catch (Exception)
            {
                NotificationItem notificationItem = Instantiate(notificationItemPrefab, scrollViewContent).GetComponent<NotificationItem>();
                notificationItem.gameObject.SetActive(true);
                notificationItem.Initialize(reservation);
                activeNotificationItems.Add(notificationItem);
            }
        }
    }
}
