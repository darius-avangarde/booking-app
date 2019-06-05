using System;
using System.Linq;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
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
    private Button addPropertyButton = null;
    [SerializeField]
    private Button backButton;

    private List<GameObject> propertyButtons = new List<GameObject>();

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
    }

    public void Initialize()
    {
        foreach (var propertyButton in propertyButtons)
        {
            Destroy(propertyButton);
        }
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            propertyButton = Instantiate(propertyItemPrefab, propertyInfoContent);
            propertyButton.GetComponent<PropertyButton>().Initialize(property, propertyInfoContent, OpenRoomScreen, OpenPropertyRoomScreen);
            propertyButtons.Add(propertyButton);
        }
    }

    public void ExpandThumbnails()
    {

    }

    public void AddPropertyItem()
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
}
