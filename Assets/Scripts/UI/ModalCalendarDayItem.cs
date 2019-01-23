using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalCalendarDayItem : MonoBehaviour
{
    public Text dayText;

    private Image dayItemImage;
    private Color dayItemImageColor;
    private Button dayItemButton;

    private void Start()
    {
        dayItemImage = GetComponent<Image>();
        dayItemButton = GetComponent<Button>();
        dayItemImageColor = dayItemImage.color;
    }

    public void UpdateDayItem(DateTime dateTime, bool isInteractableDay)
    {
        dayItemImage.color = isInteractableDay ? dayItemImageColor : Color.gray;
        dayItemButton.interactable = isInteractableDay;
        dayText.text = dateTime.Day.ToString();
    }
}
