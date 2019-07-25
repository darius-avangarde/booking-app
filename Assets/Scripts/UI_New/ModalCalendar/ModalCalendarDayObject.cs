using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalCalendarDayObject : MonoBehaviour
{
    public DateTime ObjDate => objDate;

    [SerializeField]
    private Text dateText;
    [SerializeField]
    private Button dateButton;
    [SerializeField]
    private Image dateButtonImage;

    private DateTime objDate;

    private void OnDestroy()
    {
        dateButton.onClick.RemoveAllListeners();
    }

    public void UpdateDayObject(DateTime date, UnityAction<DateTime,bool> tapAction = null)
    {
        objDate = date.Date;
        dateText.text = $"{date.Day}";
        dateButton.onClick.RemoveAllListeners();
        dateButton.onClick.AddListener(() => tapAction(objDate, true));
    }

    public void UpdateSpriteAndColor(Sprite sprite, Color color)
    {
        dateButtonImage.sprite = sprite;
        dateButtonImage.color  = color;
    }

    public void UpdateDayObject()
    {
        dateButton.onClick.RemoveAllListeners();
        dateText.text = string.Empty;
    }
}
