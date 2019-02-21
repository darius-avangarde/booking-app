using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToastController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public float waitTime = 2f;

    [SerializeField]
    private Text text = null;
    [SerializeField]
    private CanvasGroup canvasGroup = null;

    public void Show(string message)
    {
        StopAllCoroutines();
        canvasGroup.blocksRaycasts = true;
        text.text = message;
        canvasGroup.alpha = 0.8f;
        StartCoroutine(WaitAndFadeOut());
    }

    private IEnumerator WaitAndFadeOut()
    {
        yield return new WaitForSeconds(waitTime);

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }
        canvasGroup.blocksRaycasts = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canvasGroup.blocksRaycasts == true)
        {
            StopAllCoroutines();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canvasGroup.blocksRaycasts == true)
        {
            StopAllCoroutines();
            canvasGroup.alpha = 1f;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canvasGroup.blocksRaycasts == true)
        {
            StopAllCoroutines();
            canvasGroup.alpha = 0.8f;
            StartCoroutine(WaitAndFadeOut());
        }
    }
}
