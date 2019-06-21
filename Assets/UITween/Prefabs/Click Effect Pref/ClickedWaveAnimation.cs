using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickedWaveAnimation : MonoBehaviour {

	[SerializeField]
	private GameObject waveObjectPrefab;
	[SerializeField]
	private GameObject canvasMain;
	[SerializeField]
	private int poolSize = 5;

	[SerializeField]
	private float waveTime = 0.15f;

	[Space]
	[SerializeField]
	private Color waveMaxColor;

	[Space]
	[SerializeField]
	private float endScaleFactor = 10.0f;

	private Vector3 startScale;
	private Vector3 endScale;
	private List<WaveObject> pool = new List<WaveObject>();


	void Start()
	{
		endScale = Vector3.one * endScaleFactor;
		AddToPool(poolSize);
	}

	private void AddToPool(int size)
	{
		Color endColor = waveMaxColor;
		endColor.a = 0;

		for (int i = 0; i < size; i++)
		{
			WaveObject wave = Instantiate(waveObjectPrefab).GetComponent<WaveObject>();
			pool.Add(wave);
			wave.StartColor = endColor;
			wave.gameObject.SetActive(false);
		}
	}

	private WaveObject GetFreeObject()
	{
		for (int i = 0; i < pool.Count; i++)
		{
			if(!pool[i].gameObject.activeInHierarchy)
			{
				return pool[i];
			}
		}
		return null;
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown(0)
#if UNITY_EDITOR
		    || Input.GetMouseButtonDown(1)
#endif
		    )
		{
			GameObject hitUIButton = UiHit();

			if (hitUIButton)
			{
				CreateWave(hitUIButton.transform);
			}
		}
	}

	private void CreateWave(Transform Parent)
	{
		WaveObject wave = GetFreeObject();

		if (wave != null)
		{
			wave.transform.SetParent( canvasMain.transform );
			wave.transform.SetAsLastSibling();

			Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			mousePos.x = mousePos.x * Screen.width - Screen.width / 2f;
			mousePos.y = mousePos.y * Screen.height - Screen.height / 2f;
			mousePos.z = 0f;

			wave.transform.localPosition = mousePos / canvasMain.transform.localScale.x;
			wave.transform.SetParent(Parent);

			wave.StartWaveAnimation(waveTime, endScale, waveMaxColor);
		}
	}

	private GameObject UiHit()
	{
		PointerEventData pe = new PointerEventData(EventSystem.current);
		pe.position =  Input.mousePosition;

		List<RaycastResult> hits = new List<RaycastResult>();
		EventSystem.current.RaycastAll( pe, hits );

		for (int i = 0; i < hits.Count ; i++)
		{
			if (hits[i].gameObject.GetComponent<Button>() || hits[i].gameObject.GetComponent<Toggle>())
			{
				return hits[i].gameObject.GetComponent<Mask>() ? hits[i].gameObject : null;
			}
		}

		return null;
	}
}
