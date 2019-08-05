using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CalendarRoomColumn : MonoBehaviour
{
    [SerializeField]
    private GameObject roomColumnObjectPrefab;
    [SerializeField]
    private ScrollRect roomColumnScrolrect;

    [Space]
    [SerializeField]
    private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField]
    private ContentSizeFitter cfg;

    [Space]
    [SerializeField]
    private List<CalendarRoomColumnObject> roomPool = new List<CalendarRoomColumnObject>();
    private List<IRoom> currentRooms;

    private Vector2 _newAnchoredPosition = Vector2.zero;
    private float _recordOffsetY = 0;
    private float _disableMarginY = 0;
    private float _treshold = 100f;

    private void Start()
    {
        //populate pool
        for (int i = 0; i < roomColumnScrolrect.content.childCount; i++)
        {
            roomPool.Add(roomColumnScrolrect.content.GetChild(i).GetComponent<CalendarRoomColumnObject>());
        }

        _recordOffsetY = roomPool[0].RoomRectTransform.anchoredPosition.y - roomPool[1].RoomRectTransform.anchoredPosition.y;
        _disableMarginY = _recordOffsetY * roomPool.Count / 2;

        roomColumnScrolrect.onValueChanged.AddListener(OnScroll);
        verticalLayoutGroup.enabled = false;
        cfg.enabled = false;
    }

    private void OnScroll(Vector2 pos)
    {
        for (int i = 0; i < roomPool.Count; i++)
        {
            if (roomColumnScrolrect.transform.InverseTransformPoint(roomPool[i].gameObject.transform.position).y > _disableMarginY + _treshold)
            {
                _newAnchoredPosition = roomPool[i].RoomRectTransform.anchoredPosition;
                _newAnchoredPosition.y -= roomPool.Count * _recordOffsetY;
                roomPool[i].RoomRectTransform.anchoredPosition = _newAnchoredPosition;
                Transform item = roomColumnScrolrect.content.GetChild(roomPool.Count - 1).transform;
                item.SetAsFirstSibling();
            }
            else if (roomColumnScrolrect.transform.InverseTransformPoint(roomPool[i].gameObject.transform.position).y < -_disableMarginY)
            {
                _newAnchoredPosition = roomPool[i].RoomRectTransform.anchoredPosition;
                _newAnchoredPosition.y += roomPool.Count * _recordOffsetY;
                roomPool[i].RoomRectTransform.anchoredPosition = _newAnchoredPosition;
                Transform item = roomColumnScrolrect.content.GetChild(0).transform;
                item.SetAsLastSibling();
            }
        }
    }

    public void UpdateRooms(List<IRoom> rooms, UnityAction<IRoom> tapAction)
    {
        currentRooms = (rooms != null) ? rooms : new List<IRoom>();
        roomColumnScrolrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical ,rooms.Count * _recordOffsetY);

        for (int i = 0; i < roomPool.Count; i++)
        {
            if(currentRooms.Count > i)
            {
                roomPool[i].UpdateRoomObjectUI(currentRooms[i]);
            }
            else
            {
                roomPool[i].DisableObject();
            }
            roomPool[i].RoomRectTransform.SetSiblingIndex(i);
        }

    //     ManagePool(rooms);

    //     for (int r = 0; r < rooms.Count; r++)
    //     {
    //         roomPool[r].UpdateRoomObject(rooms[r], tapAction);
    //     }
    }

    // public void UpdateRoomsUI(List<IRoom> rooms)
    // {
    //     for (int r = 0; r < rooms.Count; r++)
    //     {
    //         roomPool[r].UpdateRoomObjectUI(rooms[r]);
    //     }
    // }

    // private void ManagePool(List<IRoom> rooms)
    // {
    //     if(rooms.Count != roomPool.Count)
    //     {
    //         //CreateNewObjects as needed
    //         for (int i = roomPool.Count; i < rooms.Count; i++)
    //         {
    //             CreateRoomColumnObject();
    //         }

    //         //Disable unused objects
    //         for (int i = roomPool.Count - 1; i > rooms.Count - 1; i--)
    //         {
    //             roomPool[i].gameObject.SetActive(false);
    //         }
    //     }
    // }

    // private void CreateRoomColumnObject()
    // {
    //     roomPool.Add(Instantiate(roomColumnObjectPrefab, transform).GetComponent<CalendarRoomColumnObject>());
    // }
}
