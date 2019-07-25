using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuDropdown : MonoBehaviour
{
    [SerializeField]
    private GameObject itemTemplate = null;
    [SerializeField]
    private GameObject optionsMenuDropdown = null;
    [SerializeField]
    private RectTransform optionsItemMenuTransform = null;
    [SerializeField]
    private Transform itemsList = null;
    [SerializeField]
    private Button backgroundButton = null;

    private void Start()
    {
        backgroundButton.onClick.AddListener(() => CloseMenu());
        CloseMenu();
    }

    public void AddOption(string title, Sprite icon, Action Callback, float height = -1)
    {
        Callback += CloseMenu;
        GameObject item = Instantiate(itemTemplate, itemsList);
        item.SetActive(true);
        if (height != -1)
        {
            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.sizeDelta = new Vector2(itemRT.sizeDelta.x, height);
        }
        item.GetComponent<MenuItem>().Initialize(title, icon, Callback);
    }

    public void ShowMenu()
    {
        optionsMenuDropdown.SetActive(true);
        optionsItemMenuTransform.position = Input.mousePosition;
    }

    public void CloseMenu()
    {
        optionsMenuDropdown.SetActive(false);
    }
}
