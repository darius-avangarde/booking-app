using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ModalCalendarNew : MonoBehaviour, IClosable
{
    [SerializeField]
    private EasyTween easyTween = null;
    [SerializeField]
    private Text monthName = null;
    [SerializeField]
    private Text selectionText = null;
    [SerializeField]
    private Text selectionDayCountText = null;
    [SerializeField]
    private Button confirmButton = null;
    [SerializeField]
    private float slideDuration = 0.1f;
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
    private ModalMonthPage currentPage;
    [SerializeField]
    private ModalMonthPage cachePage;

    #region Colors
    [Space]
    [SerializeField]
    private Color availableColor;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Color unavailableColor;
    [SerializeField]
    private Color currentColor;
    [SerializeField]
    private Color pastColor;
    [SerializeField]
    private Color notMonthColor;
    #endregion

    #region Public readonly properties
    public List<IReservation> RoomReservationList => roomReservationList;
    public IReservation CurrentReservation => currentReservation;
    public DateTime SelectedStart => selectedStart;
    public DateTime SelectedEnd => selectedEnd;
    public bool ShowSelection => showSelection;

    public Color AvailableColor => availableColor;
    public Color SelectedColor => selectedColor;
    public Color UnavailableColor => unavailableColor;
    public Color CurrentColor => currentColor;
    public Color PastColor => pastColor;
    public Color NotMonthColor => notMonthColor;
    #endregion

    private Action<DateTime, DateTime> DoneCallback;
    private DateTime focusDateTime = DateTime.Today.Date;
    private IReservation currentReservation;
    private List<IReservation> roomReservationList = new List<IReservation>();
    private bool isSelectingEnd = false;
    private bool showSelection = false;
    private bool allowSigleDate = false;
    private bool isSliding = false;
    private DateTime selectedStart;
    private DateTime selectedEnd;


    private void Start()
    {
        currentPage.CreateDayItems();
        currentPage.UpdatePage(DateTime.Today.Date);
        cachePage.CreateDayItems();
        cachePage.UpdatePage(DateTime.Today.Date);
    }

    #region OpenCalendarFunctions
        ///<summary>
        ///Opens the calendar in selection mode focused on the given datetime.
        ///<para>Done callback returns either the selected datetime and the day after if only one date is selected, or the selected start and end date</para>
        ///</summary>
        internal void OpenCallendar(DateTime focusedDay, Action<DateTime, DateTime> doneCallback)
        {
            focusDateTime = focusedDay;
            DoneCallback = doneCallback;
            allowSigleDate = true;
            selectionDayCountText.text = string.Empty;
            Show(focusDateTime, null, null, doneCallback);
        }

        ///<summary>
        ///Opens the calendar in reservation edit mode for the given IReservation, and room reservations list.
        ///<para>Done callback returns the selected start and end date</para>
        ///</summary>
        internal void OpenCallendar(IReservation r, List<IReservation> roomReservations, Action<DateTime, DateTime> doneCallback)
        {
            focusDateTime = (r != null) ? r.Period.Start : DateTime.Today;
            allowSigleDate = false;
            UpdateDayCountText();
            Show(focusDateTime, r, roomReservations, doneCallback);
        }
    #endregion

    public void ShowPreviousMonth()
    {
        if (focusDateTime.Date > DateTime.Today.Date)
        {
            focusDateTime = focusDateTime.AddMonths(-1);
            StopAllCoroutines();
            isSliding = false;
            StartCoroutine(Swipe(true));
        }
    }

    public void ShowNextMonth()
    {
        focusDateTime = focusDateTime.AddMonths(1);
        StopAllCoroutines();
        isSliding = false;
        StartCoroutine(Swipe(false));
    }

    public void Close()
    {
        CloseModalCalendar(false);
    }

    public void Confirm()
    {
        CloseModalCalendar(true);
    }

    public int GetDaysVisibleFromPreviousMonth(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 0;
            case DayOfWeek.Tuesday: return 1;
            case DayOfWeek.Wednesday: return 2;
            case DayOfWeek.Thursday: return 3;
            case DayOfWeek.Friday: return 4;
            case DayOfWeek.Saturday: return 5;
            case DayOfWeek.Sunday: return 6;
        }

        return 0;
    }

    public void HandleClickAction(ModalDayObject dayObj)
    {
        if(!isSliding)
        {
            if(!dayObj.IsReserved && !dayObj.IsStart && !isSelectingEnd)
            {
                selectedStart = dayObj.ObjDate.Date;
                selectedEnd = dayObj.ObjDate.Date;
                isSelectingEnd = true;
                showSelection = true;
                currentPage.UpdatePage(focusDateTime);
                selectionText.text = selectedStart.ToString(Constants.DateTimePrintFormat);
                confirmButton.interactable = allowSigleDate;
            }
            else if(!dayObj.IsReserved && isSelectingEnd)
            {
                selectedEnd = dayObj.ObjDate.Date;
                if(selectedStart.Date > selectedEnd.Date)
                {
                    selectedEnd = selectedStart;
                    selectedStart = dayObj.ObjDate;
                }

                if(!OverlapsOtherReservation(selectedStart, selectedEnd) && selectedStart != selectedEnd)
                {
                    showSelection = true;
                    selectionText.text = selectedStart.ToString(Constants.DateTimePrintFormat) + Constants.AndDelimiter + selectedEnd.ToString(Constants.DateTimePrintFormat);
                    confirmButton.interactable = true;
                }
                else
                {
                    selectedEnd = default;
                    selectedStart = default;
                    selectionText.text = string.Empty;
                    confirmButton.interactable = false;
                }
                currentPage.UpdatePage(focusDateTime);
                isSelectingEnd = false;
            }
            else
            {
                showSelection = false;
                isSelectingEnd = false;
                currentPage.UpdatePage(focusDateTime);
                selectionText.text = string.Empty;
                confirmButton.interactable = false;
            }
            UpdateDayCountText();
        }
    }

    private void Show(DateTime initialDateTime, IReservation reservation, List<IReservation> reservationList, Action<DateTime, DateTime> doneCallback)
    {
        monthName.text = Constants.MonthNamesDict[focusDateTime.Month] + " " + focusDateTime.Year;
        focusDateTime = initialDateTime;
        currentReservation = reservation;
        roomReservationList = reservationList;
        easyTween.OpenCloseObjectAnimation();
        InputManager.CurrentlyOpenClosable = this;
        DoneCallback = doneCallback;

        if(currentReservation != null)
        {
            selectionText.text = currentReservation.Period.Start.ToString(Constants.DateTimePrintFormat) + Constants.AndDelimiter + currentReservation.Period.End.ToString(Constants.DateTimePrintFormat);
            confirmButton.interactable = true;
        }
        else
        {
            selectionText.text = string.Empty;
            confirmButton.interactable = false;
        }

        currentPage.UpdatePage(focusDateTime);
    }

    private void CloseModalCalendar(bool doCallback)
    {
        if(selectedStart == default)
        {
            selectedStart = DateTime.Today.Date;
        }

        if(selectedEnd.Date == selectedStart.Date || selectedEnd == default)
        {
            selectedEnd = selectedStart.AddDays(1);
        }

        if(doCallback)
        {
            DoneCallback?.Invoke(selectedStart.Date, selectedEnd.Date);
        }
        DoneCallback = null;
        easyTween.OpenCloseObjectAnimation();
        InputManager.CurrentlyOpenClosable = null;
        showSelection = false;
        isSelectingEnd = false;
        allowSigleDate = false;
        currentReservation = null;
        roomReservationList = new List<IReservation>();
    }

    private bool OverlapsOtherReservation(DateTime start, DateTime end)
    {
        if(roomReservationList == null || roomReservationList.Count == 0)
        {
            return false;
        }

        return roomReservationList
            .Any(r => ((currentReservation != null) ? r.ID != currentReservation.ID : r.ID != Constants.defaultCustomerName)
            && ((start.Date > r.Period.Start && start.Date < r.Period.End.Date) //after start and before end
            || r.Period.Start.Date > start.Date && r.Period.End.Date < end.Date
            || r.Period.Start.Date == selectedStart.Date || r.Period.End.Date == selectedEnd.Date
            ));
    }

    private void UpdateDayCountText()
    {
        int days = (int)(selectedEnd - selectedStart).TotalDays;
        selectionDayCountText.text = String.Format("{0} {1} {2}", Constants.DAY_COUNT_PREF, days, (days == 1) ? Constants.DAY_COUNT_SUFF_SN : Constants.DAY_COUNT_SUFF_PL);
    }

    private IEnumerator Swipe(bool isLeft)
    {
        isSliding = true;
        monthName.text = Constants.MonthNamesDict[focusDateTime.Month] + " " + focusDateTime.Year;

        cachePage.Rect.position = (isLeft) ? slideLeft.position : slideRight.position;
        cachePage.UpdatePage(focusDateTime);

        Vector3 startCache = cachePage.Rect.position;
        Vector3 endCache = slideCenter.position;

        Vector3 startCurrent = currentPage.Rect.position;
        Vector3 endCurrent = (!isLeft) ? slideLeft.position : slideRight.position;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/slideDuration)
        {
            cachePage.Rect.position = Vector3.Lerp(startCache, endCache, slideCurve.Evaluate(t));
            currentPage.Rect.position = Vector3.Lerp(startCurrent, endCurrent, slideCurve.Evaluate(t));
            yield return null;
        }

        cachePage.Rect.position = endCache;
        currentPage.Rect.position = endCurrent;

        ModalMonthPage cache = currentPage;
        currentPage = cachePage;
        cachePage = cache;
        isSliding = false;
    }
}
