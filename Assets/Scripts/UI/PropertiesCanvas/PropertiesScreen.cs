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
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private Transform propertyRoomScreenTransform = null;
    [SerializeField]
    private Transform roomScreenTransform = null;
    [SerializeField]
    private GameObject propertyItemPrefab = null;
    [SerializeField]
    private RectTransform propertyInfoContent = null;
    [SerializeField]
    private Button thumbnailsViewButton = null;
    [SerializeField]
    private Button listViewButton = null;
    [SerializeField]
    private Button addPropertyButton = null;
    [SerializeField]
    private Button backButton = null;

    private List<GameObject> propertyButtonList = new List<GameObject>();
    private bool thumbnails = false;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        addPropertyButton.onClick.AddListener(() => AddPropertyItem());
    }

    public void Initialize()
    {
        foreach (var propertyButton in propertyButtonList)
        {
            Destroy(propertyButton);
        }
        propertyButtonList = new List<GameObject>();
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            propertyButton = Instantiate(propertyItemPrefab, propertyInfoContent);
            propertyButton.GetComponent<PropertyButton>().Initialize(property, OpenRoomScreen, OpenPropertyRoomScreen, null);
            propertyButtonList.Add(propertyButton);
        }
        ExpandThumbnails(thumbnails);
    }

    public void ExpandThumbnails(bool expand)
    {
        StopAllCoroutines();
        foreach (GameObject property in propertyButtonList)
        {
            StartCoroutine(ExpandView(expand, property.GetComponent<RectTransform>()));
        }
    }

    private void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        OpenPropertyAdminScreen(property);
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        PropertyAdminScreen propertyAdminScreenScript = propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>();
        propertyAdminScreenScript.SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenPropertyRoomScreen(IProperty property)
    {
        PropertyRoomScreen propertyRoomScreenScript = propertyRoomScreenTransform.GetComponent<PropertyRoomScreen>();
        propertyRoomScreenScript.SetCurrentProperty(property);
        navigator.GoTo(propertyRoomScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomScreen(IRoom room)
    {
        RoomScreen roomScreenScript = roomScreenTransform.GetComponent<RoomScreen>();
        roomScreenScript.UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
    }

    public IEnumerator ExpandView(bool expand, RectTransform property)
    {
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
                yield return null;
            }
            property.sizeDelta = endSize;
        }
    }
}
