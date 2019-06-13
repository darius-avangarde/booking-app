using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using UnityEngine.Events;

[Serializable]
public class ClickEvent : UnityEvent { }

public class ScrollButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler,
							IDragHandler, IBeginDragHandler, IEndDragHandler,
							IPointerEnterHandler, IPointerExitHandler
{

	private bool OnHover = false;
	private bool IsDragging = false;
	private ScrollRect ScrollRectParent;

	public ClickEvent OnClick;
	public ClickEvent OnPointerDownEvent;
	public ClickEvent OnPointerUpEvent;
	public ClickEvent OnDragEvent;

	public void Start()
	{
		if (GetComponentInParent<ScrollRect>() != null)
			ScrollRectParent = transform.parent.GetComponentInParent<ScrollRect>();
	}


	public void OnPointerUp(PointerEventData eventData)
	{
        OnPointerUpEvent.Invoke();
        if (OnHover && !IsDragging && OnClick != null)
			OnClick.Invoke();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
        if (OnHover && !IsDragging)
        {
            OnPointerDownEvent.Invoke();
        }
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnHover = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnHover = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
        if (ScrollRectParent != null)
		{
			ScrollRectParent.OnDrag(eventData);
		}
        OnDragEvent.Invoke();
    }
	public void OnEndDrag(PointerEventData eventData)
	{
		if (ScrollRectParent != null)
		{
			ScrollRectParent.OnEndDrag(eventData);
			IsDragging = false;
		}
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (ScrollRectParent != null)
		{
			ScrollRectParent.OnBeginDrag(eventData);
			IsDragging = true;
		}
	}
}