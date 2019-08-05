using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

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

        verticalLayoutGroup.enabled = false;
        cfg.enabled = false;
        roomColumnScrolrect.onValueChanged.AddListener(OnScroll);
    }

    public void UpdateRooms(List<IRoom> rooms, UnityAction<IRoom> tapAction)
    {
        currentRooms = (rooms != null) ? rooms : new List<IRoom>();
        roomColumnScrolrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical ,rooms.Count * _recordOffsetY);

        for (int i = 0; i < roomPool.Count - 1; i++)
        {
            if(currentRooms.Count > i)
            {
                roomPool[i].UpdateRoomObjectUI(currentRooms[i], i);
            }
            else
            {
                roomPool[i].DisableObject();
            }

            roomPool[roomPool.Count -1].UpdateRoomObjectUI(null, -1);

            //reposition and reorder items
            roomPool[i].RoomRectTransform.SetSiblingIndex(i);
            roomPool[i].RoomRectTransform.anchoredPosition = i * (- Vector2.up * _recordOffsetY);
        }
    }

    private void OnScroll(Vector2 pos)
    {
        if(currentRooms.Count > roomPool.Count)
        {
            for (int i = 0; i < roomPool.Count; i++)
            {
                //pulling scrolrect down
                if (roomColumnScrolrect.transform.InverseTransformPoint(roomPool[i].transform.position).y > _disableMarginY + _treshold)
                {
                    _newAnchoredPosition = roomPool[i].RoomRectTransform.anchoredPosition;
                    _newAnchoredPosition.y -= roomPool.Count * _recordOffsetY;
                    roomPool[i].RoomRectTransform.anchoredPosition = _newAnchoredPosition;
                    roomPool[i].RoomRectTransform.SetAsLastSibling(); // item is moved up
                    OnMovedItem(roomPool[i], true);
                }

                //pulling scrolrect up
                else if (roomColumnScrolrect.transform.InverseTransformPoint(roomPool[i].transform.position).y < -_disableMarginY)
                {
                    _newAnchoredPosition = roomPool[i].RoomRectTransform.anchoredPosition;
                    _newAnchoredPosition.y += roomPool.Count * _recordOffsetY;
                    roomPool[i].RoomRectTransform.anchoredPosition = _newAnchoredPosition;
                    roomPool[i].RoomRectTransform.SetAsFirstSibling(); // item is moved down
                    OnMovedItem(roomPool[i], false);
                }
            }
        }
    }


    private void OnMovedItem(CalendarRoomColumnObject c, bool isUp)
    {
        int nextIndex = Mathf.Clamp(c.RoomIndex + (isUp ? roomPool.Count : -roomPool.Count), 0 ,currentRooms.Count - 1);
        c.UpdateRoomObjectUI(currentRooms[nextIndex], nextIndex);
    }
}
