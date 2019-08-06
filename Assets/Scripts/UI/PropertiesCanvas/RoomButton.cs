using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    [SerializeField]
    private Text roomName = null;
    [SerializeField]
    private Button roomButton = null;

    [Header("Theme objects"), SerializeField]
    private Graphic roomNameText = null;
    [SerializeField]
    private Graphic roomBackground = null;

    private IRoom currentRoom;

    /// <summary>
    /// set current room name
    /// set room callback
    /// </summary>
    /// <param name="room">current room</param>
    /// <param name="roomCallback">callback to open room screen</param>
    public void Initialize(IRoom room, Action<IRoom> roomCallback, ThemeManager themeManager)
    {
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.NEW_ROOM : room.Name;
        currentRoom = room;
        roomButton.onClick.AddListener(() => roomCallback(room));

        themeManager.SetColor(roomNameText);
        themeManager.SetColor(roomBackground);
    }
}