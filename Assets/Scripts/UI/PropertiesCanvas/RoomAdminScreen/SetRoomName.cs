using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetRoomName : MonoBehaviour
{
    [SerializeField]
    private RoomAdminScreen roomAdminScreen = null;
    [SerializeField]
    private InputField roomNameInputField = null;
    [SerializeField]
    private InputField roomPriceInputField = null;

    private string roomNameCache;
    private string roomPriceCache;

    private void OnEnable()
    {
        roomNameInputField.text = roomNameCache;
        roomPriceInputField.text = roomPriceCache;
    }

    private void OnDisable()
    {
        roomNameCache = string.Empty;
        roomPriceCache = string.Empty;
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
        if (string.IsNullOrEmpty(value))
        {
            roomAdminScreen.CanSave = false;
            roomNameCache = Constants.NEW_ROOM;
        }
        else
        {
            roomAdminScreen.CanSave = true;
            roomNameCache = value;
        }
        
    }

    public void OnRoomPriceValueChanged(string value)
    {
        roomPriceCache = string.IsNullOrEmpty(value) ? "Free" : value;
    }
}
