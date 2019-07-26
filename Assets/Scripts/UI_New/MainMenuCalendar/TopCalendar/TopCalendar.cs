using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopCalendar : MonoBehaviour
{
    [SerializeField]
    private ScrollviewHandler scrollviewHandler;
    [SerializeField]
    private SwipeHandler swipeHandler;

    [SerializeField]
    private Text monthText;

    [Header("Animation")]
    [SerializeField]
    private float slideTime = 0.35f;
    [SerializeField]
    private AnimationCurve slideCurve;

    #region Slide Refrence Rects
        [Space]
        [SerializeField]
        private RectTransform slideCenter;
        [SerializeField]
        private RectTransform slideLeft;
        [SerializeField]
        private RectTransform slideRight;
    #endregion

    [Space]
    [SerializeField]
    private TopCalendarMonthPage currentPage;
    [SerializeField]
    private TopCalendarMonthPage cachePage;

    [Space]
    [SerializeField]
    private RectTransform topcalendarRectTransform;
    [SerializeField]
    private RectTransform lowerCalendarRectTransform;
    [SerializeField]
    private RectTransform arrow;

    #region Private variables
        private List<TopCalendarDayObject> dayItems = new List<TopCalendarDayObject>();
        private DateTime focusDateTime = DateTime.Today.Date;

        bool isSliding = false;
        bool isOpening = false;
        bool isOpen = false;
    #endregion


    private void Start()
    {
        //set to closed pos
        lowerCalendarRectTransform.offsetMax = topcalendarRectTransform.offsetMax;
        currentPage.CreateDayItems();
        currentPage.UpdatePage(DateTime.Today.Date);
        cachePage.CreateDayItems();
        cachePage.UpdatePage(DateTime.Today.Date);
        swipeHandler.Enabled = false;
    }

    public void ToggleDropdownCalendar()
    {
        if(!isOpening && !isSliding)
        {
            StartCoroutine(Slide(!isOpen));
        }
    }

    public void CloseDropdownCalendar()
    {
        if(!isOpening && !isSliding && isOpen)
        {
            StartCoroutine(Slide(false));
        }
    }

    public void UpdateMonth(DateTime date)
    {
        //only update if the year or motnth is different
        if(focusDateTime.Year != date.Year || focusDateTime.Month != date.Month)
        {
            //check if the span is greater than one day (avoid flicker when focal day column changes rapidly)
            if(Mathf.Abs((int)date.Date.Subtract(focusDateTime.Date).TotalDays) > 1)
            {
                focusDateTime = date.Date;
                isSliding = false;
                currentPage.UpdatePage(focusDateTime);
                monthText.text = $"{Constants.MonthNamesDict[focusDateTime.Month]} {focusDateTime.Year}";
            }
        }
    }

    public void ChangeMonth(int monthOffset)
    {
        focusDateTime = focusDateTime.AddMonths(monthOffset).Date;
        //StopAllCoroutines();
        isSliding = false;
        StartCoroutine(Swipe(monthOffset < 0));
    }

    private IEnumerator Slide(bool open)
    {
        isOpening = true;

        float fromRotationZ = arrow.rotation.eulerAngles.z;
        float toRotationZ = (open) ? -180 : 0;

        Vector2 fromOffset = lowerCalendarRectTransform.offsetMax;
        Vector2 toOffset = fromOffset;
        toOffset.y = (open) ? topcalendarRectTransform.rect.yMin + topcalendarRectTransform.offsetMax.y : topcalendarRectTransform.offsetMax.y;

        for (float t = 0; t < 1.0f; t += Time.deltaTime/slideTime)
        {
            arrow.rotation = Quaternion.Euler(Mathf.Lerp(fromRotationZ, toRotationZ, t*2), 0, 0);
            lowerCalendarRectTransform.offsetMax = Vector3.Lerp(fromOffset, toOffset, slideCurve.Evaluate(t));
            yield return null;
        }

        arrow.rotation = Quaternion.Euler(toRotationZ, 0, 0);
        lowerCalendarRectTransform.offsetMax = toOffset;

        isOpening = false;
        isOpen = open;
        swipeHandler.Enabled = isOpen;
    }

    private IEnumerator Swipe(bool isLeft)
    {
        isSliding = true;
        monthText.text = $"{Constants.MonthNamesDict[focusDateTime.Month]} {focusDateTime.Year}";

        cachePage.Rect.position = (isLeft) ? slideLeft.position : slideRight.position;
        cachePage.UpdatePage(focusDateTime);

        Vector3 startCache = cachePage.Rect.position;
        Vector3 endCache = slideCenter.position;

        Vector3 startCurrent = currentPage.Rect.position;
        Vector3 endCurrent = (!isLeft) ? slideLeft.position : slideRight.position;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/slideTime)
        {
            scrollviewHandler.ForceMatchVerticalPosition();
            cachePage.Rect.position = Vector3.Lerp(startCache, endCache, slideCurve.Evaluate(t));
            currentPage.Rect.position = Vector3.Lerp(startCurrent, endCurrent, slideCurve.Evaluate(t));
            yield return null;
        }

        cachePage.Rect.position = endCache;
        currentPage.Rect.position = endCurrent;

        TopCalendarMonthPage cache = currentPage;
        currentPage = cachePage;
        cachePage = cache;
        isSliding = false;
    }
}
