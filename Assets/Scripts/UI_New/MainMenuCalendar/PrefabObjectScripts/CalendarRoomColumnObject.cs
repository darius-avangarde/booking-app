using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarRoomColumnObject : MonoBehaviour
{
    public RectTransform RoomRectTransform => roomRectTransform;

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

        UpdateRoomObjectUI(room);

        roomButton.onClick.RemoveAllListeners();
        roomButton.onClick.AddListener(() => tapAction(room));
    }

    public void UpdateRoomObjectUI(IRoom room)
    {
        gameObject.SetActive(true);

        roomName.text = room.Name;
        singleBeds.text = $"{room.SingleBeds}";
        doubleBeds.text = $"{room.DoubleBeds}";
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
