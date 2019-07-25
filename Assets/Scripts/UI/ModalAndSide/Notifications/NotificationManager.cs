using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using Unity.Notifications.Android;
using UnityEngine;
using System.Text;

public class NotificationManager : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private NotificationsScreen notificationsScreen = null;

    private AndroidNotificationChannel androidNotificationChannel;
    private const string channelId = "Default";

    private string reservationsGroup = "Rezevari apropiate:";
    private string lastReservationsNotification;

#if !UNITY_EDITOR
    private void Start()
    {
        AndroidNotificationIntentData notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationIntentData.Id).ToList();
            notificationsScreen.AddNewNotification(newReservations);
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
    public void RegisterNotification(IReservation reservation, int enumValue = 3)
    {
        int notificationID = reservation.NotificationID;
        DateTime fireTime = reservation.Period.Start.Date.AddHours(12);
        if (notificationID != -1)
        {
            //check for reservation with current notification ID
            //update current notification or delete it
            List<IReservation> previousReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationID && r.ID != reservation.ID).ToList();
            if (previousReservations.Count() > 0)
            {
                AndroidNotification newNotification = new AndroidNotification();
                newNotification.Title = reservationsGroup;
                foreach (IReservation prevReservation in previousReservations)
                {
                    newNotification.Text += $"{prevReservation.CustomerName} - {PropertyDataManager.GetProperty(prevReservation.PropertyID).GetRoom(prevReservation.RoomID).Name} in {prevReservation.Period.Start.Day}/{prevReservation.Period.Start.Month}/{prevReservation.Period.Start.Year}.\n";
                    newNotification.IntentData += $"{prevReservation.ID}\n";
                }
                //notification.FireTime = fireTime.AddHours(-24);
                newNotification.FireTime = DateTime.Now.AddMinutes(1);
                newNotification.Style = NotificationStyle.BigTextStyle;

                Debug.Log($"notification update: {notificationID}");
                ReplaceNotification(notificationID, newNotification, channelId);
                //AndroidNotificationCenter.UpdateScheduledNotification(notificationID, newNotification, channelId);
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
        List<IReservation> allReservations = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date.AddHours(12) >= DateTime.Today.Date.AddHours(12) && r.Period.Start.Date.AddHours(12) < DateTime.Today.Date.AddHours(12 + 24)).OrderBy(r => r.Period.Start).ToList();
        List<IReservation> otherReservations = allReservations.Where(r => r.ID != reservation.ID).ToList();

        if (otherReservations.Count() > 0)
        {
            notificationID = otherReservations[0].NotificationID;
        }

        AndroidNotification notification = new AndroidNotification();
        notification.Title = reservationsGroup;
        foreach (IReservation res in allReservations)
        {
            notification.Text += $"{res.CustomerName} - {PropertyDataManager.GetProperty(res.PropertyID).GetRoom(res.RoomID).Name} in {res.Period.Start.Day}/{res.Period.Start.Month}/{res.Period.Start.Year}.\n";
            notification.IntentData += $"{res.ID} & \n";
        }
        //notification.FireTime = fireTime.AddHours(-24);
        notification.FireTime = DateTime.Now.AddMinutes(1);
        notification.Style = NotificationStyle.BigTextStyle;

        if (notificationID != -1)
        {
            Debug.Log($"notification update: {notificationID}");
            ReplaceNotification(notificationID, notification, channelId);
            //AndroidNotificationCenter.UpdateScheduledNotification(notificationID, notification, channelId);
        }
        else
        {
            Debug.Log($"notification send: {notificationID}");
            notificationID = AndroidNotificationCenter.SendNotification(notification, channelId);
        }
        reservation.NotificationID = notificationID;
        //Debug.Log("Notification status = " + AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationID));
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
    public void UpdateAllNotifications()
    {
        List<IReservation> previousReservations = ReservationDataManager.GetReservations().ToList();
        foreach (IReservation reservation in previousReservations)
        {
            RegisterNotification(reservation);
        }
    }

    private void CheckDeliveredNotification()
    {
        List<IReservation> reservations = ReservationDataManager.GetReservations().Where(r => r.Period.Start.Date == DateTime.Today.Date).ToList();
        //Debug.Log($"reservations today: {reservations.Count}");
        foreach (var reservation in reservations)
        {
            int notificationID = reservation.NotificationID;
            //Debug.Log($"notification ID: {notificationID}");
            if(notificationID != 0 && AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationID) == NotificationStatus.Delivered)
            {
                List<IReservation> newReservations = ReservationDataManager.GetReservations().Where(r => r.NotificationID == notificationID).ToList();
                notificationsScreen.AddNewNotification(newReservations);
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
                notificationsScreen.AddNewNotification(newReservations);
            };
        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;
    }
}
