using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollviewNoManualScroll : ScrollRect, IPointerUpHandler
{

    private ScrollRect controllingScrollrect = null;
    private UnityAction onEndDrag;
    private bool isVertical;

    public void SetControllingRect(ScrollRect scrollRect, UnityAction endDragAction, bool lockVertical)
    {
        controllingScrollrect = scrollRect;
        onEndDrag = endDragAction;
        isVertical = lockVertical;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        controllingScrollrect.vertical = isVertical;
        controllingScrollrect.horizontal = !isVertical;
        controllingScrollrect.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        controllingScrollrect.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        controllingScrollrect.OnEndDrag(eventData);
        controllingScrollrect.vertical = controllingScrollrect.horizontal = true;
        onEndDrag?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       onEndDrag?.Invoke();
    }
}
