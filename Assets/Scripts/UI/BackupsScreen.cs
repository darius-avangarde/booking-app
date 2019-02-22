using System;
using System.Collections.Generic;
using System.IO;
using UINavigation;
using UnityEngine;

public class BackupsScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;

    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;

    [SerializeField]
    private BackupManager backupManager = null;

    [SerializeField]
    private Transform container = null;

    [SerializeField]
    private GameObject backupButtonPrefab = null;

    private List<GameObject> backupButtons = new List<GameObject>();

    public void Initialize()
    {
        Debug.Log("[DEBUG] BackupScreen.Initialize ");
        CleanUp();

        BackupData[] backups = backupManager.GetBackups();

        foreach (var backup in backups)
        {
            CreateBackupButton(backup);
        }
    }

    public void CleanUp()
    {
        Debug.Log("[DEBUG] BackupScreen.CleanUp ");
        foreach (var backupButton in backupButtons)
        {
            Destroy(backupButton);
        }
    }

    private GameObject CreateBackupButton(BackupData data)
    {
        GameObject button = Instantiate(backupButtonPrefab, container);
        string buttonText = data.CreationDate.ToString("yyyy-MM-dd HH:mm:ss");

        ConfirmationDialogOptions options = new ConfirmationDialogOptions();
        options.Message = "Urmează să înlocuiți toate datele din aplicație cu datele din fișierul ales.\nSunteți sigur?";
        options.ConfirmCallback = () =>
        {
            backupManager.Restore(data);
            navigator.GoBack();
        };

        Action buttonCallback = () => { confirmationDialog.Show(options); };

        button.GetComponent<BackupButton>().Initialize(buttonText, buttonCallback);

        return button;
    }

}
