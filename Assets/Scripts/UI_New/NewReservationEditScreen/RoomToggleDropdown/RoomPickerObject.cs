using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomPickerObject : MonoBehaviour
{
    [SerializeField]
    private RectTransform objRect;
    [SerializeField]
    private Text roomNameText;
    [SerializeField]
    private Toggle objectToggle;


    public bool IsOn
    {
        get => objectToggle.isOn;
        set => objectToggle.isOn = value;
    }
    public RectTransform ObjRect => objRect;
    public IRoom ObjRoom => currentRoom;

    private UnityAction<IRoom,RoomPickerObject> tapAction;
    private UnityAction<bool, IRoom> toggleAction;

    private UnityAction<IRoom,RoomPickerObject> currentTapAction;
    private UnityAction<bool, IRoom> currentToggleAction;

    private IRoom currentRoom;

    public void SetObjectActions(UnityAction<IRoom,RoomPickerObject> onTap, UnityAction<bool, IRoom> onToggle)
    {
        tapAction = currentTapAction = onTap;
        toggleAction = currentToggleAction = onToggle;
    }

    public void SetRoom(IRoom room, bool isSelected, bool isOccupied = false)
    {
        currentTapAction = null;
        currentToggleAction = null;

        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
        roomNameText.text = room.Name;
        currentRoom = room;
        IsOn = isSelected;

        SetColors(isOccupied ? Color.red : (ThemeManager.Instance.IsDarkTheme ? ThemeManager.Instance.ThemeColor.textDark : ThemeManager.Instance.ThemeColor.textLight));

        currentTapAction = tapAction;
        currentToggleAction = toggleAction;
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void TapAction()
    {
        currentTapAction?.Invoke(ObjRoom, this);
    }

    public void ToggleAction()
    {
        currentToggleAction?.Invoke(IsOn, currentRoom);
    }

    private void SetColors(Color color)
    {
        roomNameText.color = color;
        objectToggle.targetGraphic.color = color;
    }
}
