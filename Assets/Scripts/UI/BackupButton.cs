using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BackupButton : MonoBehaviour
{
    [SerializeField]
    private Button button = null;

    [SerializeField]
    private Text buttonText = null;

    public void Initialize(string text, Action callback)
    {
        buttonText.text = text;
        button.onClick.AddListener(() => callback());
    }
}
