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
    private NotificationManager notificatoinManager = null;
    //[SerializeField]
    //private LocalizedText LocalizedText = null;
    [SerializeField]
    private Toggle sendNotitifationsToggle = null;
    [SerializeField]
    private Dropdown preAlertDropdown = null;

    private string[] dropdownOptions = { "1 ora", "2 ore", "4 ore", "1 zi", "2 zile", "3 zile", "4 zile", "5 zile", "6 zile", "7 zile" };
    private int selecteOption = 0;

    private void Start()
    {
        sendNotitifationsToggle.onValueChanged.AddListener((t) => SetNotificationToggle(t));
        settingsManager.ReadData();
        sendNotitifationsToggle.isOn = settingsManager.DataElements.settings.ReceiveNotifications;
        SetDropdownOptions();
        SetDropdownInteractable(sendNotitifationsToggle.isOn);
    }

    public void SetDropdownInteractable(bool value)
    {
        preAlertDropdown.interactable = value;
    }

    public void SelectDropdownValue(int value)
    {
        selecteOption = value;
        settingsManager.DataElements.settings.PreAlertTime = value;
        notificatoinManager.UpdateAllNotifications(Constants.PreAlertDict.ElementAt(value).Key);
        settingsManager.WriteData();
    }

    private void SetNotificationToggle(bool value)
    {
        settingsManager.DataElements.settings.ReceiveNotifications = value;
        settingsManager.WriteData();
    }

    private void SetDropdownOptions()
    {
        List<Dropdown.OptionData> dropdownOptionsList = new List<Dropdown.OptionData>();
        for (int i = 0; i < Constants.PreAlertDict.Count; i++)
        {
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = Constants.PreAlertDict.ElementAt(i).Value;
            dropdownOptionsList.Add(newOption);
        }
        preAlertDropdown.options = dropdownOptionsList;
        preAlertDropdown.value = settingsManager.DataElements.settings.PreAlertTime;
    }
}
