using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private PropertyAdminScreen propertyAdminScreen = null;
    [SerializeField]
    private PropertyRoomScreen propertyRoomScreen = null;
    [SerializeField]
    private RoomScreen roomScreen = null;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private RectTransform propertyInfoContent = null;
    [SerializeField]
    private ScrollRect propertiesScrollView = null;
    [SerializeField]
    private GameObject propertyItemPrefab = null;
    [SerializeField]
    private Button thumbnailsViewButton = null;
    [SerializeField]
    private Button listViewButton = null;
    [SerializeField]
    private Button addPropertyButton = null;
    [SerializeField]
    private Button backButton = null;
    private Canvas canvasComponent;

    private List<GameObject> propertyButtonList = new List<GameObject>();
    private float tempPosition = 1;
    private bool thumbnails = false;
    private bool expanding = false;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        addPropertyButton.onClick.AddListener(() => AddPropertyItem());
    }

    public void Initialize()
    {
        expanding = false;
        foreach (var propertyButton in propertyButtonList)
        {
            DestroyImmediate(propertyButton);
        }
        propertyButtonList = new List<GameObject>();
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            propertyButton = Instantiate(propertyItemPrefab, propertyInfoContent);
            propertyButton.GetComponent<PropertyButton>().Initialize(property, OpenRoomScreen, OpenPropertyRoomScreen, null);
            propertyButtonList.Add(propertyButton);
        }
        propertiesScrollView.verticalNormalizedPosition = tempPosition;
        ExpandThumbnails(thumbnails);
    }

    public void ExpandThumbnails(bool expand)
    {
        StopAllCoroutines();
        scrollRectComponent.ResetAll();
        tempPosition = propertiesScrollView.verticalNormalizedPosition;
        foreach (GameObject property in propertyButtonList)
        {
            property.SetActive(true);
            StartCoroutine(ExpandView(expand, property.GetComponent<RectTransform>()));
        }
        StartCoroutine(WaitForInit());
    }

    public void LastPosition()
    {
        tempPosition = propertiesScrollView.verticalNormalizedPosition;
    }

    private void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        OpenPropertyAdminScreen(property);
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreen.GetComponent<NavScreen>());
    }

    private void OpenPropertyRoomScreen(IProperty property)
    {
        propertyRoomScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyRoomScreen.GetComponent<NavScreen>());
    }

    private void OpenRoomScreen(IRoom room)
    {
        roomScreen.UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }

    public IEnumerator ExpandView(bool expand, RectTransform property)
    {
        expanding = true;
        thumbnails = expand;
        if (expand)
        {
            thumbnailsViewButton.gameObject.SetActive(false);
            listViewButton.gameObject.SetActive(true);
            Vector2 endSize = new Vector2(property.sizeDelta.x, 750f);
            float currentTime = 0;
            while (currentTime < 0.5f)
            {
                currentTime += Time.deltaTime;
                property.sizeDelta = Vector2.Lerp(property.sizeDelta, endSize, currentTime / 0.5f);
                LayoutRebuilder.ForceRebuildLayoutImmediate(propertyInfoContent);
                Canvas.ForceUpdateCanvases();
                propertiesScrollView.verticalNormalizedPosition = tempPosition;
                yield return null;
            }
            property.sizeDelta = endSize;
        }
        else
        {
            listViewButton.gameObject.SetActive(false);
            thumbnailsViewButton.gameObject.SetActive(true);
            Vector2 endSize = new Vector2(property.sizeDelta.x, 285f);
            float currentTime = 0;
            while (currentTime < 0.5f)
            {
                currentTime += Time.deltaTime;
                property.sizeDelta = Vector2.Lerp(property.sizeDelta, endSize, currentTime / 0.5f);
                LayoutRebuilder.ForceRebuildLayoutImmediate(propertyInfoContent);
                Canvas.ForceUpdateCanvases();
                propertiesScrollView.verticalNormalizedPosition = tempPosition;
                yield return null;
            }
            property.sizeDelta = endSize;
        }
        expanding = false;
    }

    public IEnumerator WaitForInit()
    {
        while (expanding)
        {
            yield return null;
        }
        if (propertiesScrollView.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
    }
}
