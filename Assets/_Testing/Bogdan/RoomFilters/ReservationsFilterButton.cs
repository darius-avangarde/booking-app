using System.Collections;
using UnityEngine;

public class ReservationsFilterButton : MonoBehaviour
{
    [SerializeField]
    private RectTransform filtersRect;
    [SerializeField]
    private RectTransform referenceRect;

    [SerializeField]
    private float slideTime = 0.15f;


    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(Slide(true));
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Slide(false));
    }

    private IEnumerator Slide(bool open)
    {
        Debug.Log("starting slide");
        float fromSize = filtersRect.rect.width;
        float toSize = (open) ? referenceRect.rect.width : referenceRect.rect.width * 2;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/slideTime)
        {
            filtersRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(fromSize, toSize, t));
            yield return null;
        }

        filtersRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, toSize);
    }
}
