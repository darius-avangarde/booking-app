using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public const string DATA_FILE_NAME = "settingsData.json";
    public  SettingsData DataElements { get; set; }

    private void Start()
    {
        ReadData();
    }
    public SettingsData ReadData()
    {

        if (DataElements == null)
        {
            string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                DataElements = JsonUtility.FromJson<SettingsData>(dataAsJson);
            }
            else
            {
                DataElements = new SettingsData();
                Debug.Log("Unable to read default input file");
                DataElements.settings = new SettingsItem();
                DataElements.settings.themeStatus = 0;
                DataElements.settings.language = "Ro";
                DataElements.settings.ReceiveNotifications = true;
                DataElements.settings.PreAlertTime = 24;
                WriteData();

            }
        }
        return DataElements;
    }
    public   void WriteData()
    {
        string dataAsJson = JsonUtility.ToJson(DataElements, true);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }
}
