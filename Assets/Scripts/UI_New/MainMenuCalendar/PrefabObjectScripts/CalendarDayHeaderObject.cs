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
        ThemeManager.Instance.OnThemeChanged += UpdateColors;
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

        UpdateColors(ThemeManager.Instance.IsDarkTheme);
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
            dayBackgroundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.LightHeadCurrentColor : ThemeManager.Instance.ThemeColor.DarkHeadCurrentColor;
            backgroundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.LightCurrentColor : ThemeManager.Instance.ThemeColor.DarkCurrentColor;
        }
        else if(isWeekend)
        {
            dayBackgroundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.LightHeadWeekendColor : ThemeManager.Instance.ThemeColor.DarkHeadWeekendColor;
            backgroundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.LightWeekendColor : ThemeManager.Instance.ThemeColor.DarkWeekendColor;
        }
        else
        {
            dayBackgroundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.LightHeadNormalColor : ThemeManager.Instance.ThemeColor.DarkHeadNormalColor;
            backgroundImage.color = (isDark) ? ThemeManager.Instance.ThemeColor.LightNormalColor : ThemeManager.Instance.ThemeColor.DarkNormalColor;
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
