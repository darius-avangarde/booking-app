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

    private List<MenuItem> optionsList = new List<MenuItem>();

    private void Start()
    {
        backgroundButton.onClick.AddListener(() => CloseMenu());
        CloseMenu();
    }

    /// <summary>
    /// function to add a new option to the menu
    /// </summary>
    /// <param name="title">title for the option</param>
    /// <param name="icon">icon for the option</param>
    /// <param name="Callback">callback to set on option click</param>
    /// <param name="height">height of option object(can be left default)</param>
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
        MenuItem menuItem = item.GetComponent<MenuItem>();
        menuItem.Initialize(title, icon, Callback);
        optionsList.Add(menuItem);
    }

    /// <summary>
    /// get a list of all options
    /// </summary>
    /// <returns></returns>
    public List<MenuItem> GetOptions()
    {
        return optionsList;
    }

    /// <summary>
    /// show dropdown menu
    /// </summary>
    public void ShowMenu()
    {
        optionsMenuDropdown.SetActive(true);
        optionsItemMenuTransform.position = Input.mousePosition;
    }

    /// <summary>
    /// close dropdown menu
    /// </summary>
    public void CloseMenu()
    {
        optionsMenuDropdown.SetActive(false);
    }
}
