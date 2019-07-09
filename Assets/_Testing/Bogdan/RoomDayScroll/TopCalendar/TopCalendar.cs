using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopCalendar : MonoBehaviour
{
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

    [SerializeField]
    private TopCalendarMonthPage currentPage;
    [SerializeField]
    private TopCalendarMonthPage cachePage;

    [Space]
    [SerializeField]
    private RectTransform topcalendarRectTransform;
    [SerializeField]
    private RectTransform lowerCalendarRectTransform;

    private List<TopCalendarDayObject> dayItems = new List<TopCalendarDayObject>();
    private DateTime focusDateTime = DateTime.Today.Date;

    bool isSliding = false;
    bool isOpening = false;
    bool isOpen = false;


    private void Start()
    {
        //set to closed pos
        lowerCalendarRectTransform.offsetMax = topcalendarRectTransform.offsetMax;
        currentPage.CreateDayItems();
        currentPage.UpdatePage(DateTime.Today.Date);
        cachePage.CreateDayItems();
        cachePage.UpdatePage(DateTime.Today.Date);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeMonth(-1);
        if(Input.GetKeyDown(KeyCode.RightArrow))
            ChangeMonth(+1);
    }

    public void OpenCloseDropdownCalendar()
    {
        if(!isOpening)
        {
            StartCoroutine(Slide());
        }
    }

    private IEnumerator Slide()
    {
        isOpening = true;

        Vector2 fromOffset = lowerCalendarRectTransform.offsetMax;
        Vector2 toOffset = fromOffset;
        toOffset.y = (!isOpen) ? topcalendarRectTransform.rect.yMin + topcalendarRectTransform.offsetMax.y : topcalendarRectTransform.offsetMax.y;

        for (float t = 0; t < 1.0f; t += Time.deltaTime/slideTime)
        {
            lowerCalendarRectTransform.offsetMax = Vector3.Lerp(fromOffset, toOffset, slideCurve.Evaluate(t));
            yield return null;
        }

        lowerCalendarRectTransform.offsetMax = toOffset;

        isOpening = false;
        isOpen = !isOpen;
    }

    public void ChangeMonth(int monthOffset)
    {
        focusDateTime = focusDateTime.AddMonths(monthOffset).Date;
        StopAllCoroutines();
        isSliding = false;
        StartCoroutine(Swipe(monthOffset < 0));
    }

    private void Show(DateTime focus)
    {
        focusDateTime = focus;
        monthText.text = $"{Constants.MonthNamesDict[focusDateTime.Month]} {focusDateTime.Year}";
        currentPage.UpdatePage(focusDateTime);
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


    // create day items

    //use logic from old new calendar

    //implement slide mechanic

    //change month void +/-

    //tap action updates room calendar
}
