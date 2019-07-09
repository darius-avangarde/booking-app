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


    private void Start()
    {
        DayColumnScrollrect.onValueChanged.AddListener((v) => RoomsColumnScrollrect.normalizedPosition = v);
        DayColumnScrollrect.onValueChanged.AddListener((v) => DayHeaderScrollrect.normalizedPosition = v);

        //Set listeners for changing the content rect sizes (layout groups and content size fitters are disables for infinite scroll)
        ResolutionChangeManager.AddListener(
            () =>
                {
                    DayColumnScrollrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal ,DayColumnScrollrect.content.childCount * DayColumnPrefabRectTransform.rect.width);
                    DayHeaderScrollrect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal ,DayColumnScrollrect.content.childCount * DayColumnPrefabRectTransform.rect.width);
                }
            );
    }

    private void Update()
    {
        DayColumnScrollrect.verticalNormalizedPosition = Mathf.Clamp01(DayColumnScrollrect.verticalNormalizedPosition);
    }


    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        CancelSnap();
        startPos = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CancelSnap();
        LockDirection(Mathf.Abs(startPos.x - eventData.position.x) < Mathf.Abs(startPos.y - eventData.position.y));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        BeginSnap();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        BeginSnap();
    }

    private void LockDirection(bool isVertical)
    {
        DayColumnScrollrect.vertical = isVertical;
        DayColumnScrollrect.horizontal = !isVertical;
    }

    private void BeginSnap()
    {
        StopAllCoroutines();
        if(!isSnaping)
        {
            StartCoroutine(Snap());
        }
    }

    private void CancelSnap()
    {
        StopAllCoroutines();
        isSnaping = false;
    }



    private IEnumerator Snap()
    {
        isSnaping = true;
        //presnap delay
        yield return new WaitForSeconds(0.5f);

        Vector2 current = DayColumnScrollrect.normalizedPosition;
        Vector2 target = NearestSnapPoint();

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/snapTime)
        {
            DayColumnScrollrect.normalizedPosition = Vector2.Lerp(current, target, snapCurve.Evaluate(t));
            yield return null;
        }
        DayColumnScrollrect.normalizedPosition = target;
        isSnaping = false;
    }

    private Vector2 NearestSnapPoint()
    {
        Vector2 snapPos = Vector2.zero;
        snapPos.y = DayColumnScrollrect.normalizedPosition.y;

        float normalizedWidth = DayColumnScrollrect.viewport.rect.width / (DayColumnScrollrect.content.rect.width - DayColumnScrollrect.viewport.rect.width) + 1;
        float snapSize = normalizedWidth/DayColumnScrollrect.content.childCount;
        snapPos.x = Mathf.RoundToInt(DayColumnScrollrect.horizontalNormalizedPosition/snapSize) * snapSize;

        return snapPos;
    }
}
