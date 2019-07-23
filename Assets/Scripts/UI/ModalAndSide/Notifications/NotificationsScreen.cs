using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using Unity.Notifications.Android;
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
    private GameObject noNotificationsObject = null;
    [SerializeField]
    private NotificationOptionsMenu notificationDropdown = null;
    [SerializeField]
    private Transform scrollViewContent = null;
    [SerializeField]
    private Button backButton = null;

    private Queue<NotificationItem> notificationItemPool = new Queue<NotificationItem>();
    private List<NotificationItem> activeNotificationItems = new List<NotificationItem>();
    private List<IReservation> currentReservations = new List<IReservation>();

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());

        noNotificationsObject.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            NotificationItem notificationItem = Instantiate(notificationItemPrefab, scrollViewContent).GetComponent<NotificationItem>();
            notificationItem.gameObject.SetActive(false);
            notificationItemPool.Enqueue(notificationItem);
        }

        AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler =
    delegate (AndroidNotificationIntentData data)
    {
        var msg = "Notification received : " + data.Id + "\n";
        msg += "\n Notification received: ";
        msg += "\n .Title: " + data.Notification.Title;
        msg += "\n .Body: " + data.Notification.Text;
        msg += "\n .Channel: " + data.Channel;
        Debug.Log(msg);
        //List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == data.Id).ToList();
        //foreach (IReservation reservation in newReservations)
        //{
        //    currentReservations.Add(reservation);
        //}
        navigator.GoTo(GetComponent<NavScreen>());
    };
        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;
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
        noNotificationsObject.SetActive(false);
    }

    public void Initialize()
    {
        noNotificationsObject.SetActive(false);
        currentReservations = ReservationDataManager.GetReservationsBetween(DateTime.Today.Date, DateTime.Today.Date.AddDays(3)).ToList();
        foreach (var reservation in currentReservations)
        {
            try
            {
                NotificationItem notificationItem = notificationItemPool.Dequeue();
                notificationItem.gameObject.SetActive(true);
                notificationItem.Initialize(reservation, OpenItemMenu);
                activeNotificationItems.Add(notificationItem);
            }
            catch (Exception)
            {
                NotificationItem notificationItem = Instantiate(notificationItemPrefab, scrollViewContent).GetComponent<NotificationItem>();
                notificationItem.gameObject.SetActive(true);
                notificationItem.Initialize(reservation, OpenItemMenu);
                activeNotificationItems.Add(notificationItem);
            }
        }

        if (activeNotificationItems.Count == 0)
        {
            noNotificationsObject.SetActive(true);
        }
    }

    private void OpenItemMenu(IReservation reservation)
    {
        notificationDropdown.OpenMenu(reservation);
    }
}
