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
        ThemeManager.Instance.OnThemeChanged += UpdateColors;
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

        UpdateColors(ThemeManager.Instance.IsDarkTheme);
    }


    public void UpdateDayObjectDate(DateTime date)
    {
        objDate = new DateTime(date.Date.Ticks);

        UpdateColors(ThemeManager.Instance.IsDarkTheme);

        if(date.Day == 1)
        {
            monthLineImage.SetActive(true);
        }
        else
        {
            monthLineImage.SetActive(false);
        }
    }

    private void UpdateColors(bool isDark)
    {
        if(objDate.Date == DateTime.Today.Date)
        {
            backgorundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.DarkCurrentColor : ThemeManager.Instance.ThemeColor.LightCurrentColor;//CalendarThemeManager.Instance.CalendarCurrentColor;
        }
        else if (objDate.DayOfWeek == DayOfWeek.Saturday || objDate.DayOfWeek == DayOfWeek.Sunday)
        {
            backgorundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.DarkWeekendColor : ThemeManager.Instance.ThemeColor.LightWeekendColor;//CalendarThemeManager.Instance.CalendarCurrentColor;
        }
        else
        {
            backgorundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.DarkNormalColor : ThemeManager.Instance.ThemeColor.LightNormalColor;
        }
    }
}
