using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeaderInteraction : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	public RectTransform Header;
	[Space()]
	public CanvasGroup Show;
	public CanvasGroup Hide;

	private bool isOpen = false;
	public bool IsOpen
	{
		get { return isOpen; }
		set { isOpen = value;
			scroll.vertical = !value;
			}
	}

	private ScrollRect scroll;
	private RectTransform scrollTrans;
	private RectTransform contentTrans;

	private float initialHeight = 0;
	private float headerHeight = 0;
	private float heightFactor = 0f;
	private float minHeightFactor = 0f;


    void Start()
    {
		scroll = GetComponent<ScrollRect>();
		scrollTrans = scroll.GetComponent<RectTransform>();
		initialHeight = transform.parent.GetComponent<RectTransform>().rect.height;
		contentTrans = scroll.content.GetComponent<RectTransform>();
		headerHeight = Header.rect.height; //750
		minHeightFactor = 286 / headerHeight; 
		heightFactor = minHeightFactor;
		Header.sizeDelta = new Vector2(Header.sizeDelta.x, headerHeight * heightFactor);
	}

	private void FixedUpdate()
	{
		Header.sizeDelta = new Vector2(Header.sizeDelta.x, headerHeight * heightFactor);
		scrollTrans.sizeDelta = new Vector2(scrollTrans.sizeDelta.x, initialHeight - Header.sizeDelta.y);
		Show.alpha = (heightFactor - minHeightFactor) / (1 - minHeightFactor);
		Hide.alpha = 1 - (heightFactor - minHeightFactor) / (1 - minHeightFactor);
	}


	// DRAG HANDLERS
	
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!IsOpen && contentTrans.anchoredPosition.y <= 0.01f && eventData.delta.y < 0)
			IsOpen = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (IsOpen)
		{
			heightFactor += -1 * eventData.delta.y / 100f * 0.2f; // 0.5f is sensibility
			heightFactor = Mathf.Clamp(heightFactor, minHeightFactor, 1f);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if( heightFactor > (minHeightFactor+1)/2f )
		{
			IsOpen = true;
			StartCoroutine(AnimateHeader(1f));
		}

		if (heightFactor <= (minHeightFactor + 1) / 2f)
		{
			IsOpen = false;
			StartCoroutine(AnimateHeader(minHeightFactor));
		}
	}


	private IEnumerator AnimateHeader(float target)
	{
		float init = heightFactor;
		for (float i = 0; i < 0.15f; i+=Time.deltaTime)
		{
			heightFactor = Mathf.Lerp(init, target, i / 0.15f);
			yield return null;
		}

		heightFactor = target;
	}
	
}
