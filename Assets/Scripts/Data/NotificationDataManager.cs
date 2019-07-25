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
    /// save property data to json file
    /// </summary>
    private static void WriteNotificationData()
    {
        string dataAsJson = JsonUtility.ToJson(Data, true);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    public static void DeleteNewNotifications()
    {
        Notification newNotification = new Notification();
        //newNotification.NewNotifications = new List<IReservation>();
    }

    public static void SetNewNotification(List<IReservation> reservationIDs)
    {
        Notification newNotification = new Notification();
        newNotification.SetReservationIDs(reservationIDs);
    }

    public static List<IReservation> GetNewNotifications()
    {
        List<IReservation> reservationsList = new List<IReservation>();
        foreach (string reservationID in Data.notification.NewNotifications)
        {
            reservationsList.Add(ReservationDataManager.GetReservation(reservationID));
        }
        return reservationsList;
    }

    [Serializable]
    private class NotificationData
    {
        public Notification notification;

        public NotificationData()
        {
            this.notification = new Notification();
        }
    }

    [Serializable]
    private class Notification : INotification
    {
        [SerializeField]
        private List<string> newNotifications = new List<string>();
        public List<string> NewNotifications => newNotifications;

        public void SetReservationIDs(List<IReservation> reservationIDs)
        {
            newNotifications = new List<string>();
            for (int i = 0; i < reservationIDs.Count(); i++)
            {
                newNotifications.Add(reservationIDs[0].ID);
            }
        }
    }
}
