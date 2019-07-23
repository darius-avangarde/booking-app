using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    [SerializeField]
    private Text itemLabel = null;
    [SerializeField]
    private Image itemImage = null;
    [SerializeField]
    private Button itemButton;

    public void Initialize(string label, Sprite icon, Action Callback)
    {
        itemLabel.text = label;
        if (itemImage != null)
        {
            if (icon != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = icon;
            }
            else
            {
                itemImage.gameObject.SetActive(false);
            }
        }
        if (Callback != null)
        {
            itemButton.onClick.AddListener(() => Callback());
        }
    }
}
