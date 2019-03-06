using System;
using System.IO;
using UnityEngine;

public class BackupManager : MonoBehaviour
{
    [SerializeField]
    private ToastController toastController = null;

    private const string DIRECTORY_NAME = "data";

    private string PropertyDataFilePath
    {
        get => Path.Combine(Application.persistentDataPath, PropertyDataManager.DATA_FILE_NAME);
    }

    private string ReservationDataFilePath
    {
        get => Path.Combine(Application.persistentDataPath, ReservationDataManager.DATA_FILE_NAME);
    }

    private FileInfo GetNewFileInfo(DateTime dateTime)
    {
        string dateTimeString = dateTime.ToString("o").Replace(':', '_');
        string name = $"data_{dateTimeString}.json";
        string path = Path.Combine(Application.persistentDataPath, DIRECTORY_NAME, name);

        return new FileInfo(path);
    }

    private BackupData ReadBackupData(FileInfo file)
    {
        string dataAsJson = File.ReadAllText(file.FullName);

        try
        {
            return JsonUtility.FromJson<BackupData>(dataAsJson);
        }
        catch (System.ArgumentException)
        {
            toastController.Show("Eroare: Nu s-a putut citi fișierul " + file.FullName);
            Debug.LogError("Failed to read BackupData at " + file.FullName);
            return null;
        }
    }

    public void BackUp()
    {
        BackupData data = new BackupData();

        if (File.Exists(PropertyDataFilePath))
        {
            data.propertyData = File.ReadAllText(PropertyDataFilePath);
        }
        else
        {
            toastController.Show("Exportare eșuată. Fisierul de date nu a fost găsit.");
            return;
        }

        if (File.Exists(ReservationDataFilePath))
        {
            data.reservationData = File.ReadAllText(ReservationDataFilePath);
        }
        else
        {
            toastController.Show("Exportare eșuată. Fisierul de date nu a fost găsit.");
            return;
        }

        data.CreationDate = DateTime.Now;

        string dataAsJson = JsonUtility.ToJson(data);

        FileInfo file = GetNewFileInfo(data.CreationDate);
        file.Directory.Create();
        File.WriteAllText(file.FullName, dataAsJson);

        toastController.Show($"Fisierul {file.Name} a fost creat.");
    }

    public BackupData[] GetBackups()
    {
        string path = Path.Combine(Application.persistentDataPath, DIRECTORY_NAME);
        DirectoryInfo directory = new DirectoryInfo(path);

        FileInfo[] files = directory.GetFiles();

        return Array.ConvertAll(files, file => ReadBackupData(file));
    }

    public void Restore(BackupData data)
    {
        // backup the current data files
        File.Copy(PropertyDataFilePath, Path.ChangeExtension(PropertyDataFilePath, ".backup"), true);
        File.Copy(ReservationDataFilePath, Path.ChangeExtension(ReservationDataFilePath, ".backup"), true);

        // replace
        File.WriteAllText(PropertyDataFilePath, data.propertyData);
        File.WriteAllText(ReservationDataFilePath, data.reservationData);
    }
}

[Serializable]
public class BackupData
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
