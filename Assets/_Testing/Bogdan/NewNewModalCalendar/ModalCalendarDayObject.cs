using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalCalendarDayObject : MonoBehaviour
{
    [SerializeField]
    private Text dateText;
    [SerializeField]
    private Button dateButton;

    private DateTime objDate;

    private void OnDestroy()
    {
        dateButton.onClick.RemoveAllListeners();
    }

    public void UpdateDayObject(DateTime date, UnityAction<DateTime,bool> tapAction = null)
    {
        objDate = date.Date;
        dateText.text = $"{date.Day}";
        dateButton.targetGraphic.color =  (objDate == DateTime.Today.Date) ? Placeholder_ThemeManager.Instance.CalendarCurrentColor : Color.clear;
        dateButton.onClick.RemoveAllListeners();
        dateButton.onClick.AddListener(() => tapAction(objDate, true));
    }

    public void UpdateDayObject()
    {
        dateButton.onClick.RemoveAllListeners();
        dateText.text = string.Empty;
    }
}
