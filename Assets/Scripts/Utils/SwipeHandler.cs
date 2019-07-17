using UnityEngine;
using UnityEngine.Events;

public class SwipeHandler : MonoBehaviour
{
    public bool Enabled
    {
        get => TargetGameObject.activeSelf;
        set => TargetGameObject.SetActive(value);
    }

    [Header("Registers swipes over this gameobject when active")]
    [SerializeField]
    private RectTransform targetRectTransform;
    [Space]
    [SerializeField]
    private UnityEvent onSwipeLeft;
    [SerializeField]
    private UnityEvent onSwipeRight;


    private GameObject TargetGameObject
    {
        get
        {
            if(targetGameObject == null)
            {
                targetGameObject = gameObject;
            }
            return targetGameObject;
        }
    }

    private GameObject targetGameObject;
    private float dragDistance;
    private Vector2 firstPos;
    private Vector2 lastPos;


    private void Start()
    {
        dragDistance = Mathf.Max(Screen.height, Screen.width) * 0.15f;
    }

    private void Update()
    {
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if(TargetGameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, touch.position))
            {

                if(touch.phase == TouchPhase.Began)
                {
                    firstPos = touch.position;
                    lastPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    lastPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    lastPos = touch.position;

                    if(Mathf.Abs(lastPos.x - firstPos.x) > dragDistance)
                    {
                        //swiped right
                        if(lastPos.x > firstPos.x)
                        {
                            if(onSwipeRight != null)
                                onSwipeRight?.Invoke();
                        }
                        // swiped left
                        else
                        {
                            if(onSwipeLeft != null)
                                onSwipeLeft?.Invoke();
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        if(TargetGameObject.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
                onSwipeLeft?.Invoke();

            if(Input.GetKeyDown(KeyCode.RightArrow))
                onSwipeRight?.Invoke();
        }
#endif
    }
}
