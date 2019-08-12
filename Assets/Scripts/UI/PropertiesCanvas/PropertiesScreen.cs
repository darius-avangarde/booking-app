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
    private Shadow propertyItemPrefab = null;
    [SerializeField]
    private Button addPropertyButton = null;
    [SerializeField]
    private Button backButton = null;

    private List<PropertyButton> propertyButtonList = new List<PropertyButton>();
    private float scrollPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        addPropertyButton.onClick.AddListener(() => AddPropertyItem());
    }
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            InstantiatePropertyButton();
        }
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
        StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        Handheld.StartActivityIndicator();
        scrollRectComponent.ResetAll();
        List<IProperty> properties = PropertyDataManager.GetProperties().ToList();

        if(properties.Count != propertyButtonList.Count)
        {
            //Create New Objects as needed
            for (int i = propertyButtonList.Count - 1; i < properties.Count; i++)
            {
                InstantiatePropertyButton();
            }

            //Disable unused objects
            for (int i = propertyButtonList.Count - 1; i >= properties.Count; i--)
            {
                propertyButtonList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < properties.Count(); i++)
        {
            propertyButtonList[i].gameObject.SetActive(true);
            PropertyButton propertyButton = propertyButtonList[i];
            if (properties[i].HasRooms)
            {
                propertyButton.Initialize(properties[i], OpenPropertyRoomScreen);
            }
            else
            {
                propertyButton.Initialize(properties[i], OpenPropertyAdminScreen);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(propertiesContainerContent);
        Canvas.ForceUpdateCanvases();
        propertiesScrollView.verticalNormalizedPosition = scrollPosition;
        if (propertiesScrollView.content.childCount > 0)
        {
            //scrollRectComponent.Init();
        }
        yield return new WaitForEndOfFrame();
        Handheld.StopActivityIndicator();
    }

    private void InstantiatePropertyButton()
    {
        GameObject propertyButtonObject = Instantiate(propertyItemPrefab.gameObject, propertiesContainerContent);
        propertyButtonObject.SetActive(false);
        PropertyButton propertyButton = propertyButtonObject.GetComponent<PropertyButton>();
        propertyButton.InitializeTheme(themeManager);
        propertyButtonList.Add(propertyButton);
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
        propertyAdminScreen.OpenPropertyAdminScreen(null);
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
