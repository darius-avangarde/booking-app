using UnityEngine;
using UnityEngine.Events;

public class SwipeHandler : MonoBehaviour
{

    [Header("Registers swipes when this gameobject is active in hierarchy")]
    [SerializeField]
    private GameObject targetUiObject;
    [Space]
    [SerializeField]
    private UnityEvent onSwipeLeft;
    [SerializeField]
    private UnityEvent onSwipeRight;


    private float dragDistance;
    private Vector2 firstPos;
    private Vector2 lastPos;

    private void Start()
    {
        dragDistance = Screen.height * 0.15f;
    }

    private void Update()
    {
        if(targetUiObject.activeInHierarchy == true)
        {
            if(Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

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
                                onSwipeRight.Invoke();
                        }
                        // swiped left
                        else
                        {
                            if(onSwipeLeft != null)
                                onSwipeLeft.Invoke();
                        }
                    }
                }
            }
        }
    }

}
