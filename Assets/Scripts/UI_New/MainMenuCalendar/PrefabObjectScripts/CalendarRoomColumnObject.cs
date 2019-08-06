using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarRoomColumnObject : MonoBehaviour
{
    public RectTransform RoomRectTransform => roomRectTransform;
    public int RoomIndex => currentRoomIndex;
    public IRoom Room => currentRoom;
    public List<CalendarDayColumnObject> LinkedDayRow => linkedDayRow;

    [SerializeField]
    private RectTransform roomRectTransform;

    [SerializeField]
    private Button roomButton;

    [SerializeField]
    private Text roomName;
    [SerializeField]
    private Text singleBeds;
    [SerializeField]
    private Text doubleBeds;
    [SerializeField]
    private Image singleBedsImage;
    [SerializeField]
    private Image doubleBedsImage;

    private int currentRoomIndex = 0;
    private IRoom currentRoom;
    private List<CalendarDayColumnObject> linkedDayRow = new List<CalendarDayColumnObject>();

    private void Start()
    {
        ThemeManager.Instance.AddItems(roomName, roomButton.targetGraphic, singleBeds, doubleBeds, singleBedsImage, doubleBedsImage);
    }

    private void OnDestroy()
    {
        roomButton.onClick.RemoveAllListeners();
    }

    public void AddLinkedDayObject(CalendarDayColumnObject dayObject)
    {
        linkedDayRow.Add(dayObject);
    }

    public void SetObjectAction(UnityAction<IRoom> tapAction)
    {
        roomButton.onClick.RemoveAllListeners();
        roomButton.onClick.AddListener(() => tapAction(currentRoom));
    }

    public void UpdateObjectRoom(IRoom room, int roomIndex)
    {
        currentRoomIndex = roomIndex;
        currentRoom = room;
        gameObject.SetActive(true);

        if(currentRoom !=null)
        {
            roomName.text = room.Name;
            singleBeds.text = $"{room.SingleBeds}";
            doubleBeds.text = $"{room.DoubleBeds}";
        }

        for (int i = 0; i < linkedDayRow.Count; i++)
        {
            linkedDayRow[i].SetPosition();
        }
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
