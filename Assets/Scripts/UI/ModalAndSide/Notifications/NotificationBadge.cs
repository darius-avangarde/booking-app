using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationBadge : MonoBehaviour
{
    [SerializeField]
    private Image notificationsBadgeIcon = null;
    [SerializeField]
    private Text notificationsBadgeNumber = null;

    private int badgeCount = 0;

    private void OnEnable()
    {
        if (badgeCount > 0)
        {
            notificationsBadgeIcon.color = new Color(notificationsBadgeIcon.color.r, notificationsBadgeIcon.color.g, notificationsBadgeIcon.color.b, 255);
            notificationsBadgeNumber.color = new Color(notificationsBadgeNumber.color.r, notificationsBadgeNumber.color.g, notificationsBadgeNumber.color.b, 255);
        }
        else
        {
            notificationsBadgeIcon.color = new Color(notificationsBadgeIcon.color.r, notificationsBadgeIcon.color.g, notificationsBadgeIcon.color.b, 0);
            notificationsBadgeNumber.color = new Color(notificationsBadgeNumber.color.r, notificationsBadgeNumber.color.g, notificationsBadgeNumber.color.b, 0);
        }
    }

    private void OnDisable()
    {
        notificationsBadgeIcon.color = new Color(notificationsBadgeIcon.color.r, notificationsBadgeIcon.color.g, notificationsBadgeIcon.color.b, 0);
        notificationsBadgeNumber.color = new Color(notificationsBadgeNumber.color.r, notificationsBadgeNumber.color.g, notificationsBadgeNumber.color.b, 0);
    }

    public IEnumerator SetNotificationBadge(int newNotifications)
    {
        if (newNotifications > 0)
        {
            badgeCount = newNotifications;
            notificationsBadgeNumber.text = badgeCount.ToString();
        }
        else
        {
            badgeCount = 0;
        }
        yield return new WaitForEndOfFrame();
        if (badgeCount > 0)
        {
            notificationsBadgeIcon.color = new Color(notificationsBadgeIcon.color.r, notificationsBadgeIcon.color.g, notificationsBadgeIcon.color.b, 255);
            notificationsBadgeNumber.color = new Color(notificationsBadgeNumber.color.r, notificationsBadgeNumber.color.g, notificationsBadgeNumber.color.b, 255);
        }
        else
        {
            notificationsBadgeIcon.color = new Color(notificationsBadgeIcon.color.r, notificationsBadgeIcon.color.g, notificationsBadgeIcon.color.b, 0);
            notificationsBadgeNumber.color = new Color(notificationsBadgeNumber.color.r, notificationsBadgeNumber.color.g, notificationsBadgeNumber.color.b, 0);
        }
    }
}
