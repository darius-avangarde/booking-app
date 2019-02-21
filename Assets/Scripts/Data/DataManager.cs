using System;
using System.IO;
using UnityEngine;

public static class DataManager
{
    private const string DIRECTORY_NAME = "data";

    private static string PropertyDataFilePath
    {
        get => Path.Combine(Application.persistentDataPath, PropertyDataManager.DATA_FILE_NAME);
    }

    private static string ReservationDataFilePath
    {
        get => Path.Combine(Application.persistentDataPath, ReservationDataManager.DATA_FILE_NAME);
    }

    private static FileInfo GetNewFileInfo(DateTime dateTime)
    {
        string dateTimeString = dateTime.ToString("o").Replace(':', '_');
        string name = $"data_{dateTimeString}.json";
        string path = Path.Combine(Application.persistentDataPath, DIRECTORY_NAME, name);

        return new FileInfo(path);
    }

    public static void BackUp()
    {
        Data data = new Data();

        if (File.Exists(PropertyDataFilePath))
        {
            data.propertyData = File.ReadAllText(PropertyDataFilePath);
        }
        else
        {
            // TODO: implement warning toast or some type of user notification
            Debug.LogWarning("[DEBUG] Could not find file at " + PropertyDataFilePath);
            return;
        }

        if (File.Exists(ReservationDataFilePath))
        {
            data.reservationData = File.ReadAllText(ReservationDataFilePath);
        }
        else
        {
            // TODO: implement warning toast or some type of user notification
            Debug.LogWarning("[DEBUG] Could not find file at " + ReservationDataFilePath);
            return;
        }

        data.CreationDate = DateTime.Now;

        string dataAsJson = JsonUtility.ToJson(data);

        FileInfo file = GetNewFileInfo(data.CreationDate);
        file.Directory.Create();
        File.WriteAllText(file.FullName, dataAsJson);
    }

    public static void Restore(FileInfo file)
    {
        string dataAsJson = File.ReadAllText(file.FullName);

        Data data = JsonUtility.FromJson<Data>(dataAsJson);

        File.Copy(PropertyDataFilePath, Path.ChangeExtension(PropertyDataFilePath, ".backup"), true);
        File.Copy(ReservationDataFilePath, Path.ChangeExtension(ReservationDataFilePath, ".backup"), true);

        File.WriteAllText(PropertyDataFilePath, data.propertyData);
        File.WriteAllText(ReservationDataFilePath, data.reservationData);
    }

    public static FileInfo[] GetFiles()
    {
        string path = Path.Combine(Application.persistentDataPath, DIRECTORY_NAME);
        DirectoryInfo directory = new DirectoryInfo(path);

        return directory.GetFiles();
    }

    [Serializable]
    private class Data
    {
        public long ticks;
        public DateTime CreationDate
        {
            get => new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
            set => ticks = value.ToUniversalTime().Ticks;
        }

        public string propertyData;
        public string reservationData;
    }
}
