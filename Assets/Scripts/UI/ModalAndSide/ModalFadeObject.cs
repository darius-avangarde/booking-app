using System.Collections;
using UnityEngine;

public class ModalFadeObject : MonoBehaviour
{
    [SerializeField]
    private float fadeTime = 0.35f;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup.alpha = 0;
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeTimer(true));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTimer(false));
    }

    private IEnumerator FadeTimer(bool fadingIn)
    {
        canvasGroup.interactable = false;
        float from = canvasGroup.alpha;
        float to = (fadingIn) ? 1 : 0;


        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;

        if(!fadingIn)
        {
            canvasGroup.gameObject.SetActive(false);
        }
        canvasGroup.interactable = fadingIn;
    }
}
