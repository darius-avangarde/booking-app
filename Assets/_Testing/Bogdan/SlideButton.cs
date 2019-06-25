using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlideButton : MonoBehaviour
{
    [SerializeField]
    private Text buttonText;
    [SerializeField]
    private RectTransform referenceOpenRect;
    [SerializeField]
    private RectTransform fillRect;

    [SerializeField]
    private float closeDelay = 3.0f;
    [SerializeField]
    private float slideTime = 0.15f;
    [SerializeField]
    private UnityEvent onClick;

    public string ButtonText
    {
        set{ buttonText.text = value; }
    }



    private void OnEnable()
    {
        ButtonText = "Yo buttonama";
        fillRect.gameObject.SetActive(true);
        SizeRect(fillRect, fillRect.rect.width);
        StartCoroutine(WaitForClose());
    }

    public void ButtonAction()
    {
        onClick?.Invoke();
        StopAllCoroutines();
        StartCoroutine(Close());
    }

    private IEnumerator WaitForClose()
    {
        yield return new WaitForSeconds(closeDelay);
        StartCoroutine(Close());
    }

    private IEnumerator Close()
    {
        float startSize = fillRect.rect.width;
        float endSize = 0.1f;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/slideTime)
        {
            SizeRect(fillRect, Mathf.Lerp(startSize, endSize, t*t*t));
            yield return null;
        }

        fillRect.gameObject.SetActive(false);
        SizeRect(fillRect, startSize);
    }

    private void SizeRect(RectTransform rectTransform,float setSize)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, setSize);
    }


}
