using System;
using UnityEngine;

public class ClearButton : MonoBehaviour
{
    public void UpdateClearButtonStatus(int value)
    {
        gameObject.SetActive(value != 0);
    }

    public void UpdateClearButtonStatus(string value)
    {
        gameObject.SetActive(!String.IsNullOrEmpty(value));
    }
}
