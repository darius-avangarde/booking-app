using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarDayColumnObject : MonoBehaviour
{
    public RectTransform DayRectTransform => dayRectTransform;
    public IRoom ObjectRoom => objRoom;

    [SerializeField]
    private Button dayButton;
    [SerializeField]
    private RectTransform dayRectTransform;
    [SerializeField]
    private Image backgorundImage;
    [SerializeField]
    private Image separatorImage;
    [SerializeField]
    private GameObject monthLineImage;

    private DateTime objDate;
    private IRoom objRoom;

    private void Start()
    {
        ThemeManager.Instance.AddItems(separatorImage);
        //ThemeManager.OnThemeChanged
    }

    private void OnDestroy()
    {
        dayButton.onClick.RemoveAllListeners();
    }

    public void UpdateEnableDayObject(DateTime date, IRoom room, UnityAction<DateTime,IRoom> tapAction)
    {
        objDate = new DateTime(date.Date.Ticks);
        objRoom = room;

        gameObject.SetActive(true);
        dayButton.onClick.RemoveAllListeners();
        dayButton.onClick.AddListener(() => tapAction(objDate, room));

        UpdateColors();
    }


    public void UpdateDayObjectDate(DateTime date)
    {
        objDate = new DateTime(date.Date.Ticks);

        UpdateColors();

        if(date.Day == 1)
        {
            monthLineImage.SetActive(true);
        }
        else
        {
            monthLineImage.SetActive(false);
        }
    }

    private void UpdateColors()
    {
        if(objDate.Date == DateTime.Today.Date)
        {
            backgorundImage.color = CalendarThemeManager.Instance.CalendarCurrentColor;
        }
        else if (objDate.DayOfWeek == DayOfWeek.Saturday || objDate.DayOfWeek == DayOfWeek.Sunday)
        {
            backgorundImage.color = CalendarThemeManager.Instance.CalendarWeekendColor;
        }
        else
        {
            backgorundImage.color = CalendarThemeManager.Instance.CalendarNormalColor;
        }
    }
}
