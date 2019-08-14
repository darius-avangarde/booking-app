using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalCalendar : MonoBehaviour, IClosable
{

    [SerializeField]
    private ModalFadeObject modalFade;

    #region OrientationReferences
        [SerializeField]
        private GameObject portraitHeader;
        [SerializeField]
        private GameObject landscapeHeader;
        [SerializeField]
        private RectTransform portraitReference;
        [SerializeField]
        private RectTransform landscapeReference;
        [SerializeField]
        private RectTransform calendarRect;
    #endregion

    [Space]
    #region UI_Elements
        [SerializeField]
        private TextMeshProUGUI[] dateTexts;
        [SerializeField]
        private TextMeshProUGUI dayText;
        [SerializeField]
        private TextMeshProUGUI monthYearText;
        [SerializeField]
        private TextMeshProUGUI monthYearDayText;

        [SerializeField]
        private TextMeshProUGUI currentMonthYearText;
        [SerializeField]
        private Button confirmButton;
    #endregion


    [Space]
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
    private ModalCalendarPage currentPage;
    [SerializeField]
    private ModalCalendarPage cachePage;

    #region Private variables
        private List<TopCalendarDayObject> dayItems = new List<TopCalendarDayObject>();
        private DateTime focusDateTime = DateTime.Today.Date;
        private DateTime? selectedDateTime = null;
        private UnityAction<DateTime> callback;

        bool isSliding = false;
    #endregion


    // Start is called before the first frame update
    private void Start()
    {
        ResolutionChangeManager.AddListener(UpdateOrientation);

        currentPage.CreateDayItems(UpdateSelectedDate);
        currentPage.UpdatePage(DateTime.Today.Date);
        cachePage.CreateDayItems(UpdateSelectedDate);
        cachePage.UpdatePage(DateTime.Today.Date);
        UpdateOrientation();
        gameObject.SetActive(false);
    }

    public void OpenCallendar(UnityAction<DateTime> doneCallback, DateTime? focusDate = null)
    {
        focusDateTime = focusDate == null ? DateTime.Today.Date : focusDate.Value.Date;
        UpdateCurrentMonthText(focusDateTime);
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => UpdateCurrentMonthText(focusDateTime));
        selectedDateTime = focusDate;
        callback = doneCallback;
        UpdateSelectedDate(focusDateTime, focusDate != null);
        modalFade.FadeIn();
        InputManager.CurrentlyOpenClosable = this;
        currentPage.UpdatePage(focusDateTime);
    }

    public void Close()
    {
        modalFade.FadeOut();
        InputManager.CurrentlyOpenClosable = null;
        selectedDateTime = null;
    }

    public void ConfirmChanges()
    {
        callback?.Invoke(selectedDateTime.Value.Date);
        Close();
    }

    public void ChangeMonth(int monthOffset)
    {
        if(!isSliding)
        {
            focusDateTime = focusDateTime.AddMonths(monthOffset).Date;
            StopAllCoroutines();
            isSliding = false;
            StartCoroutine(Swipe(monthOffset < 0));
        }
    }

    // Update is called once per frame
    private void UpdateOrientation()
    {
        bool isLanscape = Screen.width > Screen.height;

        portraitHeader.SetActive(!isLanscape);
        landscapeHeader.SetActive(isLanscape);

        calendarRect.anchorMin = isLanscape ? landscapeReference.anchorMin : portraitReference.anchorMin;
        calendarRect.anchorMax = isLanscape ? landscapeReference.anchorMax : portraitReference.anchorMax;

        currentPage.Rect.anchoredPosition = Vector2.zero;
        cachePage.Rect.anchoredPosition = Vector2.zero;
    }

    private void UpdateCurrentMonthText(DateTime date)
    {
         //currentMonthYearText.text = $"{Constants.MonthNamesDict[date.Month]} {date.Year}";
         currentMonthYearText.text = $"{LocalizedText.Instance.Months[date.Month-1].TrimEnd()} {date.Year}";
    }

    private void UpdateSelectedDate(DateTime date, bool alsoSetSelected = true)
    {
        if (alsoSetSelected)
            selectedDateTime = date.Date;
        else
            selectedDateTime = null;

        foreach (TextMeshProUGUI t in dateTexts)
            t.text = $"{date.Day}";

       // dayText.text = Constants.DayOfWeekNamesFull[(int)date.DayOfWeek];
       dayText.text = LocalizedText.Instance.DaysLong[(int)date.DayOfWeek];
        monthYearText.text = $"<b>{LocalizedText.Instance.Months[date.Month-1]}</b>\n{date.Year}";
        //monthYearDayText.text = $"{Constants.DayOfWeekNamesFull[(int)date.DayOfWeek]}\n<b>{LocalizedText.Instance.Months[date.Month - 1]}</b>\n{date.Year}";
        monthYearDayText.text = $"{LocalizedText.Instance.DaysLong[(int)date.DayOfWeek]}\n<b>{LocalizedText.Instance.Months[date.Month - 1]}</b>\n{date.Year}";

        confirmButton.interactable = (selectedDateTime != null);

        //move selection to date if not null

        //move visible day if month && year ==

        currentPage.UpdateSelections(selectedDateTime);
    }

    private IEnumerator Swipe(bool isLeft)
    {
        isSliding = true;
        UpdateCurrentMonthText(focusDateTime);

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

        ModalCalendarPage cache = currentPage;
        currentPage = cachePage;
        cachePage = cache;
        isSliding = false;
    }
}
