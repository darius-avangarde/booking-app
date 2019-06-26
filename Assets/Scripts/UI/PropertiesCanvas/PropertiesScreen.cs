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

    private List<GameObject> propertyButtonList = new List<GameObject>();
    private DateTime startDate = DateTime.Today.Date;
    private DateTime endDate = DateTime.Today.AddDays(1).Date;
    private float scrollPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        OpenModalClendarButton.onClick.AddListener(() => ShowModalCalendar());
    }

    /// <summary>
    /// resets the date to the current date when the user opens the screen from main menu
    /// </summary>
    public void SetDefaultDate()
    {
        startDate = DateTime.Today.Date;
        endDate = DateTime.Today.AddDays(1).Date;
        scrollPosition = 1;
    }

    /// <summary>
    /// filter the date period for the items from the list
    /// </summary>
    /// <param name="start">start of the date period</param>
    /// <param name="end">end of the date period</param>
    public void UpdateDateTime(DateTime start, DateTime end)
    {
        startDate = start;
        endDate = end;
    }

    /// <summary>
    /// instantiate the properties items
    /// </summary>
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
        LayoutRebuilder.ForceRebuildLayoutImmediate(propertyInfoContent);
        Canvas.ForceUpdateCanvases();
        propertiesScrollView.verticalNormalizedPosition = scrollPosition;
        if (propertiesScrollView.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
    }

    /// <summary>
    /// open modal calendar with the current date set
    ///  cakkback returns the new date
    /// </summary>
    private void ShowModalCalendar()
    {
        calendarScreen.OpenCallendar(startDate, endDate, SetNewDatePeriod, true);
    }

    /// <summary>
    /// modal calendar callback to set the new date period
    /// </summary>
    /// <param name="startDate">start of the date period</param>
    /// <param name="endDate">end of the date period</param>
    private void SetNewDatePeriod(DateTime startDate, DateTime endDate)
    {
        this.startDate = startDate;
        this.endDate = endDate;
        scrollPosition = 1;
        Initialize();
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
    public void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        OpenPropertyAdminScreen(property);
    }

    /// <summary>
    /// open the add or edit property screen
    /// </summary>
    /// <param name="property">the property to edit or the new property to create</param>
    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreen.GetComponent<NavScreen>());
    }

    /// <summary>
    /// open the screen with the rooms of the current property
    /// </summary>
    /// <param name="property">current property</param>
    private void OpenPropertyRoomScreen(IProperty property)
    {
        propertyRoomScreen.ScrollToTop();
        propertyRoomScreen.UpdateDateTime(startDate, endDate);
        propertyRoomScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyRoomScreen.GetComponent<NavScreen>());
    }

    /// <summary>
    /// opens the reservations for a property without rooms
    /// a property without rooms has a room created by script, the user will not interact with that room directly
    /// </summary>
    /// <param name="room"> the room from the property without rooms</param>
    private void OpenRoomScreen(IRoom room)
    {
        LastPosition();
        roomScreen.UpdateDateTime(startDate, endDate);
        roomScreen.UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }
}
