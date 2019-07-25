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
    private RoomAdminScreen roomAdminScreen = null;
    [SerializeField]
    private ThemeManager themeManager = null;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private RectTransform propertiesContainerContent = null;
    [SerializeField]
    private ScrollRect propertiesScrollView = null;
    [SerializeField]
    private GameObject propertyItemPrefab = null;
    [SerializeField]
    private Button addPropertyButton = null;
    [SerializeField]
    private Button backButton = null;

    private List<GameObject> propertyButtonList = new List<GameObject>();
    private float scrollPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        addPropertyButton.onClick.AddListener(() => AddPropertyItem());
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        LastPosition();
    }

    /// <summary>
    /// instantiate the properties items
    /// </summary>
    public void Initialize()
    {
        //scrollRectComponent.ResetAll();
        foreach (var propertyButton in propertyButtonList)
        {
            DestroyImmediate(propertyButton);
        }
        propertyButtonList = new List<GameObject>();
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            propertyButton = Instantiate(propertyItemPrefab, propertiesContainerContent);
            if (property.HasRooms)
            {
                propertyButton.GetComponent<PropertyButton>().Initialize(property, OpenPropertyRoomScreen, SetTheme);
            }
            else
            {
                propertyButton.GetComponent<PropertyButton>().Initialize(property, OpenPropertyAdminScreen, SetTheme);
            }
            propertyButtonList.Add(propertyButton);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(propertiesContainerContent);
        Canvas.ForceUpdateCanvases();
        propertiesScrollView.verticalNormalizedPosition = scrollPosition;
        if (propertiesScrollView.content.childCount > 0)
        {
            //scrollRectComponent.Init();
        }
    }

    private void SetTheme(Graphic myObj)
    {
        themeManager.SetColor(myObj);
    }

    /// <summary>
    /// set the scroll position to last one before moving to another screen
    /// </summary>
    public void LastPosition()
    {
        scrollPosition = propertiesScrollView.verticalNormalizedPosition;
    }

    /// <summary>
    /// function to the add button, to open the add new property screen
    /// </summary>
    private void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        OpenPropertyAdminScreen(property);
    }

    /// <summary>
    /// open the screen with the rooms of the current property
    /// </summary>
    /// <param name="property">current property</param>
    private void OpenPropertyRoomScreen(IProperty property)
    {
        propertyRoomScreen.ScrollToTop();
        propertyRoomScreen.SetCurrentProperty(property);
    }

    /// <summary>
    /// open the add or edit property screen
    /// </summary>
    /// <param name="property">the property to edit or the new property to create</param>
    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreen.OpenPropertyAdminScreen(property);
    }
}
