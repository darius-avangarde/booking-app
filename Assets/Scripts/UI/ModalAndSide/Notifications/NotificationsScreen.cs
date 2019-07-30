using System;
using System.Linq;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ThemeManager themeManager = null;
    [SerializeField]
    private SettingsManager settingsManager = null;
    [SerializeField]
    private NotificationBadge notificationBadge = null;
    [SerializeField]
    private GameObject notificationItemPrefab = null;
    [SerializeField]
    private GameObject noNotificationsObject = null;
    [SerializeField]
    private NotificationOptionsMenu notificationDropdown = null;
    [SerializeField]
    private Transform scrollViewContent = null;
    [SerializeField]
    private Button backButton = null;

    private Queue<NotificationItem> notificationItemPool = new Queue<NotificationItem>();
    private List<NotificationItem> activeNotificationItems = new List<NotificationItem>();
    private List<IReservation> newReservations = new List<IReservation>();
    private List<IReservation> currentReservations = new List<IReservation>();

    private void Start()
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
        foreach (NotificationItem notification in activeNotificationItems)
        {
            notification.gameObject.SetActive(false);
            notificationItemPool.Enqueue(notification);
        }
        activeNotificationItems.Clear();
        noNotificationsObject.SetActive(false);
    }

    public void AddNewReservations(List<IReservation> newNotifications)
    {
        foreach (IReservation reservation in newNotifications)
        {
            this.newReservations.Add(reservation);
        }
        int notificationsCount = newNotifications.Count();
        notificationBadge.SetNotificationBadge(notificationsCount);
    }

    public void Initialize()
    {
        noNotificationsObject.SetActive(false);
        settingsManager.ReadData();
        currentReservations = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date.AddHours(12) > DateTime.Now && r.Period.Start.Date.AddHours(12) <= DateTime.Today.Date.AddHours(12 + Constants.PreAlertDict.ElementAt(settingsManager.DataElements.settings.PreAlertTime).Key)).OrderBy(r => r.Period.Start).ToList();
        if (currentReservations != null)
        {
            foreach (IReservation reservation in currentReservations)
            {
                if (newReservations.Any(r => r.ID == reservation.ID))
                {
                    InitializeNotification(reservation, true);
                    NotificationDataManager.GetNotification(reservation.NotificationID).UnRead = false;
                }
                else
                {
                    InitializeNotification(reservation, false);
                }
            }
        }
        newReservations = new List<IReservation>();
        notificationBadge.SetNotificationBadge(newReservations.Count);
        if (activeNotificationItems.Count == 0)
        {
            noNotificationsObject.SetActive(true);
        }
    }

    private void InitializeNotification(IReservation reservation, bool newNotification)
    {
        try
        {
            NotificationItem notificationItem = notificationItemPool.Dequeue();
            notificationItem.gameObject.SetActive(true);
            notificationItem.Initialize(newNotification, themeManager, reservation, OpenItemMenu);
            activeNotificationItems.Add(notificationItem);
        }
        catch (Exception)
        {
            NotificationItem notificationItem = Instantiate(notificationItemPrefab, scrollViewContent).GetComponent<NotificationItem>();
            notificationItem.gameObject.SetActive(true);
            notificationItem.Initialize(newNotification, themeManager,reservation, OpenItemMenu);
            activeNotificationItems.Add(notificationItem);
        }
    }

    private void OpenItemMenu(IReservation reservation)
    {
        notificationDropdown.OpenMenu(reservation);
    }
}
