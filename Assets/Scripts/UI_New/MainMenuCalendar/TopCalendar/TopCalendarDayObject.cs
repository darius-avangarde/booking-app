using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TopCalendarDayObject : MonoBehaviour
{
    [SerializeField]
    private Text dateText;
    [SerializeField]
    private Button dateButton;
    [SerializeField]
    private Image dateImage;

    private DateTime objDate;

    private void Start()
    {
        ThemeManager.Instance.AddItems(dateText);
    }

    private void OnDestroy()
    {
        dateButton.onClick.RemoveAllListeners();
    }

    public void UpdateDayObject(DateTime date, UnityAction<DateTime> tapAction = null)
    {
        objDate = date.Date;
        dateText.text = $"{date.Day}";
        dateImage.color =  (objDate == DateTime.Today.Date) ? ThemeManager.Instance.ThemeColor.LightHeadCurrentColor : Color.clear;
        dateButton.onClick.RemoveAllListeners();
        dateButton.onClick.AddListener(() => tapAction(objDate));
    }

    public void UpdateDayObject()
    {
        dateButton.onClick.RemoveAllListeners();
        dateText.text = string.Empty;
    }
}
