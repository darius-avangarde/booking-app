using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomToggleObject : MonoBehaviour
{
    public bool IsOn
    {
        get => objectToggle.isOn;
        set => objectToggle.isOn = value;
    }
    public IRoom ObjectRoom => currentRoom;

    [SerializeField]
    private Toggle objectToggle;
    [SerializeField]
    private Text roomNameText;
    [SerializeField]
    private Image roomTypeIcon;

    [Space]
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Image underline;
    [SerializeField]
    private Image checkmarkRim;
    [SerializeField]
    private Image checkmarkFill;

    private IRoom currentRoom;
    private UnityAction<IRoom, RoomToggleObject> onTapCallback;
    private UnityAction<RoomToggleObject> onToggleCallback;

    private void Start()
    {
        //ThemeManager.Instance.AddItems(roomNameText, /*roomTypeIcon */ backgroundImage, underline, checkmarkFill, checkmarkRim);
    }

    public void UpdateObject(UnityAction<IRoom, RoomToggleObject> tapCallback, UnityAction<RoomToggleObject> toggleCallback, IRoom room, bool selected)
    {
        gameObject.SetActive(true);
        currentRoom = room;
        objectToggle.isOn = selected;
        roomNameText.text = room.Name;
        //add room sprite by image
        onTapCallback = tapCallback;
        onToggleCallback = toggleCallback;
    }

    public void ObjectButtonAction()
    {
        onTapCallback?.Invoke(currentRoom, this);
    }

    public void ObjectTogglAction()
    {
        onToggleCallback?.Invoke(this);
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
