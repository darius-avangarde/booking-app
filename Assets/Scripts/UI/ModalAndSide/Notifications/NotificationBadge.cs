using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBadge : MonoBehaviour
{
    [SerializeField]
    private GameObject notificationsBadgeObject = null;
    [SerializeField]
    private Text notificationsBadgeText = null;

    private int badgeCount = 0;

    private void OnEnable()
    {
        if (badgeCount > 0)
        {
            notificationsBadgeObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        notificationsBadgeObject.SetActive(false);
    }

    public void SetNotificationBadge(int newNotifications)
    {
        if (newNotifications > 0)
        {
            badgeCount = newNotifications;
            notificationsBadgeText.text = badgeCount.ToString();
        }
        else
        {
            badgeCount = 0;
        }
    }
}
