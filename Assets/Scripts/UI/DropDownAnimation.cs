using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DropDownAnimation : MonoBehaviour
{
    public float MaxNumberOfItems = 7;
    [SerializeField]
    private RectTransform itemRectTransform;
    [SerializeField]
    private bool horizontalPosition = false;

    private RectTransform rectTransformComponent;
    private Dropdown dropdownComponent;
    private float finalHeight;

    private void Awake()
    {
        rectTransformComponent = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rectTransformComponent.sizeDelta = new Vector2(rectTransformComponent.sizeDelta.x, 0);
        StartCoroutine(WaitForInitialization());
    }

    private IEnumerator WaitForInitialization()
    {
        yield return null;
        if (dropdownComponent == null)
        {
            dropdownComponent = GetComponentInParent<Dropdown>();
        }
        if (dropdownComponent != null)
        {
            int numberOfItems = Mathf.Min(dropdownComponent.options.Count, (int)MaxNumberOfItems);
            finalHeight = numberOfItems * itemRectTransform.rect.height;
        }
        else
        {
            finalHeight = MaxNumberOfItems * itemRectTransform.rect.height;
        }
        float pivotPosX = rectTransformComponent.pivot.x;
        if (horizontalPosition)
        {
            pivotPosX = rectTransformComponent.position.x / Screen.width;
        }
        float pivotPosY = rectTransformComponent.position.y / Screen.height;
        rectTransformComponent.pivot = new Vector2(pivotPosX, pivotPosY);
        StartCoroutine(ExpandDropdown());
    }

    private IEnumerator ExpandDropdown()
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
