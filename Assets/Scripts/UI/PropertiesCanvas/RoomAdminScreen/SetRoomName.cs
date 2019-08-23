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

    /// <summary>
    /// set current room name
    /// </summary>
    /// <param name="value">current name value</param>
    public void SetCurrentName(string value)
    {
        roomNameCache = value;
        roomNameInputField.text = roomNameCache;
    }

    /// <summary>
    /// get current room name
    /// </summary>
    /// <returns>return name</returns>
    public string GetCurrentName()
    {
        return roomNameCache;
    }

    /// <summary>
    /// chack if the room name is changed and if the save can be pressed
    /// </summary>
    /// <param name="value">room name string</param>
    public void OnRoomNameValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            roomAdminScreen.CanSave = false;
            roomNameCache = string.Empty;
        }
        else
        {
            roomAdminScreen.CanSave = true;
            roomNameCache = value;
        }
        
    }

    /// <summary>
    /// deprecated function because the price is not a feature for the app
    /// check room price
    /// </summary>
    /// <param name="value"></param>
    public void OnRoomPriceValueChanged(string value)
    {
        roomPriceCache = string.IsNullOrEmpty(value) ? "Free" : value;
    }
}
