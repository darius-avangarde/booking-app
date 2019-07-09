using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarRoomColumnObject : MonoBehaviour
{
    [SerializeField]
    private Button roomButton;

    [SerializeField]
    private Text roomName;
    [SerializeField]
    private Text singleBeds;
    [SerializeField]
    private Text doubleBeds;



    private void OnDestroy()
    {
        roomButton.onClick.RemoveAllListeners();
    }

    public void UpdateRoomObject(IRoom room, UnityAction<IRoom> tapAction)
    {
        roomName.text = room.Name;
        singleBeds.text = $"{room.SingleBeds}";
        doubleBeds.text = $"{room.DoubleBeds}";

        roomButton.onClick.AddListener(() => tapAction(room));
    }
}
