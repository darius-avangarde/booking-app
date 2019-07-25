using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBadge : MonoBehaviour
{
    [SerializeField]
    private NotificationsScreen NotificationsScreen = null;
    [SerializeField]
    private GameObject notificationsBadgeObject = null;
    [SerializeField]
    private Text notificationsBadgeText = null;

    private void Start()
    {
        NotificationsScreen.SetNewNotifications += SetNotificationBadge;
        SetNotificationBadge(NotificationsScreen.GetNotificationsCount());
    }

    private void OnEnable()
    {
        SetNotificationBadge(NotificationsScreen.GetNotificationsCount());
    }

    public void SetNotificationBadge(int newNotifications)
    {
        if (newNotifications > 0)
        {
            notificationsBadgeObject.SetActive(true);
            notificationsBadgeText.text = newNotifications.ToString();
        }
        else
        {
            notificationsBadgeObject.SetActive(false);
        }
    }
}
