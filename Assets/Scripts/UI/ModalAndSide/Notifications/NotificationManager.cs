using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class NotificationManager : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private NotificationsScreen notificationsScreen = null;
    [SerializeField]
    private NotificationBadge notificationBadge = null;
    [SerializeField]
    private SettingsManager settingsManager = null;
    [SerializeField]
    private AndroidNotificationsManager androidNotificationsManager = null;
    [SerializeField]
    private iOSNotificationsManager iOSNotificationsManager = null;

    private const string channelId = "Default";
    private string reservationsTitle;

    private void Start()
    {
        reservationsTitle = LocalizedText.Instance.UpcomingReservations;
        LocalizedText.Instance.OnLanguageChanged.AddListener(UpdateAllNotifications);
#if !UNITY_EDITOR && UNITY_ANDROID
        androidNotificationsManager.Initialize(navigator, notificationsScreen, notificationBadge, channelId);
#endif
#if !UNITY_EDITOR && UNITY_IOS
        iOSNotificationsManager.Initialize(navigator, notificationsScreen, notificationBadge, channelId);
#endif
    }

    /// <summary>
    /// function to update all sent notifications notifications
    /// </summary>
    private void UpdateAllNotifications()
    {
        settingsManager.ReadData();
        List<IReservation> allReservations = ReservationDataManager.GetReservations().ToList();
        for (int i = 0; i < allReservations.Count; i++)
        {
            RegisterNotification(allReservations[i], settingsManager.DataElements.settings.PreAlertTime);
        }
    }

    /// <summary>
    /// when the time before a notification is set in the set-up menu, 
    /// this function should be called to update all current notifications according to new setting
    /// </summary>
    public void UpdateDefaultNotifications(int previousPreAlert, int newPreAlert)
    {
        List<IReservation> previousReservations = ReservationDataManager.GetReservations().ToList();
        for (int i = 0; i < previousReservations.Count; i++)
        {
            INotification notification = NotificationDataManager.GetNotification(previousReservations[i].NotificationID);
            if (notification != null && notification.PreAlertTime == previousPreAlert)
            {
                RegisterNotification(previousReservations[i], newPreAlert);
            }
        }
    }

    /// <summary>
    /// call this when you create or edit a reservation to update the current set notifications
    /// </summary>
    /// <param name="reservation">the new reservation after add or edit</param>
    /// <param name="enumValue">value for period before a notification fires</param>
    public void RegisterNotification(IReservation reservation, int preAlert)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        androidNotificationsManager.RegisterNotification(reservation, preAlert, reservationsTitle, channelId);
#endif
#if !UNITY_EDITOR && UNITY_IOS
        iOSNotificationsManager.RegisterNotification(reservation, preAlert, reservationsTitle, channelId);
#endif
    }

    /// <summary>
    /// function to delete a notification for a specific reservation
    /// </summary>
    /// <param name="reservation">reservation for wich to delete the notification</param>
    public void DeleteNotification(IReservation reservation)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        androidNotificationsManager.DeleteNotification(reservation, reservationsTitle, channelId);
#endif
#if !UNITY_EDITOR && UNITY_IOS
        iOSNotificationsManager.DeleteNotification(reservation, reservationsTitle, channelId);
#endif
    }
}
