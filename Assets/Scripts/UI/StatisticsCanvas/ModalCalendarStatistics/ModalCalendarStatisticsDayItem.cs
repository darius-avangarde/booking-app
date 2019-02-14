using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalCalendarStatisticsDayItem : MonoBehaviour
{
    [SerializeField]
    private Text dayText = null;
    private Button modalItemButton;
    private Image modalItemImage;
    private Color modalItemImageColor;
    private DateTime itemDateTime;

    public void Initialize(Action<DateTime, Image> callback)
    {
        modalItemButton = GetComponent<Button>();
        modalItemImage = GetComponent<Image>();
        modalItemImageColor = modalItemImage.color;
        GetComponent<Button>().onClick.AddListener(() =>callback(itemDateTime, modalItemImage));
    }

    public void UpdateDayItem(DateTime dateTime)
    {
        modalItemImage.color = modalItemImageColor;
        itemDateTime = dateTime; 
        dayText.text = dateTime.Day.ToString();
    }
}
