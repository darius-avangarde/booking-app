using System.Collections;
using UnityEngine;

public class DropdownAnimationByHeight : MonoBehaviour
{
    [SerializeField]
    private RectTransform contentRect;
    [SerializeField]
    protected bool horizontalPosition = false;

    protected RectTransform rectTransformComponent;
    protected float finalHeight;

    private void Awake()
    {
        rectTransformComponent = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rectTransformComponent.sizeDelta = new Vector2(rectTransformComponent.sizeDelta.x, 0);
        StartCoroutine(WaitForInitialization());
    }

    protected virtual IEnumerator WaitForInitialization()
    {
        yield return null;

        finalHeight = contentRect.rect.height;

        float pivotPosX = rectTransformComponent.pivot.x;
        if (horizontalPosition)
        {
            pivotPosX = rectTransformComponent.position.x / Screen.width;
        }
        float pivotPosY = rectTransformComponent.position.y / Screen.height;

        rectTransformComponent.pivot = new Vector2(pivotPosX, pivotPosY);

        StartCoroutine(ExpandDropdown());
    }

    protected IEnumerator ExpandDropdown()
    {
        Vector2 finalSize = new Vector2(rectTransformComponent.sizeDelta.x, finalHeight);
        float currentTime = 0;
        float duration = 1f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            rectTransformComponent.sizeDelta = Vector2.Lerp(rectTransformComponent.sizeDelta, finalSize, currentTime / duration);
            yield return null;
        }
        rectTransformComponent.sizeDelta = finalSize;
    }
}
