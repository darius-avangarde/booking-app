using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetRoomName : MonoBehaviour
{
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private InputField roomPriceInputField = null;

    private string roomNameCache;
    private string roomPriceCache;

    private void OnEnable()
    {
        roomNameCache = string.Empty;
        roomPriceCache = string.Empty;
        roomNameInputField.text = roomNameCache;
        roomPriceInputField.text = roomPriceCache;
    }

    public void SetCurrentName(string value)
    {
        roomNameCache = value;
        roomNameInputField.text = roomNameCache;
    }

    public string GetCurrentName()
    {
        return roomNameCache;
    }

    public void OnRoomNameValueChanged(string value)
    {
        roomNameCache = string.IsNullOrEmpty(value) ? Constants.NEW_ROOM : value;
    }

    public void OnRoomPriceValueChanged(string value)
    {
        roomPriceCache = string.IsNullOrEmpty(value) ? "Free" : value;
    }
}
