using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalendarDayColumnObject : MonoBehaviour
{
    public RectTransform DayRectTransform => dayRectTransform;
    public IRoom ObjectRoom => linkedRoomObject.Room;
    public DateTime ObjDate => objDate;

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
    private CalendarRoomColumnObject linkedRoomObject;

    private bool handledThemeManager = false;

    private void HandleThemeManager()
    {
        ThemeManager.Instance.AddItems(separatorImage);
        ThemeManager.Instance.OnThemeChanged += UpdateColors;
        handledThemeManager = true;
    }

    private void OnDestroy()
    {
        dayButton.onClick.RemoveAllListeners();
    }

    public void SetObjectAction(DateTime date, UnityAction<DateTime, IRoom> tapAction, CalendarRoomColumnObject roomObject)
    {
        if(!handledThemeManager)
            HandleThemeManager();

        objDate = date.Date;
        linkedRoomObject = roomObject;
        dayButton.onClick.RemoveAllListeners();
        dayButton.onClick.AddListener(() => tapAction(objDate, ObjectRoom));
        UpdateColors(ThemeManager.Instance.IsDarkTheme);
    }

    public void SetPosition(bool alsoEnable = false)
    {
        if(alsoEnable) gameObject.SetActive(true);
        Vector3 pos = dayRectTransform.anchoredPosition;
        pos.y = linkedRoomObject.RoomRectTransform.anchoredPosition.y - linkedRoomObject.RoomRectTransform.rect.height;
        dayRectTransform.anchoredPosition = pos;
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void UpdateEnableDayObject(DateTime date)
    {
        objDate = new DateTime(date.Date.Ticks);

        gameObject.SetActive(true);
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
