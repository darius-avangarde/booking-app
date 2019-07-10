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
    private ScrollRect RoomsColumnScrollrect;
    [SerializeField]
    private ScrollRect DayHeaderScrollrect;
    [SerializeField]
    private ScrollRect DayColumnScrollrect;

    [Space]
    [SerializeField]
    private RectTransform DayColumnPrefabRectTransform;

    [Space]
    [SerializeField]
    private float snapTime = 0.2f;
    [SerializeField]
    private AnimationCurve snapCurve;

    private Vector2 startPos;
    private bool isSnaping = false;
    private bool isScrolling = false;


    private void Start()
    {
        DayColumnScrollrect.onValueChanged.AddListener(MatchPositionMain);
        DayHeaderScrollrect.onValueChanged.AddListener(MatchPositionRooms);
        DayHeaderScrollrect.onValueChanged.AddListener(MatchPositionDayHeader);

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
        isScrolling = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CancelSnap();
        LockDirection(Mathf.Abs(startPos.x - eventData.position.x) < Mathf.Abs(startPos.y - eventData.position.y));
        isScrolling = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        BeginSnap(DayColumnScrollrect);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        BeginSnap(DayColumnScrollrect);
    }

    public void BeginSnap(ScrollRect scrollRect)
    {
        StopAllCoroutines();
        if(!isSnaping)
        {
            Debug.Log("doingSnap");
            StartCoroutine(Snap(scrollRect));
        }
    }

    public void CancelSnap(bool cancelIsScrolling = false)
    {
        StopAllCoroutines();
        isSnaping = false;

        if(cancelIsScrolling)
            isScrolling = false;
    }

    private void MatchPositionMain(Vector2 position)
    {
        if(isScrolling)
        {
            RoomsColumnScrollrect.verticalNormalizedPosition = position.y;
            DayHeaderScrollrect.horizontalNormalizedPosition = position.x;
        }
    }

    private void MatchPositionDayHeader(Vector2 position)
    {
        if (!isScrolling)
        {
            DayColumnScrollrect.horizontalNormalizedPosition = position.x;
        }
    }

    private void MatchPositionRooms(Vector2 position)
    {
        if (!isScrolling)
        {
            DayColumnScrollrect.verticalNormalizedPosition = position.y;
        }
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

    private IEnumerator Snap(ScrollRect scrollRect)
    {
        isSnaping = true;
        //presnap delay
        yield return new WaitForSeconds(0.5f);

        Vector2 current = scrollRect.normalizedPosition;
        Vector2 target = NearestSnapPoint(scrollRect);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/snapTime)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(current, target, snapCurve.Evaluate(t));
            yield return null;
        }
        scrollRect.normalizedPosition = target;
        isSnaping = false;
        isScrolling = false;
    }

    private Vector2 NearestSnapPoint(ScrollRect scrollRect)
    {
        Vector2 snapPos = Vector2.zero;
        snapPos.y = scrollRect.normalizedPosition.y;

        float normalizedWidth = scrollRect.viewport.rect.width / (scrollRect.content.rect.width - scrollRect.viewport.rect.width) + 1;
        float snapSize = normalizedWidth/scrollRect.content.childCount;
        snapPos.x = Mathf.RoundToInt(scrollRect.horizontalNormalizedPosition/snapSize) * snapSize;

        return snapPos;
    }
}
