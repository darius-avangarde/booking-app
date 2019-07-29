
[System.Serializable]
public class SettingsData 
{
    public SettingsItem settings;
}
[System.Serializable]
public class SettingsItem
{
    public int themeStatus;
    public string language;
    public int PreAlertTime;
    public bool ReceiveNotifications;
}
