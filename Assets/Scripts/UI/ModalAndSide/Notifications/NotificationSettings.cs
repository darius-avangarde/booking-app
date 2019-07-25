using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationSettings : MonoBehaviour
{
    [SerializeField]
    private SettingsManager settingsManager = null;
    [SerializeField]
    private LocalizedText LocalizedText = null;
    [SerializeField]
    private Toggle sendNotitifationsToggle = null;
    [SerializeField]
    private Dropdown preAlertDropdown = null;

    private string[] dropdownOptions = {"1 ora", "2 ore", "4 ore", "1 zi", "2 zile", "3 zile", "4 zile", "5 zile", "6 zile", "7 zile" };

    private void Start()
    {
        sendNotitifationsToggle.onValueChanged.AddListener((t) => SetNotificationToggle(t));
        settingsManager.ReadData();
        sendNotitifationsToggle.isOn = settingsManager.DataElements.settings.ReceiveNotifications;
        SetDropdownOptions();
    }

    private void SetNotificationToggle(bool value)
    {
        settingsManager.DataElements.settings.ReceiveNotifications = value;
    }

    private void SetDropdownOptions()
    {
        foreach (string notificationAlert in dropdownOptions)
        {
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = notificationAlert;
        }
    }

    public int GetDefaultPreAlert()
    {
        return Constants.PreAlertDict.ElementAt(preAlertDropdown.value).Key;
    }
}
