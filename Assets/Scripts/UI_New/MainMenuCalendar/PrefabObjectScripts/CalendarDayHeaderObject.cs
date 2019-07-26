using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarDayHeaderObject : MonoBehaviour
{
    public DateTime ObjectDate => objectDate;

    [SerializeField]
    private Image dayBackgroundImage;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Button dayButton;
    [SerializeField]
    private Text dayOfWeekText;
    [SerializeField]
    private Text dateText;

    private DateTime objectDate;
    private IProperty objProperty;

    private void Start()
    {
        ThemeManager.Instance.AddItems(dayOfWeekText, dateText);
        ThemeManager.Instance.OnThemeChanged.AddListener(UpdateColors);
    }

    private void OnDestroy()
    {
        dayButton.onClick.RemoveAllListeners();
    }

    public void UpdateDayObject(DateTime date, IProperty property, UnityAction<DateTime,IProperty> tapAction)
    {
        objectDate = new DateTime(date.Date.Ticks);
        objProperty = property;
        dayButton.onClick.RemoveAllListeners();
        dayButton.onClick.AddListener(() => tapAction(objectDate, objProperty));
        UpdateUI(objectDate);
    }

    public void OnScrollReposition(int dayOffset)
    {
        UpdateUI(objectDate.AddDays(dayOffset));
    }

    public void UpdateUI(DateTime date)
    {
        if(objectDate.Date != date.Date)
            objectDate = new DateTime(date.Date.Ticks);

        dayOfWeekText.text = Constants.DayOfWeekNamesShort[GetDayOfWeekIndex(date.DayOfWeek, out bool isWeekend)];
        dateText.text = date.Day.ToString();

        //update colors with theme bool from stuff
        //UpdateColors(ThemeManager.Instance.IsDarkTheme)
    }

    public void UpdateProperty(IProperty property)
    {
        objProperty = property;
    }

    private void UpdateColors(bool isDark)
    {
        GetDayOfWeekIndex(objectDate.DayOfWeek, out bool isWeekend);

        if(objectDate.Date == DateTime.Today.Date)
        {
            // dayBackgroundImage.color = CalendarThemeManager.Instance.CalendarHeadCurrentColor;
            // backgroundImage.color = CalendarThemeManager.Instance.CalendarCurrentColor;
        }
        else if(isWeekend)
        {
            // dayBackgroundImage.color = CalendarThemeManager.Instance.CalendarHeadWeekendColor;
            // backgroundImage.color = CalendarThemeManager.Instance.CalendarWeekendColor;
        }
        else
        {
            // dayBackgroundImage.color = CalendarThemeManager.Instance.CalendarHeadNormalColor;
            // backgroundImage.color = CalendarThemeManager.Instance.CalendarNormalColor;
        }
    }

    private int GetDayOfWeekIndex(DayOfWeek day, out bool isWeekend)
    {
        isWeekend = false;

        switch (day)
        {
            case DayOfWeek.Monday:      return 0;
            case DayOfWeek.Tuesday:     return 1;
            case DayOfWeek.Wednesday:   return 2;
            case DayOfWeek.Thursday:    return 3;
            case DayOfWeek.Friday:      return 4;
            case DayOfWeek.Saturday:    isWeekend = true;    return 5;
            case DayOfWeek.Sunday:      isWeekend = true;    return 6;
        }

        return 0;
    }
}
