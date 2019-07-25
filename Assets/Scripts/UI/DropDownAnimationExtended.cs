using System.Collections;
using UnityEngine;

public class DropDownAnimationExtended : DropDownAnimation
{
    [SerializeField]
    private RectTransform contentsRect;
    [SerializeField]
    private bool fixedPivotPosition = false;
    [SerializeField]
    private bool countActiveObjectsOnly = false;


    public void ManualAnimationTrigger()
    {
        StopAllCoroutines();
        StartCoroutine(WaitForInitialization());
    }

    protected override IEnumerator WaitForInitialization()
    {
        yield return null;

        int numberOfItems = Mathf.Min(ItemCount(), (int)MaxNumberOfItems);
        finalHeight = numberOfItems * itemRectTransform.rect.height;

        if(!fixedPivotPosition)
        {
            float pivotPosX = rectTransformComponent.pivot.x;
            if (horizontalPosition)
            {
                pivotPosX = rectTransformComponent.position.x / Screen.width;
            }
            float pivotPosY = rectTransformComponent.position.y / Screen.height;

            rectTransformComponent.pivot = new Vector2(pivotPosX, pivotPosY);
        }
        StartCoroutine(ExpandDropdown());
    }

    private int ItemCount()
    {
        if(countActiveObjectsOnly)
        {
            int activeChildren = 0;
            for (int i = 0; i < contentsRect.childCount; i++)
            {
                if(contentsRect.GetChild(i).gameObject.activeSelf)
                    activeChildren ++;
            }
            return Mathf.Max(activeChildren, 1);
        }
        else
        {
            return contentsRect.childCount;
        }
    }
}
