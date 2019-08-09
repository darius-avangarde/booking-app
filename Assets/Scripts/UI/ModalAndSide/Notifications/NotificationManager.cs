using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using Unity.Notifications.Android;
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

    private AndroidNotificationChannel androidNotificationChannel;
    private const string channelId = "Default";

    private string reservationsTitle = "Rezervări apropiate:";
    private string lastReservationsNotification;
    private int preAlertTime = 0;

#if !UNITY_EDITOR
    private void Start()
    {
        AndroidNotificationIntentData notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationIntentData.Id).ToList();
            notificationsScreen.AddNewReservations(newReservations);
            DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
            navigator.GoTo(notificationsScreen.GetComponent<NavScreen>());
        }
        else
        {
            CheckDeliveredNotification();
        }
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        CreateNotificationChannel();
        NotificationReceivedEvent();
    }
#endif

    private void CreateNotificationChannel()
    {
        androidNotificationChannel = new AndroidNotificationChannel
        {
            Id = channelId,
            Name = "Default Channel",
            Importance = Importance.Default,
            CanShowBadge = true,
            EnableLights = true,
            EnableVibration = true,
            LockScreenVisibility = LockScreenVisibility.Public,
            Description = "Reservation notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(androidNotificationChannel);
    }

    /// <summary>
    /// call this when you create or edit a reservation to update the current set notifications
    /// </summary>
    /// <param name="reservation">the new reservation after add or edit</param>
    /// <param name="enumValue">value for period before a notification fires</param>
    public void RegisterNotification(IReservation reservation, int preAlert)
    {
        preAlertTime = preAlert;
        int notificationID = reservation.NotificationID;
        INotification notificationItem = NotificationDataManager.GetNotification(notificationID);
        DateTime fireTime = reservation.Period.Start.Date.AddHours(12);
        if (notificationID > 0)
        {
            //check for reservation with current notification ID
            //update current notification or delete it
            List<IReservation> previousReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationID && r.ID != reservation.ID).ToList();
            if (previousReservations.Count() > 0)
            {
                AndroidNotification newNotification = new AndroidNotification();
                newNotification.Title = reservationsTitle;
                for (int i = 0; i < previousReservations.Count; i++)
                {
                    newNotification.Text += $"{previousReservations[i].CustomerName} - {PropertyDataManager.GetProperty(previousReservations[i].PropertyID).Name}, {PropertyDataManager.GetProperty(previousReservations[i].PropertyID).GetRoom(previousReservations[i].RoomID).Name} in {previousReservations[i].Period.Start.Day}/{previousReservations[i].Period.Start.Month}/{previousReservations[i].Period.Start.Year}.\n";
                    newNotification.IntentData += $"{previousReservations[i].ID}\n";
                }
                newNotification.FireTime = fireTime.AddHours(-preAlert);
                //newNotification.FireTime = DateTime.Now.AddMinutes(1);
                newNotification.Style = NotificationStyle.BigTextStyle;

                ReplaceNotification(notificationID, newNotification, channelId);
                if (notificationItem != null)
                {
                    notificationItem.UnRead = true;
                    notificationItem.FireTime = newNotification.FireTime;
                    notificationItem.SetReservationIDs(previousReservations);
                }
                else
                {
                    notificationItem = NotificationDataManager.AddNotification();
                    notificationItem.NotificationID = notificationID;
                    notificationItem.UnRead = true;
                    notificationItem.PreAlertTime = preAlert;
                    notificationItem.FireTime = newNotification.FireTime;
                    notificationItem.SetReservationIDs(previousReservations);
                }
            }
            else
            {
                AndroidNotificationCenter.CancelNotification(notificationID);
                if (notificationItem != null)
                {
                    NotificationDataManager.DeleteNotification(notificationItem.NotificationID);
                }
            }
            notificationItem = null;
            notificationID = -1;
        }
        //check new start period for other notifications
        //get the new ID and set it to current reservation or create new notification and set the ID
        List<IReservation> allReservations = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date == reservation.Period.Start.Date).ToList();
        IReservation otherReservation = allReservations.Find(r => r.ID != reservation.ID);

        if (otherReservation != null)
        {
            if (NotificationDataManager.GetNotification(otherReservation.NotificationID).PreAlertTime == preAlertTime)
            {
                notificationID = otherReservation.NotificationID;
                notificationItem = NotificationDataManager.GetNotification(notificationID);
            }
        }

        AndroidNotification notification = new AndroidNotification();
        notification.Title = reservationsTitle;
        for (int i = 0; i < allReservations.Count; i++)
        {
            notification.Text += $"{allReservations[i].CustomerName} - {PropertyDataManager.GetProperty(allReservations[i].PropertyID).GetRoom(allReservations[i].RoomID).Name} in {allReservations[i].Period.Start.Day}/{allReservations[i].Period.Start.Month}/{allReservations[i].Period.Start.Year}.\n";
            notification.IntentData += $"{allReservations[i].ID} & \n";
        }
        notification.FireTime = fireTime.AddHours(-preAlert);
        //notification.FireTime = DateTime.Now.AddMinutes(1);
        notification.Style = NotificationStyle.BigTextStyle;

        if (notificationID > 0)
        {
            ReplaceNotification(notificationID, notification, channelId);
            if (notificationItem != null)
            {
                notificationItem.UnRead = true;
                notificationItem.FireTime = notification.FireTime;
                notificationItem.SetReservationIDs(allReservations);
            }
            else
            {
                notificationItem = NotificationDataManager.AddNotification();
                notificationItem.NotificationID = notificationID;
                notificationItem.UnRead = true;
                notificationItem.PreAlertTime = preAlert;
                notificationItem.FireTime = notification.FireTime;
                notificationItem.SetReservationIDs(allReservations);
            }
        }
        else
        {
            notificationID = AndroidNotificationCenter.SendNotification(notification, channelId);

            notificationItem = NotificationDataManager.AddNotification();
            notificationItem.NotificationID = notificationID;
            notificationItem.UnRead = true;
            notificationItem.PreAlertTime = preAlert;
            notificationItem.FireTime = notification.FireTime;
            notificationItem.SetReservationIDs(allReservations);
        }
        reservation.NotificationID = notificationID;
    }

    private void ReplaceNotification(int ID, AndroidNotification notification, string channel)
    {
        AndroidNotificationCenter.CancelNotification(ID);
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, channel, ID);
    }

    /// <summary>
    /// when the time before a notification is set in the set-up menu, 
    /// this function should be called to update all current notifications according to new setting
    /// </summary>
    public void UpdateAllNotifications(int previousPreAlert, int newPreAlert)
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

    public void DeleteNotification(IReservation reservation)
    {
        int notificationID = reservation.NotificationID;
        if (notificationID <= 0)
        {
            return;
        }
        INotification notificationItem = NotificationDataManager.GetNotification(notificationID);
        IReservation otherReservation = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date == reservation.Period.Start.Date).ToList().Find(r => r.ID != reservation.ID);


        if (otherReservation == null)
        {
            AndroidNotificationCenter.CancelNotification(reservation.NotificationID);
            NotificationDataManager.DeleteNotification(reservation.NotificationID);
        }
        else
        {
            List<IReservation> allReservations = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date == reservation.Period.Start.Date && r.NotificationID == notificationID && r.ID != reservation.ID).ToList();

            AndroidNotification notification = new AndroidNotification();
            notification.Title = reservationsTitle;
            for (int i = 0; i < allReservations.Count; i++)
            {
                notification.Text += $"{allReservations[i].CustomerName} - {PropertyDataManager.GetProperty(allReservations[i].PropertyID).GetRoom(allReservations[i].RoomID).Name} in {allReservations[i].Period.Start.Day}/{allReservations[i].Period.Start.Month}/{allReservations[i].Period.Start.Year}.\n";
                notification.IntentData += $"{allReservations[i].ID} & \n";
            }
            notification.FireTime = notificationItem.FireTime;
            notification.Style = NotificationStyle.BigTextStyle;

            ReplaceNotification(notificationID, notification, channelId);

            notificationItem.UnRead = true;
            notificationItem.SetReservationIDs(allReservations);
        }
    }

    private void CheckDeliveredNotification()
    {
        List<INotification> notifications = NotificationDataManager.GetNewNotifications();
        for (int i = 0; i < notifications.Count; i++)
        {
            if (notifications[i].FireTime < DateTime.Now)
            {
                AndroidNotificationChannel notificationChannel = AndroidNotificationCenter.GetNotificationChannel(channelId);
                List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notifications[i].NotificationID).ToList();
                notificationsScreen.AddNewReservations(newReservations);
                DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
            }
        }
    }

    private void NotificationReceivedEvent()
    {
        AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler =
            delegate (AndroidNotificationIntentData data)
            {
                AndroidNotificationCenter.CancelAllDisplayedNotifications();
                List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == data.Id).ToList();
                notificationsScreen.AddNewReservations(newReservations);
                DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
            };
        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;
    }
}
