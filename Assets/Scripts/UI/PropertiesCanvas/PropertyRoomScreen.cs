using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyRoomScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ModalCalendarNew calendarScreen = null;
    [SerializeField]
    private PropertyAdminScreen propertyAdminScreen = null;
    [SerializeField]
    private RoomAdminScreen roomAdminScreen = null;
    [SerializeField]
    private PropertiesScreen propertiesScreen = null;
    [SerializeField]
    private RoomScreen roomScreen = null;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private ScrollRect propertyRoomScrollRect = null;
    [SerializeField]
    private Transform roomsContentScrollView = null;
    [SerializeField]
    private Text propertyRoomScreenTitle = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private AspectRatioFitter propertyImageAspectFitter = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private GameObject roomItemPrefab = null;
    [SerializeField]
    private Button openCalendarButton = null;
    [SerializeField]
    private Button backButton = null;

    private List<GameObject> roomButtons = new List<GameObject>();
    private DateTime startDate = DateTime.Today.Date;
    private DateTime endDate = DateTime.Today.AddDays(1).Date;
    private IProperty currentProperty;
    private float tempPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        openCalendarButton.onClick.AddListener(() => ShowModalCalendar());
    }

    public void ScrollToTop()
    {
        tempPosition = 1;
    }

    public void UpdateDateTime(DateTime start, DateTime end)
    {
        startDate = start;
        endDate = end;
    }

    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
    }

    public void Initialize()
    {
        scrollRectComponent.ResetAll();
        propertyRoomScreenTitle.text = string.IsNullOrEmpty(currentProperty.Name) ? Constants.PROPERTY : currentProperty.Name;
        if (ImageDataManager.PropertyPhotos.ContainsKey(currentProperty.ID))
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
            backgroundImage.sprite = (Sprite)ImageDataManager.BlurPropertyPhotos[currentProperty.ID];
            backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[Constants.defaultPropertyPicture];
            backgroundImage.gameObject.SetActive(false);
        }
        propertyImageAspectFitter.aspectRatio = (float)propertyImage.sprite.texture.width/propertyImage.sprite.texture.height;
        foreach (var roomButton in roomButtons)
        {
            DestroyImmediate(roomButton);
        }
        if (currentProperty != null)
        {
            propertyRoomScreenTitle.text = string.IsNullOrEmpty(currentProperty.Name) ? Constants.PROPERTY : currentProperty.Name;
            List<IRoom> currentRooms = currentProperty.Rooms.OrderBy(r => r.RoomNumber).ThenBy(r => r.Name).ToList();
            foreach (var room in currentRooms)
            {
                GameObject roomButton = Instantiate(roomItemPrefab, roomsContentScrollView);
                roomButton.GetComponent<RoomButton>().Initialize(room, null, OpenRoomScreen, null);
                roomButtons.Add(roomButton);
            }
        }
        propertyRoomScrollRect.verticalNormalizedPosition = tempPosition;
        if (propertyRoomScrollRect.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
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

    public void AddRoomItem()
    {
        IRoom room = currentProperty.AddRoom();
        OpenRoomAdminScreen(room);
    }

    public void EditProperty()
    {
        OpenPropertyAdminScreen(currentProperty);
    }

    public void SetPropertyDate()
    {
        propertiesScreen.UpdateDateTime(startDate, endDate);
    }

    private void OpenRoomScreen(IRoom room)
    {
        tempPosition = propertyRoomScrollRect.verticalNormalizedPosition;
        roomScreen.UpdateDateTime(startDate, endDate);
        roomScreen.UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen(IRoom room)
    {
        navigator.GoTo(roomAdminScreen.GetComponent<NavScreen>());
        roomAdminScreen.SetCurrentPropertyRoom(room);
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreen.GetComponent<NavScreen>());
    }
}
