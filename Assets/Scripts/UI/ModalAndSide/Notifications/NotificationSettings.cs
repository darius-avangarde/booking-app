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

    private int selectedOption = 0;
    private int previousSelectedOption = 0;

    private void Start()
    {
        sendNotitifationsToggle.onValueChanged.AddListener((t) => SetNotificationToggle(t));
        settingsManager.ReadData();
        sendNotitifationsToggle.isOn = settingsManager.DataElements.settings.ReceiveNotifications;
        SetDropdownOptions();
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => SetDropdownOptions());
        SetDropdownInteractable(sendNotitifationsToggle.isOn);
    }

    /// <summary>
    /// set pre alert dropdown interactable
    /// </summary>
    /// <param name="value">interactable value</param>
    public void SetDropdownInteractable(bool value)
    {
        preAlertDropdown.interactable = value;
    }

    /// <summary>
    /// set dropdown value
    /// </summary>
    /// <param name="value">dropdown value</param>
    public void SelectDropdownValue(int value)
    {
        previousSelectedOption = selectedOption;
        selectedOption = value;
        settingsManager.DataElements.settings.PreAlertTime = selectedOption;
        notificatoinManager.UpdateDefaultNotifications(previousSelectedOption, LocalizedText.Instance.PreAlertDictFunction.ElementAt(selectedOption).Key);
        settingsManager.WriteData();
    }

    /// <summary>
    /// set toggle for enabling notifications
    /// </summary>
    /// <param name="value">toggle value</param>
    private void SetNotificationToggle(bool value)
    {
        settingsManager.DataElements.settings.ReceiveNotifications = value;
        settingsManager.WriteData();
    }

    /// <summary>
    /// update pre alert dropdown options list
    /// </summary>
    private void SetDropdownOptions()
    {
        List<Dropdown.OptionData> dropdownOptionsList = new List<Dropdown.OptionData>();
        for (int i = 0; i < LocalizedText.Instance.PreAlertDictFunction.Count; i++)
        {
            Dropdown.OptionData newOption = new Dropdown.OptionData();
            newOption.text = LocalizedText.Instance.PreAlertDictFunction.ElementAt(i).Value;
        dropdownOptionsList.Add(newOption);
        }
        preAlertDropdown.options = dropdownOptionsList;
        preAlertDropdown.value = settingsManager.DataElements.settings.PreAlertTime;
    }
}
