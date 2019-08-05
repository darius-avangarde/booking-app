using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class InfiniteRoomScroll : MonoBehaviour
{
    private ScrollRect _scrollRect;
    private ContentSizeFitter _contentSizeFitter;
    private VerticalLayoutGroup _verticalLayoutGroup;
    private HorizontalLayoutGroup _horizontalLayoutGroup;
    private GridLayoutGroup _gridLayoutGroup;
    private float _disableMarginX = 0;
    private float _disableMarginY = 0;
    private bool _hasDisabledGridComponents = false;
    private List<RectTransform> items = new List<RectTransform>();
    private Vector2 _newAnchoredPosition = Vector2.zero;
    //TO DISABLE FLICKERING OBJECT WHEN SCROLL VIEW IS IDLE IN BETWEEN OBJECTS
    private float _treshold = 100f;
    private int _itemCount = 0;
    private float _recordOffsetX = 0;
    private float _recordOffsetY = 0;

    public UnityAction<Transform,bool> onMoveItem;


    public void Init()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.onValueChanged.AddListener(OnScroll);
        _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;

        for (int i = 0; i < _scrollRect.content.childCount; i++)
        {
            items.Add(_scrollRect.content.GetChild(i).GetComponent<RectTransform>());
        }
        if (_scrollRect.content.GetComponent<VerticalLayoutGroup>() != null)
        {
            _verticalLayoutGroup = _scrollRect.content.GetComponent<VerticalLayoutGroup>();
        }
        if (_scrollRect.content.GetComponent<HorizontalLayoutGroup>() != null)
        {
            _horizontalLayoutGroup = _scrollRect.content.GetComponent<HorizontalLayoutGroup>();
        }
        if (_scrollRect.content.GetComponent<GridLayoutGroup>() != null)
        {
            _gridLayoutGroup = _scrollRect.content.GetComponent<GridLayoutGroup>();
        }
        if (_scrollRect.content.GetComponent<ContentSizeFitter>() != null)
        {
            _contentSizeFitter = _scrollRect.content.GetComponent<ContentSizeFitter>();
        }

        _itemCount = _scrollRect.content.childCount;
    }

    void DisableGridComponents()
    {
        _recordOffsetX = items[1].GetComponent<RectTransform>().anchoredPosition.x - items[0].GetComponent<RectTransform>().anchoredPosition.x;
        _disableMarginX = _recordOffsetX * _itemCount / 2;//_scrollRect.GetComponent<RectTransform>().rect.width/2 + items[0].sizeDelta.x;

        if (_verticalLayoutGroup)
        {
            _verticalLayoutGroup.enabled = false;
        }
        if (_horizontalLayoutGroup)
        {
            _horizontalLayoutGroup.enabled = false;
        }
        if (_contentSizeFitter)
        {
            _contentSizeFitter.enabled = false;
        }
        if (_gridLayoutGroup)
        {
            _gridLayoutGroup.enabled = false;
        }
        _hasDisabledGridComponents = true;
    }

    public void OnScroll(Vector2 pos)
    {
        if (!_hasDisabledGridComponents)
            DisableGridComponents();

        for (int i = 0; i < items.Count; i++)
        {
            if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).y > _disableMarginY + _treshold)
            {
                _newAnchoredPosition = items[i].anchoredPosition;
                _newAnchoredPosition.y -= _itemCount * _recordOffsetY;
                items[i].anchoredPosition = _newAnchoredPosition;
                Transform item = _scrollRect.content.GetChild(_itemCount - 1).transform;
                item.SetAsFirstSibling();
                onMoveItem?.Invoke(item, false);
            }
            else if (_scrollRect.transform.InverseTransformPoint(items[i].gameObject.transform.position).y < -_disableMarginY)
            {
                _newAnchoredPosition = items[i].anchoredPosition;
                _newAnchoredPosition.y += _itemCount * _recordOffsetY;
                items[i].anchoredPosition = _newAnchoredPosition;
                Transform item = _scrollRect.content.GetChild(0).transform;
                item.SetAsLastSibling();
                onMoveItem?.Invoke(item, true);
            }
        }


    }
}
