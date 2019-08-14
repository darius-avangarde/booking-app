using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

///<summary>
///Handles the three scrolrects specific to the main screen room/day view. Must be attached to the main room/day scrolrect.
///</summary>
public class ScrollviewHandler : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IPointerUpHandler
{
    [SerializeField]
    private bool doVerticalSnap = false;

    [SerializeField]
    private ScrollviewNoManualScroll RoomsColumnScrollrect;
    [SerializeField]
    private ScrollviewNoManualScroll DayHeaderScrollrect;
    [SerializeField]
    private ScrollRect DayColumnScrollrect;
    [SerializeField]
    private RectTransform ReservationsColumnRect;

    [Space]
    [SerializeField]
    private RectTransform DayColumnPrefabRectTransform;
    [SerializeField]
    private RectTransform roomObjectPrefabTransform;

    [Space]
    [SerializeField]
    private float snapTime = 0.2f;
    [SerializeField]
    private AnimationCurve snapCurve;

    private Vector2 startPos;
    private bool isSnaping = false;
    //private bool isScrolling = false;


    private void Start()
    {
        DayColumnScrollrect.onValueChanged.AddListener(MatchPositionMain);
        DayHeaderScrollrect.SetControllingRect(DayColumnScrollrect,BeginSnap, false);
        RoomsColumnScrollrect.SetControllingRect(DayColumnScrollrect, BeginSnap, true);


        //Set listeners for changing the content rect sizes (layout groups and content size fitters are disables for infinite scroll)
        ResolutionChangeManager.AddListener(MatchSize);
    }

    private void Update()
    {
        DayColumnScrollrect.verticalNormalizedPosition = Mathf.Clamp01(DayColumnScrollrect.verticalNormalizedPosition);
    }


    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        CancelSnap();
        startPos = eventData.position;
        //isScrolling = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(isSnaping)
            CancelSnap();

        LockDirection(Mathf.Abs(startPos.x - eventData.position.x) < Mathf.Abs(startPos.y - eventData.position.y));
        //isScrolling = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        BeginSnap();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        BeginSnap();
    }

    public void BeginSnap()
    {
        if(!isSnaping)
            StartCoroutine(Snap());
    }

    public void KillMainVelocity()
    {
        DayColumnScrollrect.velocity = Vector2.zero;
        DayHeaderScrollrect.velocity = Vector2.zero;
        RoomsColumnScrollrect.velocity = Vector2.zero;
    }

    public void InstantSnap()
    {
        float snapToX = NearestSnapPointX();
        DayColumnScrollrect.horizontalNormalizedPosition = snapToX;
        DayHeaderScrollrect.horizontalNormalizedPosition = snapToX;

        float snapToY = NearestSnapPointY();
        DayColumnScrollrect.verticalNormalizedPosition = snapToY;
        RoomsColumnScrollrect.verticalNormalizedPosition = snapToY;
    }

    public void CancelSnap(bool cancelIsScrolling = false)
    {
        StopAllCoroutines();
        isSnaping = false;

        //if(cancelIsScrolling)
        //    isScrolling = false;
    }

    public void ForceMatchVerticalPosition()
    {
        DayColumnScrollrect.verticalNormalizedPosition = RoomsColumnScrollrect.verticalNormalizedPosition;
    }

    private void MatchPositionMain(Vector2 position)
    {
        DayHeaderScrollrect.horizontalNormalizedPosition = position.x;
        RoomsColumnScrollrect.verticalNormalizedPosition = position.y;
        ReservationsColumnRect.position = DayColumnScrollrect.content.position;
    }

    private void MatchSize()
    {
        DayColumnScrollrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal ,DayColumnScrollrect.content.childCount * DayColumnPrefabRectTransform.rect.width);
        DayHeaderScrollrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal ,DayColumnScrollrect.content.childCount * DayColumnPrefabRectTransform.rect.width);
    }

    private void LockDirection(bool isVertical)
    {
        DayColumnScrollrect.vertical = isVertical;
        DayColumnScrollrect.horizontal = !isVertical;
    }

    private IEnumerator Snap()
    {
        isSnaping = true;
        //presnap delay
        yield return new WaitForSeconds(0.5f);

        float currentX = DayColumnScrollrect.horizontalNormalizedPosition;
        float targetX = NearestSnapPointX();

        float currentY = DayColumnScrollrect.verticalNormalizedPosition;
        float targetY = NearestSnapPointY();

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/snapTime)
        {
            DayColumnScrollrect.horizontalNormalizedPosition = Mathf.Lerp(currentX, targetX, snapCurve.Evaluate(t));
            if(doVerticalSnap) DayColumnScrollrect.verticalNormalizedPosition = Mathf.Lerp(currentY, targetY, snapCurve.Evaluate(t));
            yield return null;
        }
        DayColumnScrollrect.horizontalNormalizedPosition = targetX;
        if(doVerticalSnap) DayColumnScrollrect.verticalNormalizedPosition = targetY;
        isSnaping = false;
        //isScrolling = false;

        System.GC.Collect();
    }

    private float NearestSnapPointX()
    {
        float snapPosX = 1 - DayColumnScrollrect.horizontalNormalizedPosition;

        float normalizedWidth = DayColumnScrollrect.viewport.rect.width / (DayColumnScrollrect.content.rect.width - DayColumnScrollrect.viewport.rect.width) + 1;
        float snapSizeX = normalizedWidth/DayColumnScrollrect.content.childCount;
        snapPosX = Mathf.RoundToInt(DayColumnScrollrect.horizontalNormalizedPosition/snapSizeX) * snapSizeX;

        return snapPosX;
    }

    private float NearestSnapPointY()
    {
        float snapPosY = DayColumnScrollrect.verticalNormalizedPosition;

        float normalizedHeight = DayColumnScrollrect.viewport.rect.height / (DayColumnScrollrect.content.rect.height - DayColumnScrollrect.viewport.rect.height) + 1;

        float snapSizeY = normalizedHeight/(RoomsColumnScrollrect.content.rect.height/roomObjectPrefabTransform.rect.height);

        float normalizedRemainder = 1 % snapSizeY; //The bit left at the top after placing objects to cover the viewport

        snapPosY = Mathf.RoundToInt(DayColumnScrollrect.verticalNormalizedPosition/snapSizeY) * snapSizeY + normalizedRemainder;

        if(snapPosY < snapSizeY ) //if the snap point is near/would be the last > snap to end
            return 0;

        return snapPosY;
    }
}
