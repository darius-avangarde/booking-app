using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;

public class RoomPicker : MonoBehaviour
{
    [SerializeField]
    private GameObject pickerParent;
    [SerializeField]
    private RectTransform pickerParentRect;
    [SerializeField]
    private ScrollRect pickerScrolrect;
    [SerializeField]
    private Text selectedItemsText;
    [SerializeField]
    private GameObject confirmationButton;
    [SerializeField]
    private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField]
    private GameObject closeButton;


    [SerializeField]
    private List<RoomPickerObject> roomPool = new List<RoomPickerObject>();
    private List<IRoom> currentRooms = new List<IRoom>();
    private List<IRoom> selectedRooms = new List<IRoom>();
    private UnityAction<List<IRoom>> onConfirm;

    private IProperty currentPropery;

    private bool isSelecting = false;
    private Vector2 _newAnchoredPosition = Vector2.zero;
    private float _recordOffsetY = 0;
    private static float _disableMarginY = 0;
    private float _treshold = 200f;
    private DateTime? fromDate;
    private DateTime? toDate;
    private IReservation currentReservation;



    private void Start()
    {
        verticalLayoutGroup.enabled = false;
        for (int i = 0; i < pickerScrolrect.content.childCount; i++)
        {
            roomPool.Add(pickerScrolrect.content.GetChild(i).GetComponent<RoomPickerObject>());
            roomPool[i].SetObjectActions(OnObjectTap, OnObjectToggle);
        }

        _recordOffsetY = roomPool[0].ObjRect.anchoredPosition.y - roomPool[1].ObjRect.anchoredPosition.y;
        _disableMarginY = _recordOffsetY * roomPool.Count / 2;

        pickerScrolrect.onValueChanged.AddListener(OnScroll);
    }

    public void Initialize(List<IRoom> allRooms, UnityAction<List<IRoom>> confirmSelectionCallback, List<IRoom> selected = null)
    {
        pickerScrolrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, allRooms.Count * _recordOffsetY);
        currentPropery = PropertyDataManager.GetProperty(allRooms[0].PropertyID);
        currentRooms = allRooms;
        selectedRooms = (selected != null) ? new List<IRoom>(selected) : new List<IRoom>();
        selectedItemsText.text = selectedRooms.Count > 1 ?
            $"{selectedRooms.Count} {LocalizedText.Instance.RoomNumbers}" :
            (selectedRooms.Count > 0 ? selectedRooms[0].Name : "Alege camere");
        onConfirm = confirmSelectionCallback;
    }

    //TODO: Add datetime and check room availavility
    public void Open(List<IRoom> selected, DateTime? start, DateTime? end, IReservation res)
    {
        fromDate = start;
        toDate = end;
        currentReservation = res;

        selectedRooms = (selected != null) ? new List<IRoom>(selected) : new List<IRoom>();

        pickerParent.SetActive(true);
        closeButton.SetActive(true);
        StartCoroutine(OpenAnimation(Mathf.Min(currentRooms.Count, 7) *_recordOffsetY));
        isSelecting = selectedRooms.Count > 0;
        confirmationButton.SetActive(isSelecting);
        UpdateVisible();
    }

    public void ConfirmSelection()
    {
        onConfirm?.Invoke(selectedRooms);
        selectedItemsText.text = selectedRooms.Count > 1 ? $"{selectedRooms.Count} camere alese." : selectedRooms[0].Name;
        Close();
    }

    public void Close()
    {
        pickerParent.SetActive(false);
        closeButton.SetActive(false);
    }

    private void UpdateVisible()
    {
        for (int i = 0; i < roomPool.Count; i++)
        {
            if(currentRooms.Count > i)
            {
                roomPool[i].SetRoom(currentRooms[i], selectedRooms.Contains(currentRooms[i]), IsRoomOccupied(currentRooms[i]));
            }
            else
                roomPool[i].DisableObject();
        }
    }

    private void OnScroll(Vector2 pos)
    {
        if(currentRooms != null && currentRooms.Count > roomPool.Count)
        {
            for (int i = 0; i < roomPool.Count; i++)
            {
                //pulling scrolrect down
                if (pickerScrolrect.transform.InverseTransformPoint(roomPool[i].transform.position).y > _disableMarginY + _treshold)
                {
                    _newAnchoredPosition = roomPool[i].ObjRect.anchoredPosition;
                    _newAnchoredPosition.y -= roomPool.Count * _recordOffsetY;
                    roomPool[i].ObjRect.anchoredPosition = _newAnchoredPosition;
                    roomPool[i].ObjRect.SetAsLastSibling(); // item is moved up
                    OnMovedItem(roomPool[i]);
                }

                //pulling scrolrect up
                else if (pickerScrolrect.transform.InverseTransformPoint(roomPool[i].transform.position).y < -_disableMarginY + _treshold)
                {
                    _newAnchoredPosition = roomPool[i].ObjRect.anchoredPosition;
                    _newAnchoredPosition.y += roomPool.Count * _recordOffsetY;
                    roomPool[i].ObjRect.anchoredPosition = _newAnchoredPosition;
                    roomPool[i].ObjRect.SetAsFirstSibling(); // item is moved down
                    OnMovedItem(roomPool[i]);
                }
            }
        }
    }

    private void OnMovedItem(RoomPickerObject c)
    {
        IRoom next = RoomByPosition(c);
        c.SetRoom(next, selectedRooms.Contains(next), IsRoomOccupied(next));
    }

    private IRoom RoomByPosition(RoomPickerObject c)
    {
        int roomHeightInRect = -(int)(c.ObjRect.anchoredPosition.y/(pickerScrolrect.content.rect.height / currentRooms.Count));
        return currentRooms[Mathf.Clamp(roomHeightInRect, 0, currentRooms.Count - 1)];
    }

    private void OnObjectTap(IRoom room, RoomPickerObject toggleObject)
    {
        if(!isSelecting)
        {
            selectedRooms.Clear();
            selectedRooms.Add(room);
            ConfirmSelection();
        }
        else
        {
            toggleObject.IsOn = !toggleObject.IsOn;
        }
    }

    private void OnObjectToggle(bool toggleState, IRoom room)
    {
        if(toggleState)
            selectedRooms.Add(room);
        else
            selectedRooms.Remove(room);

        isSelecting = selectedRooms.Count != 0;
        confirmationButton.SetActive(isSelecting);
    }

    private IEnumerator OpenAnimation(float openSize)
    {
        pickerParentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/0.25f){
            pickerParentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(0, openSize, t*t));
            yield return null;
        }
        pickerParentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, openSize);
    }

    private bool IsRoomOccupied(IRoom room)
    {
        if(fromDate.HasValue && toDate.HasValue)
            return ReservationDataManager.IsRoomFreeOn(room, fromDate.Value, toDate.Value, (currentReservation != null) ? currentReservation.ID : string.Empty);
        else
            return false;
    }
}
