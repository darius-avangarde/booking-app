using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UINavigation;

public class AndroidNotificationsManager : MonoBehaviour
{
    private AndroidNotificationChannel androidNotificationChannel;
    private int preAlertTime = 0;

    public void Initialize(Navigator navigator, NotificationsScreen notificationsScreen, NotificationBadge notificationBadge, string channelId)
    {
        AndroidNotificationIntentData notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationIntentData.Id.ToString()).ToList();
            notificationsScreen.AddNewReservations(newReservations);
            DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
            navigator.GoTo(notificationsScreen.GetComponent<NavScreen>());
        }
        else
        {
            CheckDeliveredNotification(notificationsScreen, notificationBadge, channelId);
        }
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        CreateNotificationChannel(channelId);
        NotificationReceivedEvent(notificationsScreen, notificationBadge);
    }

    public void RegisterNotification(IReservation reservation, int preAlert, string title, string channelId)
    {
        preAlertTime = preAlert;
        string notificationID = reservation.NotificationID;
        INotification notificationItem = NotificationDataManager.GetNotification(notificationID);
        DateTime reservationStart = reservation.Period.Start.Date.AddHours(12);
        if (!string.IsNullOrEmpty(notificationID))
        {
            //check for reservation with current notification ID
            //update current notification or delete it
            List<IReservation> previousReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationID && r.ID != reservation.ID).ToList();
            if (previousReservations.Count() > 0)
            {
                AndroidNotification newNotification = new AndroidNotification();
                newNotification.Title = title;
                for (int i = 0; i < previousReservations.Count; i++)
                {
                    newNotification.Text += $"{previousReservations[i].CustomerName} - {PropertyDataManager.GetProperty(previousReservations[i].PropertyID).Name}, {PropertyDataManager.GetProperty(previousReservations[i].PropertyID).GetRoom(previousReservations[i].RoomID).Name} in {previousReservations[i].Period.Start.Day}/{previousReservations[i].Period.Start.Month}/{previousReservations[i].Period.Start.Year}.\n";
                    newNotification.IntentData += $"{previousReservations[i].ID} & \n";
                }
                newNotification.FireTime = reservationStart.AddHours(-preAlert);
                newNotification.Style = NotificationStyle.BigTextStyle;

                if (reservationStart > DateTime.Now)
                {
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
            }
            else
            {
                AndroidNotificationCenter.CancelNotification(int.Parse(notificationID));
                if (notificationItem != null)
                {
                    NotificationDataManager.DeleteNotification(notificationItem.NotificationID);
                }
            }
            notificationItem = null;
            notificationID = null;
        }
        //check new start period for other notifications
        //get the new ID and set it to current reservation or create new notification and set the ID
        List<IReservation> allReservations = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date == reservation.Period.Start.Date).ToList();
        IReservation otherReservation = allReservations.Find(r => r.ID != reservation.ID);

        if (otherReservation != null && !string.IsNullOrEmpty(otherReservation.NotificationID))
        {
            if (NotificationDataManager.GetNotification(otherReservation.NotificationID).PreAlertTime == preAlertTime)
            {
                notificationID = otherReservation.NotificationID;
                notificationItem = NotificationDataManager.GetNotification(notificationID);
            }
        }

        AndroidNotification notification = new AndroidNotification();
        notification.Title = title;
        for (int i = 0; i < allReservations.Count; i++)
        {
            notification.Text += $"{allReservations[i].CustomerName} - {PropertyDataManager.GetProperty(allReservations[i].PropertyID).GetRoom(allReservations[i].RoomID).Name} in {allReservations[i].Period.Start.Day}/{allReservations[i].Period.Start.Month}/{allReservations[i].Period.Start.Year}.\n";
            notification.IntentData += $"{allReservations[i].ID} & \n";
        }
        notification.FireTime = reservationStart.AddHours(-preAlert);
        notification.Style = NotificationStyle.BigTextStyle;

        if (!string.IsNullOrEmpty(notificationID))
        {
            if (reservationStart > DateTime.Now)
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
                reservation.NotificationID = notificationID;
            }
        }
        else
        {
            if (reservationStart > DateTime.Now)
            {
                notificationID = AndroidNotificationCenter.SendNotification(notification, channelId).ToString();

                notificationItem = NotificationDataManager.AddNotification();
                notificationItem.NotificationID = notificationID;
                notificationItem.UnRead = true;
                notificationItem.PreAlertTime = preAlert;
                notificationItem.FireTime = notification.FireTime;
                notificationItem.SetReservationIDs(allReservations);
                reservation.NotificationID = notificationID;
            }
        }
    }

    public void DeleteNotification(IReservation reservation, string title, string channelId)
    {
        string notificationID = reservation.NotificationID;
        if (!string.IsNullOrEmpty(notificationID))
        {
            return;
        }
        INotification notificationItem = NotificationDataManager.GetNotification(notificationID);
        IReservation otherReservation = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date == reservation.Period.Start.Date).ToList().Find(r => r.ID != reservation.ID);


        if (otherReservation == null)
        {
            AndroidNotificationCenter.CancelNotification(int.Parse(reservation.NotificationID));
            NotificationDataManager.DeleteNotification(reservation.NotificationID);
        }
        else
        {
            List<IReservation> allReservations = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date == reservation.Period.Start.Date && r.NotificationID == notificationID && r.ID != reservation.ID).ToList();
            AndroidNotification notification = new AndroidNotification();
            notification.Title = title;
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

    private void ReplaceNotification(string ID, AndroidNotification notification, string channel)
    {
        int id = int.Parse(ID);
        AndroidNotificationCenter.CancelNotification(id);
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, channel, id);
    }

    private void CreateNotificationChannel(string channelId)
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

    private void CheckDeliveredNotification(NotificationsScreen notificationsScreen, NotificationBadge notificationBadge, string channelId)
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

    private void NotificationReceivedEvent(NotificationsScreen notificationsScreen, NotificationBadge notificationBadge)
    {
        AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler =
            delegate (AndroidNotificationIntentData data)
            {
                AndroidNotificationCenter.CancelAllDisplayedNotifications();
                List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => int.Parse(r.NotificationID) == data.Id).ToList();
                notificationsScreen.AddNewReservations(newReservations);
                DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
            };
        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;
    }
}
