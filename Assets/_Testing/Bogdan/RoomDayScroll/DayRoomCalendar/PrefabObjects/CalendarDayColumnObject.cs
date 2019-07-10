using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarDayColumnObject : MonoBehaviour
{
    public RectTransform DayRectTransform => dayRectTransform;

    [SerializeField]
    private Button dayButton;
    [SerializeField]
    private RectTransform dayRectTransform;
    [SerializeField]
    private Image backgorundImage;
    [SerializeField]
    private GameObject monthLineImage;

    private DateTime objDate;

    private void OnDestroy()
    {
        dayButton.onClick.RemoveAllListeners();
    }

    public void UpdateEnableDayObject(DateTime date, IRoom room, UnityAction<DateTime,IRoom> tapAction)
    {
        objDate = new DateTime(date.Date.Ticks);

        gameObject.SetActive(true);
        dayButton.onClick.RemoveAllListeners();
        dayButton.onClick.AddListener(() => tapAction(objDate, room));


        if(date.Date == DateTime.Today.Date)
        {
            backgorundImage.color = Placeholder_ThemeManager.Instance.CalendarCurrentColor;
        }
        else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            backgorundImage.color = Placeholder_ThemeManager.Instance.CalendarWeekendColor;
        }
        else
        {
            backgorundImage.color = Placeholder_ThemeManager.Instance.CalendarNormalColor;
        }
    }


    public void UpdateDayObjectDate(DateTime date)
    {
        objDate = new DateTime(date.Date.Ticks);

        if(date.Date == DateTime.Today.Date)
        {
            backgorundImage.color = Placeholder_ThemeManager.Instance.CalendarCurrentColor;
        }
        else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            backgorundImage.color = Placeholder_ThemeManager.Instance.CalendarWeekendColor;
        }
        else
        {
            backgorundImage.color = Placeholder_ThemeManager.Instance.CalendarNormalColor;
        }

        if(date.Day == 1)
        {
            monthLineImage.SetActive(true);
        }
        else
        {
            monthLineImage.SetActive(false);
        }
    }
}
