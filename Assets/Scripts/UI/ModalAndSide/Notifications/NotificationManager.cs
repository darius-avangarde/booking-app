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

    private string reservationsGroup = "Rezevari apropiate:";
    private string lastReservationsNotification;

    private void Start()
    {
        AndroidNotificationCenter.GetLastNotificationIntent();
        CreateNotificationChannel();
    }

    private void TestNotification()
    {
        AndroidNotification notification = new AndroidNotification();
        notification.Title = reservationsGroup;
        notification.Text = $"Sergiu Test - Camera Test in {DateTime.Now.Date.AddSeconds(-20)}.\n";
        notification.FireTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 14, 0, 0);

        AndroidNotificationCenter.SendNotification(notification, channelId);
    }

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
    public void RegisterNotification(IReservation reservation, int enumValue = 3)
    {
        int notificationID = reservation.NotificationID;
        if (notificationID != -1)
        {
            //check for reservation with current notification ID
            //update current notification or delete it
            List<IReservation> previousReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationID && r.ID != reservation.ID).ToList();
            if (previousReservations.Count() > 0)
            {
                AndroidNotification newNotification = new AndroidNotification();
                newNotification.Title = reservationsGroup;
                foreach (IReservation res in previousReservations)
                {
                    newNotification.Text = $"{res.CustomerName} - {PropertyDataManager.GetProperty(res.PropertyID).GetRoom(res.RoomID).Name} in {res.Period.Start}.\n";
                    newNotification.IntentData = $"{res.ID}\n";
                }
                newNotification.FireTime = new DateTime( reservation.Period.Start.Year, reservation.Period.Start.Month, reservation.Period.Start.Day, 15, 20, 0);

                Debug.Log($"notification update: {notificationID}");
                AndroidNotificationCenter.UpdateScheduledNotification(notificationID, newNotification, channelId);
            }
            else
            {
                Debug.Log($"notification canceled");
                AndroidNotificationCenter.CancelNotification(notificationID);
            }
            notificationID = -1;
        }
        //check new start period for other notifications
        //get the new ID and set it to current reservation or create new notification and set the ID
        List<IReservation> allReservations = ReservationDataManager.GetReservationsBetween(reservation.Period.Start, reservation.Period.Start.AddDays(1)).ToList();
        List<IReservation> otherReservations = allReservations.Where(r => r.ID != reservation.ID).ToList();

        if (otherReservations.Count() > 0)
        {
            notificationID = otherReservations[0].NotificationID;
        }

        AndroidNotification notification = new AndroidNotification();
        notification.Title = reservationsGroup;
        foreach (IReservation res in allReservations)
        {
            notification.Text = $"{res.CustomerName} - {PropertyDataManager.GetProperty(res.PropertyID).GetRoom(res.RoomID).Name} in {res.Period.Start}.\n";
            notification.IntentData = $"{res.ID} & \n";
        }
        //notification.FireTime = reservation.Period.Start.AddDays(-enumValue);
        notification.FireTime = new DateTime(reservation.Period.Start.Year, reservation.Period.Start.Month, reservation.Period.Start.Day, 15, 0, 0);

        if (notificationID != -1)
        {
            Debug.Log($"notification update: {notificationID}");
            AndroidNotificationCenter.UpdateScheduledNotification(notificationID, notification, channelId);
        }
        else
        {
            Debug.Log($"notification send: {notificationID}");
            notificationID = AndroidNotificationCenter.SendNotification(notification, channelId);
        }
        reservation.NotificationID = notificationID;
        //Debug.Log("Notification status = " + AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationID));
    }

    /// <summary>
    /// when the time before a notification is set in the set-up menu, 
    /// this function should be called to update all current notifications according to new setting
    /// </summary>
    public void UpdateAllNotifications()
    {
        List<IReservation> previousReservations = ReservationDataManager.GetReservations().ToList();
        foreach (IReservation reservation in previousReservations)
        {
            RegisterNotification(reservation);
        }
    }

    private void UpdateScheduledNotifcation(int id, AndroidNotification newNotification)
    {
        AndroidNotificationCenter.CancelNotification(id);
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
