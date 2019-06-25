using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WaveObject : MonoBehaviour
{

    [SerializeField]
    private RectTransform thisTransform;
    [SerializeField]
    private Image waveImage;

    public Color StartColor{set => startColor = value;}
    private Color startColor;



    private void OnEnable()
    {
        StopAllCoroutines();
        ResetObject();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ResetObject();
    }

    public void StartWaveAnimation(float waveTime, Vector3 endScale, Color waveEndColor)
    {
        gameObject.SetActive(true);
        StartCoroutine(WaveAnimation(waveTime, endScale, waveEndColor));
    }

	private IEnumerator WaveAnimation(float waveTime, Vector3 endScale, Color waveEndColor)
	{
		waveImage.color = startColor;
        thisTransform.localScale = Vector2.zero;

		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/waveTime)
		{
			waveImage.color = Color.Lerp(startColor, waveEndColor,Mathf.Sin(t * Mathf.PI));
			thisTransform.localScale = Vector3.Lerp(Vector3.zero, endScale,t);
			yield return null;
		}

		ResetObject();
		gameObject.SetActive(false);
	}

    private void ResetObject()
    {
		waveImage.color = startColor;
		thisTransform.localScale = Vector2.one;
    }
}
