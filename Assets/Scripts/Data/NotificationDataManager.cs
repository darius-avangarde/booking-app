using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NotificationDataManager : MonoBehaviour
{
    public const string DATA_FILE_NAME = "NotificationData.json";

    private static NotificationData cache;

    private static NotificationData Data
    {
        get
        {
            if (cache == null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
                if (File.Exists(filePath))
                {
                    string dataAsJson = File.ReadAllText(filePath);
                    cache = JsonUtility.FromJson<NotificationData>(dataAsJson);
                }
                else
                {
                    cache = new NotificationData();
                }
            }
            return cache;
        }
    }

    /// <summary>
    /// save notification data to json file
    /// </summary>
    private static void WriteNotificationData()
    {
        string dataAsJson = JsonUtility.ToJson(Data, true);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    public static INotification AddNotification()
    {
        Notification newNotification = new Notification();
        Data.notification.Add(newNotification);
        WriteNotificationData();
        return newNotification;
    }

    public static INotification GetNotification(string ID)
    {
        return Data.notification.Find(n => n.NotificationID == ID);
    }

    public static List<INotification> GetNotifications()
    {
        List<INotification> notificationsList = new List<INotification>();
        List<Notification> notifications = Data.notification.FindAll(n => !n.Deleted);
        for (int i = 0; i < notifications.Count; i++)
        {
            notificationsList.Add(notifications[i]);
        }
        return notificationsList;
    }

    public static List<INotification> GetNewNotifications()
    {
        List<INotification> notificationsList = new List<INotification>();
        List<Notification> notifications = Data.notification.FindAll(n => n.UnRead && !n.Deleted);
        for (int i = 0; i < notifications.Count; i++)
        {
            notificationsList.Add(notifications[i]);
        }
        return notificationsList;
    }

    public static void SetNotificationReservations(List<IReservation> reservationIDs)
    {
        Notification newNotification = new Notification();
        newNotification.SetReservationIDs(reservationIDs);
    }

    /// <summary>
    /// delete selected notification
    /// </summary>
    /// <param name="ID">notification id</param>
    public static void DeleteNotification(string ID)
    {
        Notification notification = Data.notification.Find(n => n.NotificationID.Equals(ID));
        if (notification != null)
        {
            notification.Deleted = true;
        }
    }

    [Serializable]
    private class NotificationData
    {
        public List<Notification> notification;

        public NotificationData()
        {
            this.notification = new List<Notification>();
        }
    }

    [Serializable]
    private class Notification : INotification
    {
        [SerializeField]
        private string notificationID = null;
        public string NotificationID
        {
            get => notificationID;
            set
            {
                notificationID = value;
                WriteNotificationData();
            }
        }

        [SerializeField]
        private int preAlertTime;
        public int PreAlertTime
        {
            get => preAlertTime;
            set
            {
                preAlertTime = value;
                WriteNotificationData();
            }
        }

        [SerializeField]
        private DateTime fireTime;
        public DateTime FireTime
        {
            get => fireTime;
            set
            {
                fireTime = value;
                WriteNotificationData();
            }
        }

        [SerializeField]
        private bool unRead = true;
        public bool UnRead
        {
            get => unRead;
            set
            {
                unRead = value;
                WriteNotificationData();
            }
        }

        [SerializeField]
        private bool deleted = false;
        public bool Deleted
        {
            get => deleted;
            set
            {
                deleted = value;
                WriteNotificationData();
            }
        }

        [SerializeField]
        private List<string> reservationIDs = new List<string>();
        public List<string> ReservationIDs => reservationIDs;

        public void SetReservationIDs(List<IReservation> reservationsList)
        {
            reservationIDs = new List<string>();
            for (int i = 0; i < reservationsList.Count(); i++)
            {
                reservationIDs.Add(reservationsList[0].ID);
            }
        }
    }
}
