using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NotificationsItemMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject itemTemplate = null;
    [SerializeField]
    private GameObject menuBlocker = null;
    [SerializeField]
    private GameObject scrollView = null;
    [SerializeField]
    private Transform scrollContent = null;

    private Transform menuItemTransform;
    private GameObject blocker;
    private Toggle toggleButton;

    private void Awake()
    {
        toggleButton = GetComponent<Toggle>();
        toggleButton.onValueChanged.AddListener(b => ActivateMenu(b));
        menuItemTransform = GetComponent<Transform>();
    }

    public void AddOption(string title, Sprite icon, Action Callback, float height = -1)
    {
        GameObject item = Instantiate(itemTemplate, scrollContent);
        item.SetActive(true);
        if (height != -1)
        {
            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.sizeDelta = new Vector2(itemRT.sizeDelta.x, height);
        }
        item.GetComponent<MenuItem>().Initialize(title, icon, Callback);
    }

    private void ActivateMenu(bool active)
    {
        scrollView.SetActive(active);
        if (active)
        {
            blocker = Instantiate(menuBlocker, menuItemTransform.parent);
            RectTransform blockerTransform = blocker.GetComponent<RectTransform>();
            blockerTransform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            blockerTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            blocker.GetComponent<Button>().onClick.AddListener(() => CloseMenu());
            menuItemTransform.SetAsLastSibling();
        }
        else
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        scrollView.SetActive(false);
        toggleButton.isOn = false;
        Destroy(blocker);
    }
}
