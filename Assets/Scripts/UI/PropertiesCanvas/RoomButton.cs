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
    [SerializeField]
    private Text floorNumber = null;
    [SerializeField]
    private RectTransform transformComponent = null;
    [SerializeField]
    private GameObject floorNumberObject = null;
    [SerializeField]
    private GameObject roomButtonObject = null;


    [Header("Theme objects"), SerializeField]
    private Graphic roomNameText = null;
    [SerializeField]
    private Graphic roomBackground = null;
    [SerializeField]
    private Graphic floorNumberText = null;
    [SerializeField]
    private Shadow shadowComponent = null;

    private IRoom currentRoom;

    private void Start()
    {
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => SetText());
    }

    private void OnDisable()
    {
        roomButton.onClick.RemoveAllListeners();
    }

    public void InitializeTheme(ThemeManager themeManager)
    {
        themeManager.AddItems(roomNameText, roomBackground, floorNumberText);
        themeManager.AddShadow(shadowComponent);
    }

    /// <summary>
    /// set current room name
    /// set room callback
    /// </summary>
    /// <param name="room">current room</param>
    /// <param name="roomCallback">callback to open room screen</param>
    public void InitializeRoom(IRoom room, Action<IRoom> roomCallback)
    {
        floorNumberObject.SetActive(false);
        roomButtonObject.SetActive(true);
        transformComponent.sizeDelta = new Vector2(transformComponent.sizeDelta.x, 110);
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.NEW_ROOM : $"{LocalizedText.Instance.RoomItem} {room.Name}";
        currentRoom = room;
        roomButton.onClick.AddListener(() => roomCallback(room));
    }

    public void InitializeFloor(string floor)
    {
        floorNumberObject.SetActive(true);
        roomButtonObject.SetActive(false);
        transformComponent.sizeDelta = new Vector2(transformComponent.sizeDelta.x, 70);
        floorNumber.text = floor;
    }

    private void SetText()
    {
        roomName.text = $"{LocalizedText.Instance.RoomItem}{currentRoom.Name}";
    }
}