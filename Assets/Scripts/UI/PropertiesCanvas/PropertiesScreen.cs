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
    private ModalCalendarNew calendarScreen = null;
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
    private Button OpenModalClendarButton = null;
    [SerializeField]
    private Button backButton = null;
    private Canvas canvasComponent;

    private List<GameObject> propertyButtonList = new List<GameObject>();
    private DateTime startDate = DateTime.Today.Date;
    private DateTime endDate = DateTime.Today.AddDays(1).Date;
    private float tempPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        //addPropertyButton.onClick.AddListener(() => AddPropertyItem());
        OpenModalClendarButton.onClick.AddListener(() => ShowModalCalendar());
    }

    public void SetDefaultDate()
    {
        startDate = DateTime.Today.Date;
        endDate = DateTime.Today.AddDays(1).Date;
        tempPosition = 1;
    }

    public void UpdateDateTime(DateTime start, DateTime end)
    {
        startDate = start;
        endDate = end;
    }

    public void Initialize()
    {
        scrollRectComponent.ResetAll();
        foreach (var propertyButton in propertyButtonList)
        {
            DestroyImmediate(propertyButton);
        }
        propertyButtonList = new List<GameObject>();
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            propertyButton = Instantiate(propertyItemPrefab, propertyInfoContent);
            propertyButton.GetComponent<PropertyButton>().InitializeDateTime(startDate, endDate);
            propertyButton.GetComponent<PropertyButton>().Initialize(property, false, OpenRoomScreen, OpenPropertyRoomScreen, null);
            propertyButtonList.Add(propertyButton);
        }
        propertiesScrollView.verticalNormalizedPosition = tempPosition;
        LayoutRebuilder.ForceRebuildLayoutImmediate(propertyInfoContent);
        Canvas.ForceUpdateCanvases();
        scrollRectComponent.Init();
    }

    private void ShowModalCalendar()
    {
        calendarScreen.OpenCallendar(startDate, endDate, SetNewDatePeriod, true);
    }

    private void SetNewDatePeriod(DateTime startDate, DateTime endDate)
    {
        this.startDate = startDate;
        this.endDate = endDate;
        tempPosition = 1;
        Initialize();
    }

    public void LastPosition()
    {
        tempPosition = propertiesScrollView.verticalNormalizedPosition;
    }

    public void AddPropertyItem()
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
        propertyRoomScreen.ScrollToTop();
        propertyRoomScreen.UpdateDateTime(startDate, endDate);
        propertyRoomScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyRoomScreen.GetComponent<NavScreen>());
    }

    private void OpenRoomScreen(IRoom room)
    {
        LastPosition();
        roomScreen.UpdateDateTime(startDate, endDate);
        roomScreen.UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }
}
