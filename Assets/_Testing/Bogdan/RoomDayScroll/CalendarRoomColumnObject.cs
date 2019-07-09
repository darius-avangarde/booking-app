using UnityEngine;
using UnityEngine.Events;

public class CalendarRoomColumnObject : MonoBehaviour
{
    public void UpdateRoomObject(IRoom room, UnityAction<IRoom> tapAction)
    {
        if(room == null)
        {
            gameObject.SetActive(false);
        }
    }
}
