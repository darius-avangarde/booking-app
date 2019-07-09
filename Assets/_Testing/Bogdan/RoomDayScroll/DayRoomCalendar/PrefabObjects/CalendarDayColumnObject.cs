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

    private void OnDestroy()
    {
        dayButton.onClick.RemoveAllListeners();
    }

    public void UpdateEnableDayObject(DateTime date, IRoom room, UnityAction<DateTime,IRoom> tapAction)
    {
        gameObject.SetActive(true);
        dayButton.onClick.RemoveAllListeners();
        dayButton.onClick.AddListener(() => tapAction(date, room));

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
}
