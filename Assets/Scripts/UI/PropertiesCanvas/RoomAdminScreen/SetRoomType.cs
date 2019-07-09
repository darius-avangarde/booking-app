using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetRoomType : MonoBehaviour
{
    [SerializeField]
    private RoomAdminScreen roomAdminScreen = null;
    [SerializeField]
    private Dropdown roomTypeDropdown = null;

    private string currentRoomType;

    private void Awake()
    {
        roomAdminScreen.GetRoomType += GetRoomTypeDropdown;
        roomAdminScreen.SetRoomType += SetRoomTypeDropdown;
    }

    private void GetRoomTypeDropdown()
    {
        roomAdminScreen.CurrentRoom.Type = currentRoomType;
    }

    private void SetRoomTypeDropdown(string type)
    {
        int count = 0;
        roomTypeDropdown.value = 0;
        foreach (var option in roomTypeDropdown.options)
        {
            if (!string.IsNullOrEmpty(type) && option.text == type)
            {
                roomTypeDropdown.value = count;
                break;
            }
            count++;
        }
    }

    public void SelectRoomType(int value)
    {
        currentRoomType = roomTypeDropdown.options[value].text;
    }
}
