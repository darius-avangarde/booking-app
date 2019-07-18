using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    private AndroidNotificationChannel androidNotificationChannel;
    private const string channelId = "Default";

    private void CreateNotificationChannel()
    {
        androidNotificationChannel = new AndroidNotificationChannel
        {
            Id = channelId,
            Name = "Default Channel",
            Importance = Importance.High,
            EnableVibration = true,
            LockScreenVisibility = LockScreenVisibility.Public,
            Description = "Reservation notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(androidNotificationChannel);
    }

    private void OnEnable()
    {
        CreateNotificationChannel();
        SendNotification();
        NotificationReceived();
    }

    private void SendNotification()
    {
        AndroidNotification notification = new AndroidNotification();
        notification.Title = "Rezevari apropiate";
        notification.Text = $"first line {Environment.NewLine}  second line";
        notification.FireTime = DateTime.Now.AddSeconds(20);

        int id = AndroidNotificationCenter.SendNotification(notification, channelId);

        //if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
        //{
        //    Debug.Log("scheduled notification");
        //    // Replace the currently scheduled notification with a new notification.
        //    //UpdateScheduledNotifcation(identifier, newNotification);
        //}
        //else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Delivered)
        //{
        //    Debug.Log("delivered notification");
        //    //Remove the notification from the status bar
        //    //CancelNotification(identifier)
        //}

        //List<IReservation> reservations = ReservationDataManager.GetReservationsBetween(DateTime.Today.Date, DateTime.Today.Date.AddDays(30)).ToList();
        //
        //int days = 3;
        //for (int i = 0; i < reservations.Count(); i++)
        //{
        //    AndroidNotification notification = new AndroidNotification();
        //    notification.Title = "Rezevari apropiate";
        //    notification.Text = $"{reservations[i].CustomerName} - {PropertyDataManager.GetProperty(reservations[i].PropertyID).GetRoom(reservations[i].RoomID).Name} in {reservations[i].Period.Start} {Environment.NewLine}";
        //    notification.FireTime = reservations[i].Period.Start.AddDays(-days);
        //
        //    int id = AndroidNotificationCenter.SendNotification(notification, channelId);
        //    //Debug.Log("Notification status = " + AndroidNotificationCenter.CheckScheduledNotificationStatus(id));
        //
        //    if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Unknown)
        //    {
        //        //identifier = AndroidNotificationCenter.SendNotification(notification, "channel_id");
        //    }
        //}
    }

    private void NotificationReceived()
    {
        AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler =
    delegate (AndroidNotificationIntentData data)
    {
        var msg = "Notification received : " + data.Id + "\n";
        msg += "\n Notification received: ";
        msg += "\n .Title: " + data.Notification.Title;
        msg += "\n .Body: " + data.Notification.Text;
        msg += "\n .Channel: " + data.Channel;
        Debug.Log(msg);
    };
        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;
    }
}
