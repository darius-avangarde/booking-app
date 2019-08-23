﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UINavigation;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class iOSNotificationsManager : MonoBehaviour
{
#if UNITY_IOS
    private int preAlertTime = 0;

    /// <summary>
    /// initialize iOS notification data
    /// open notification screen if the app is opened from a notification
    /// </summary>
    /// <param name="navigator">navigator refference</param>
    /// <param name="notificationsScreen">notifications screeen  refference</param>
    /// <param name="notificationBadge">notification badge refference</param>
    /// <param name="channelId">channel ID name</param>
    public void Initialize(Navigator navigator, NotificationsScreen notificationsScreen, NotificationBadge notificationBadge, string channelId)
    {
        iOSNotification notification = iOSNotificationCenter.GetLastRespondedNotification();
        if (notification != null)
        {
            List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notification.Identifier).ToList();
            notificationsScreen.AddNewReservations(newReservations);
            DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
            navigator.GoTo(notificationsScreen.GetComponent<NavScreen>());
        }
        else
        {
            CheckDeliveredNotifications(notificationsScreen, notificationBadge);
        }
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.ApplicationBadge = 0;
        StartCoroutine(RequestAuthorization());
        NotificationReceivedEvent(notificationsScreen, notificationBadge);
    }

    /// <summary>
    /// regiser an iOS notification
    /// set timespan for notification fire time
    /// set notification text
    /// update or create a new notification
    /// </summary>
    /// <param name="reservation"></param>
    /// <param name="preAlert"></param>
    /// <param name="title"></param>
    /// <param name="channelId"></param>
    public void RegisterNotification(IReservation reservation, int preAlert, string title, string channelId)
    {
        preAlertTime = preAlert;
        string notificationID = reservation.NotificationID;
        INotification notificationItem = NotificationDataManager.GetNotification(notificationID);
        DateTime reservationStart = reservation.Period.Start.Date.AddHours(12);
        DateTime fireTime = reservationStart.AddHours(-preAlert);
        TimeSpan notificationTime = new TimeSpan(0, 0, 1);
        if (fireTime >= DateTime.Now)
        {
            notificationTime = fireTime - DateTime.Now;
        }

        iOSNotificationTimeIntervalTrigger timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = notificationTime,
            Repeats = false
        };
        if (!string.IsNullOrEmpty(notificationID))
        {
            //check for reservation with current notification ID
            //update current notification or delete it
            List<IReservation> previousReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationID && r.ID != reservation.ID).ToList();
            if (previousReservations.Count() > 0)
            {
                iOSNotification newNotification = new iOSNotification();
                newNotification.Title = title;
                for (int i = 0; i < previousReservations.Count; i++)
                {
                    newNotification.Body += $"{previousReservations[i].CustomerName} - {PropertyDataManager.GetProperty(previousReservations[i].PropertyID).Name}, {LocalizedText.Instance.RoomItem} {PropertyDataManager.GetProperty(previousReservations[i].PropertyID).GetRoom(previousReservations[i].RoomID).Name} in {previousReservations[i].Period.Start.Day}/{previousReservations[i].Period.Start.Month}/{previousReservations[i].Period.Start.Year}.\n";
                }
                newNotification.ShowInForeground = true;
                newNotification.ForegroundPresentationOption = (PresentationOption.Alert & PresentationOption.Badge & PresentationOption.Sound);
                newNotification.Badge = previousReservations.Count;
                newNotification.CategoryIdentifier = "Reservations";
                newNotification.ThreadIdentifier = channelId;
                newNotification.Trigger = timeTrigger;

                if (reservationStart > DateTime.Now)
                {
                    ReplaceNotification(notificationID, newNotification);
                    if (notificationItem != null)
                    {
                        notificationItem.UnRead = true;
                        notificationItem.FireTime = fireTime;
                        notificationItem.SetReservationIDs(previousReservations);
                    }
                    else
                    {
                        notificationItem = NotificationDataManager.AddNotification();
                        notificationItem.NotificationID = notificationID;
                        notificationItem.UnRead = true;
                        notificationItem.PreAlertTime = preAlert;
                        notificationItem.FireTime = fireTime;
                        notificationItem.SetReservationIDs(previousReservations);
                    }
                }
            }
            else
            {
                iOSNotificationCenter.RemoveScheduledNotification(notificationID);
                iOSNotificationCenter.RemoveDeliveredNotification(notificationID);

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
        iOSNotification notification = new iOSNotification();
        notification.Title = title;
        for (int i = 0; i < allReservations.Count; i++)
        {
            notification.Body += $"{allReservations[i].CustomerName} - {PropertyDataManager.GetProperty(allReservations[i].PropertyID).Name}, {LocalizedText.Instance.RoomItem} {PropertyDataManager.GetProperty(allReservations[i].PropertyID).GetRoom(allReservations[i].RoomID).Name} in {allReservations[i].Period.Start.Day}/{allReservations[i].Period.Start.Month}/{allReservations[i].Period.Start.Year}.\n";
        }
        notification.ShowInForeground = true;
        notification.ForegroundPresentationOption = (PresentationOption.Alert & PresentationOption.Badge & PresentationOption.Sound);
        notification.Badge = allReservations.Count;
        notification.CategoryIdentifier = "Reservations";
        notification.ThreadIdentifier = channelId;
        notification.Trigger = timeTrigger;

        if (!string.IsNullOrEmpty(notificationID))
        {
            if (reservationStart > DateTime.Now)
            {
                ReplaceNotification(notificationID, notification);
                if (notificationItem != null)
                {
                    notificationItem.UnRead = true;
                    notificationItem.FireTime = fireTime;
                    notificationItem.SetReservationIDs(allReservations);
                }
                else
                {
                    notificationItem = NotificationDataManager.AddNotification();
                    notificationItem.NotificationID = notificationID;
                    notificationItem.UnRead = true;
                    notificationItem.PreAlertTime = preAlert;
                    notificationItem.FireTime = fireTime;
                    notificationItem.SetReservationIDs(allReservations);
                }
                reservation.NotificationID = notificationID;
            }
        }
        else
        {
            if (reservationStart > DateTime.Now)
            {
                iOSNotificationCenter.ScheduleNotification(notification);
                notificationID = notification.Identifier;

                notificationItem = NotificationDataManager.AddNotification();
                notificationItem.NotificationID = notificationID;
                notificationItem.UnRead = true;
                notificationItem.PreAlertTime = preAlert;
                notificationItem.FireTime = fireTime;
                notificationItem.SetReservationIDs(allReservations);
                reservation.NotificationID = notificationID;
            }
        }
    }

    /// <summary>
    /// delete notification function for a specific reservation
    /// </summary>
    /// <param name="reservation">reservation for notification to delete</param>
    /// <param name="title">notification title string</param>
    /// <param name="channelId">notification channel ID name</param>
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
            if (iOSNotificationCenter.GetScheduledNotifications().Any(n => n.Identifier == reservation.NotificationID))
            {
                iOSNotificationCenter.RemoveScheduledNotification(reservation.NotificationID);
            }
            NotificationDataManager.DeleteNotification(reservation.NotificationID);
        }
        else
        {
            List<IReservation> allReservations = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date == reservation.Period.Start.Date && r.NotificationID == notificationID && r.ID != reservation.ID).ToList();
            TimeSpan notificationTime = new TimeSpan(0, 0, 1);
            if (notificationItem.FireTime >= DateTime.Now)
            {
                notificationTime = notificationItem.FireTime - DateTime.Now;
            }
            iOSNotificationTimeIntervalTrigger timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = notificationTime,
                Repeats = false
            };
            iOSNotification notification = new iOSNotification();
            notification.Title = title;
            for (int i = 0; i < allReservations.Count; i++)
            {
                notification.Body += $"{allReservations[i].CustomerName} - {PropertyDataManager.GetProperty(allReservations[i].PropertyID).Name}, {LocalizedText.Instance.RoomItem} {PropertyDataManager.GetProperty(allReservations[i].PropertyID).GetRoom(allReservations[i].RoomID).Name} in {allReservations[i].Period.Start.Day}/{allReservations[i].Period.Start.Month}/{allReservations[i].Period.Start.Year}.\n";
            }
            notification.ShowInForeground = true;
            notification.ForegroundPresentationOption = (PresentationOption.Alert & PresentationOption.Badge & PresentationOption.Sound);
            notification.Badge = allReservations.Count;
            notification.CategoryIdentifier = "Reservations";
            notification.ThreadIdentifier = channelId;
            notification.Trigger = timeTrigger;

            ReplaceNotification(notificationID, notification);
            notificationItem.UnRead = true;
            notificationItem.SetReservationIDs(allReservations);
        }
    }

    /// <summary>
    /// function to delete a previous reservation and replace it with a new updated one
    /// </summary>
    /// <param name="ID">notification ID</param>
    /// <param name="notification">new notification data</param>
    private void ReplaceNotification(string ID, iOSNotification notification)
    {
        if (iOSNotificationCenter.GetScheduledNotifications().Any(n => n.Identifier == ID))
        {
            iOSNotificationCenter.RemoveScheduledNotification(ID);
        }
        notification.Identifier = ID;
        iOSNotificationCenter.ScheduleNotification(notification);
    }

    /// <summary>
    /// function to request notification authorization from the OS
    /// </summary>
    /// <returns></returns>
    private IEnumerator RequestAuthorization()
    {
        using (AuthorizationRequest req = new AuthorizationRequest(AuthorizationOption.Alert & AuthorizationOption.Badge & AuthorizationOption.Sound, false))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization: \n";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
        }
    }

    /// <summary>
    /// check previous delivered notifications
    /// update notification badge
    /// </summary>
    /// <param name="notificationsScreen">notification screen component</param>
    /// <param name="notificationBadge">notification badge component</param>
    private void CheckDeliveredNotifications(NotificationsScreen notificationsScreen, NotificationBadge notificationBadge)
    {
        //iOSNotification[] notifications = iOSNotificationCenter.GetDeliveredNotifications();
        //for (int i = 0; i < notifications.Length; i++)
        //{
        //    if (NotificationDataManager.GetNotification(notifications[i].Identifier).UnRead)
        //    {
        //        List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notifications[i].Identifier).ToList();
        //        notificationsScreen.AddNewReservations(newReservations);
        //        DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
        //    }
        //}

        List<INotification> notifications = NotificationDataManager.GetNewNotifications();
        for (int i = 0; i < notifications.Count; i++)
        {
            if (notifications[i].FireTime < DateTime.Now)
            {
                List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notifications[i].NotificationID).ToList();
                notificationsScreen.AddNewReservations(newReservations);
                DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
            }
        }
    }

    /// <summary>
    /// event for a notification received with the app opened
    /// set the notification badge from the main screen
    /// </summary>
    /// <param name="notificationsScreen"></param>
    /// <param name="notificationBadge"></param>
    private void NotificationReceivedEvent(NotificationsScreen notificationsScreen, NotificationBadge notificationBadge)
    {
        iOSNotificationCenter.OnNotificationReceived += notification =>
        {
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
            List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notification.Identifier).ToList();
            notificationsScreen.AddNewReservations(newReservations);
            DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { StartCoroutine(notificationBadge.SetNotificationBadge(newReservations.Count)); });
        };
    }
#endif
}