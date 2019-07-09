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

    [SerializeField]
    private float snapTime = 0.2f;

    private Vector2 startPos;
    private bool isSnaping = false;


    private void Start()
    {
        DayColumnScrollrect.onValueChanged.AddListener((v) => RoomsColumnScrollrect.normalizedPosition = v);
        DayColumnScrollrect.onValueChanged.AddListener((v) => DayHeaderScrollrect.normalizedPosition = v);
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

        Debug.Log("snapping from: " + current.x);
        // Debug.Log("snappin from " + ReservationsScrollrect.normalizedPosition.y + "  to " + target.y);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/snapTime){
            DayColumnScrollrect.normalizedPosition = Vector2.Lerp(current, target, t);
            yield return null;
        }
        DayColumnScrollrect.normalizedPosition = target;
        isSnaping = false;
    }

    private Vector2 NearestSnapPoint()
    {
        Vector2 snapPos = Vector2.zero;
        snapPos.y = DayColumnScrollrect.normalizedPosition.y;


        //TODO: Extract normal pos from rects snap to nearest increment
        float xOffset = DayColumnScrollrect.content.anchoredPosition.x;
        float xSize = DayColumnScrollrect.content.GetChild(0).GetComponent<RectTransform>().rect.width;

        float normalSizeX = 1.0f/(DayColumnScrollrect.content.childCount);



        Debug.Log("xOffset  " + xOffset + "   width " + xSize);
        snapPos.x = Mathf.RoundToInt(DayColumnScrollrect.horizontalNormalizedPosition/normalSizeX) * normalSizeX;
        return snapPos;
    }
}
