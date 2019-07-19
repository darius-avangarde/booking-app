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

    private string reservationsGroup = "Today's Reservations:";
    private int days = 3;

    private void Start()
    {
        AndroidNotificationCenter.GetLastNotificationIntent();
        CreateNotificationChannel();
        NotificationReceived();
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
    /// 
    /// </summary>
    /// <param name="reservation"></param>
    public void RegisterNotification(IReservation reservation)
    {
        int notificationID = -1;//reservation.NotificationID;
        if (notificationID != -1)
        {
            //check for reservation with current notification ID
            //update current notification or delete it
            List<IReservation> previousReservations = ReservationDataManager.GetReservations().Where(r => /*r.notificationID == notificationID && */r.ID != reservation.ID).ToList();
            if (previousReservations.Count() > 0)
            {
                AndroidNotification newNotification = new AndroidNotification();
                newNotification.Title = "Rezevari apropiate:";
                //foreach reservation from the current reservation start date, set notification text
                foreach (IReservation res in previousReservations)
                {
                    newNotification.Text = $"{res.CustomerName} - {PropertyDataManager.GetProperty(res.PropertyID).GetRoom(res.RoomID).Name} in {res.Period.Start}.\n";
                }
                newNotification.FireTime = reservation.Period.Start.AddDays(-days);

                AndroidNotificationCenter.UpdateScheduledNotification(notificationID, newNotification, channelId);
            }
            else
            {
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
            //notificationID = withoutThisReservation[0].notificationID;
        }

        AndroidNotification notification = new AndroidNotification();
        notification.Title = "Rezevari apropiate:";
        //foreach reservation from the current reservation start date, set notification text
        foreach (IReservation res in allReservations)
        {
            notification.Text = $"{res.CustomerName} - {PropertyDataManager.GetProperty(res.PropertyID).GetRoom(res.RoomID).Name} in {res.Period.Start}.\n";
        }
        notification.FireTime = reservation.Period.Start.AddDays(-days);

        if (notificationID != -1)
        {
            AndroidNotificationCenter.UpdateScheduledNotification(notificationID, notification, channelId);
        }
        else
        {
            notificationID = AndroidNotificationCenter.SendNotification(notification, channelId);
        }
        Debug.Log("Notification status = " + AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationID));
        //reservation.notificationID = notificationID;
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
