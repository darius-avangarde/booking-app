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
	private float waveTime = 0.25f;

	[Space]
	[SerializeField]
	private Color waveMaxColor;

	[Space]
	[SerializeField]
	private float endScaleFactor = 7.5f;

	private Vector3 startScale;
	private Vector3 endScale;
	public List<WaveObject> pool = new List<WaveObject>();


	void Start()
	{
		endScale = Vector3.one * endScaleFactor;
		AddToPool(poolSize);
	}

	private void Update ()
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

	private void AddToPool(int size)
	{
		for (int i = 0; i < size; i++)
		{
			pool.Add(NewWaveObject());
		}
	}

	private WaveObject NewWaveObject()
	{
		Color endColor = waveMaxColor;
		endColor.a = 0;

		WaveObject wave = Instantiate(waveObjectPrefab).GetComponent<WaveObject>();
		wave.StartColor = endColor;
		wave.gameObject.SetActive(false);
		return wave;
	}

	private WaveObject GetFreeObject()
	{
		WaveObject returnObject = null;

		for (int i = 0; i < pool.Count; i++)
		{
			//look for the first active non null wave object in the pool
			if(pool[i] != null)
			{
				if(returnObject == null && !pool[i].gameObject.activeInHierarchy)
				{
					returnObject = pool[i];
				}
			}
			//readd deleted items and return deleted items
			else
			{
				pool[i] = NewWaveObject();
			}
		}

		return returnObject;
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
