using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class CalendarRoomColumn : MonoBehaviour
{
    public List<CalendarRoomColumnObject> ActiveRoomObjects => roomPool.FindAll(g => g.gameObject.activeSelf);
    public static float DisableMargin => _disableMarginY;


    [SerializeField]
    private ReservationObjectManager reservationsManager;

    [SerializeField]
    private GameObject roomColumnObjectPrefab;
    [SerializeField]
    private ScrollRect roomColumnScrolrect;

    [Space]
    [SerializeField]
    private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField]
    private ContentSizeFitter sizeFitter;

    [Space]
    [SerializeField]
    private List<CalendarRoomColumnObject> roomPool = new List<CalendarRoomColumnObject>();
    private List<IRoom> currentRooms;

    private Vector2 _newAnchoredPosition = Vector2.zero;
    private float _recordOffsetY = 0;
    private static float _disableMarginY = 0;
    private float _treshold = 100f;


    public List<CalendarRoomColumnObject> InitializeRoomColumn(UnityAction<IRoom> tapAction)
    {
        //populate pool
        verticalLayoutGroup.enabled = false;
        sizeFitter.enabled = false;

        for (int i = 0; i < roomColumnScrolrect.content.childCount; i++)
        {
            roomPool.Add(roomColumnScrolrect.content.GetChild(i).GetComponent<CalendarRoomColumnObject>());
            roomPool[i].SetObjectAction(tapAction);
        }

        _recordOffsetY = roomPool[0].RoomRectTransform.anchoredPosition.y - roomPool[1].RoomRectTransform.anchoredPosition.y;
        _disableMarginY = _recordOffsetY * roomPool.Count / 2;

        roomColumnScrolrect.onValueChanged.AddListener(OnScroll);

        return roomPool;
    }

    public void UpdateRooms(List<IRoom> rooms)
    {
        currentRooms = (rooms != null) ? rooms : new List<IRoom>();
        roomColumnScrolrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical ,rooms.Count * _recordOffsetY);
        roomColumnScrolrect.verticalNormalizedPosition = 1;

        for (int i = 0; i < roomPool.Count - 1; i++)
        {
            if(currentRooms.Count > i)
            {
                roomPool[i].UpdateObjectRoom(currentRooms[i], i);
            }
            else
            {
                roomPool[i].DisableObject();
            }

            roomPool[roomPool.Count -1].UpdateObjectRoom(null, -1);

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
        //int nextIndex = Mathf.Clamp(currentRooms.IndexOf(c.Room) + (isUp ? roomPool.Count : -roomPool.Count), 0 ,currentRooms.Count - 1);
        // int nextIndex = Mathf.Clamp(isUp ? LastItemRoomIndex() : FirstItemRoomIndex(), 0 ,currentRooms.Count - 1);

        // c.UpdateObjectRoom(currentRooms[nextIndex], nextIndex);

        c.UpdateObjectRoom(RoomByPosition(c), 0);

        if(c.Room != null)
        {
            reservationsManager.DisableUnseenReservations();
            reservationsManager.CreateReservationsForRow(c.LinkedDayRow);
        }
    }

    private IRoom RoomByPosition(CalendarRoomColumnObject c)
    {
        int roomHeightInRect = -(int)(c.RoomRectTransform.anchoredPosition.y/(roomColumnScrolrect.content.rect.height / currentRooms.Count));
        return currentRooms[Mathf.Clamp(roomHeightInRect, 0, currentRooms.Count - 1)];
    }

    private int LastItemRoomIndex()
    {
        return currentRooms.IndexOf(roomPool.Find(r => r.RoomRectTransform.GetSiblingIndex() == roomPool.Count -2).Room) + 1;
    }

    private int FirstItemRoomIndex()
    {
        return currentRooms.IndexOf(roomPool.Find(r => r.RoomRectTransform.GetSiblingIndex() == 1).Room) -1;
    }
}
