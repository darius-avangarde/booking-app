using System.IO;
using UnityEditor;
using UnityEngine;

public static class EditorUtils
{
    [MenuItem("Data Files/Delete Reservation Data")]
    public static void DeleteReservationDataFiles()
    {
        string path = Path.Combine(Application.persistentDataPath, ReservationDataManager.DATA_FILE_NAME);

        if (File.Exists(path))
        {
            string title = "Delete reservation data?";
            string message = path + "\nAre you sure you want to delete this file?";

            if (EditorUtility.DisplayDialog(title, message, "Delete", "Cancel"))
            {
                File.Delete(path);
            }
        }
        else
        {
            Debug.LogWarning("There is no file at " + path);
        }
    }

    [MenuItem("Data Files/Delete Property Data")]
    public static void ClearPropertiesDataFiles()
    {
        string path = Path.Combine(Application.persistentDataPath, PropertyDataManager.DATA_FILE_NAME);

        if (File.Exists(path))
        {
            string title = "Delete property data?";
            string message = path + "\nAre you sure you want to delete this file?";

            if (EditorUtility.DisplayDialog(title, message, "Delete", "Cancel"))
            {
                File.Delete(path);
            }
        }
        else
        {
            Debug.LogWarning("There is no file at " + path);
        }
    }

    [MenuItem("Data Files/Update Deleted Reservations")]
    public static void UpdateDeletedReservations()
    {
        int updatedCount = 0;

        foreach (var reservation in ReservationDataManager.GetReservations())
        {
            IProperty property = PropertyDataManager.GetProperty(reservation.PropertyID);

            if (property == null)
            {
                ReservationDataManager.DeleteReservation(reservation.ID);
                updatedCount++;
                continue;
            }

            IRoom room = property.GetRoom(reservation.RoomID);
            if (room == null)
            {
                ReservationDataManager.DeleteReservation(reservation.ID);
                updatedCount++;
            }
        }

        Debug.Log("Done. Deleted " + updatedCount + " reservations with deleted rooms.");
    }
}
