using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarRoomColumnObject : MonoBehaviour
{
    public RectTransform RoomRectTransform => roomRectTransform;
    public int RoomIndex => currentRoomIndex;

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

    private void Start()
    {
        ThemeManager.Instance.AddItems(roomName, roomButton.targetGraphic, singleBeds, doubleBeds, singleBedsImage, doubleBedsImage);
    }

    private void OnDestroy()
    {
        roomButton.onClick.RemoveAllListeners();
    }

    public void UpdateRoomObject(IRoom room, UnityAction<IRoom> tapAction)
    {
        gameObject.SetActive(true);

        UpdateRoomObjectUI(room, 0);

        roomButton.onClick.RemoveAllListeners();
        roomButton.onClick.AddListener(() => tapAction(room));
    }

    public void UpdateRoomObjectUI(IRoom room, int roomIndex)
    {
        currentRoomIndex = roomIndex;
        gameObject.SetActive(true);

        if(room !=null)
        {
            roomName.text = room.Name;
            singleBeds.text = $"{room.SingleBeds}";
            doubleBeds.text = $"{room.DoubleBeds}";
        }
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
