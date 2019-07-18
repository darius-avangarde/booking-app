using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    private AndroidNotificationChannel androidNotificationChannel;
    private const string channelId = "Default";

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void CreateNotificationChannel()
    {
        androidNotificationChannel = new AndroidNotificationChannel
        {
            Id = channelId,
            Name = "Default Channel",
            Importance = Importance.High,
            EnableVibration = true,
            LockScreenVisibility = LockScreenVisibility.Public,
            Description = "Reservations notification",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(androidNotificationChannel);
    }

    private void OnEnable()
    {
        SendNotification();
        NotificationReceived();
    }

    private void SendNotification()
    {
        AndroidNotification notification = new AndroidNotification();
        notification.Title = "Test";
        notification.Text = $"Notification system test";
        notification.FireTime = DateTime.Now.ToLocalTime().AddMinutes(1);

        int identifier = AndroidNotificationCenter.SendNotification(notification, channelId);
        //Debug.Log("Notification status = " + AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier));

        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
        {
            Debug.Log("scheduled notification");
            // Replace the currently scheduled notification with a new notification.
            //UpdateScheduledNotifcation(identifier, newNotification);
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Delivered)
        {
            Debug.Log("delivered notification");
            //Remove the notification from the status bar
            //CancelNotification(identifier)
        }
        else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Unknown)
        {
            Debug.Log("unknown notification");
            //identifier = AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }

        //DateTime firstDay = new DateTime(DateTime.Today.Date.Year, DateTime.Today.Date.Month, 1);
        //DateTime lastDay = new DateTime(DateTime.Today.Date.Year, DateTime.Today.Date.Month, DateTime.DaysInMonth(DateTime.Today.Date.Year, DateTime.Today.Date.Month));
        //int days = 3;
        //foreach (var reservation in ReservationDataManager.GetReservationsBetween(firstDay, lastDay))
        //{
        //    AndroidNotification notification = new AndroidNotification();
        //    notification.Title = "Rezevari apropiate";
        //    notification.Text = $"{reservation.CustomerName} - {reservation.room} in {reservation.Period.Start} ";
        //    notification.FireTime = reservation.Period.Start.AddDays(-days);
        //
        //    int id = AndroidNotificationCenter.SendNotification(notification, channelId);
        //    Debug.Log("Notification status = " + AndroidNotificationCenter.CheckScheduledNotificationStatus(id));
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
