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
    private RectTransform roomsContentScrollView = null;
    [SerializeField]
    private Text propertyRoomScreenTitle = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private AspectRatioFitter propertyImageAspectFitter = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private AspectRatioFitter backgroundImageAspectFitter = null;
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
    private float scrollPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        openCalendarButton.onClick.AddListener(() => ShowModalCalendar());
    }

    /// <summary>
    /// set the position of the scroll rect to the top of the screen
    /// </summary>
    public void ScrollToTop()
    {
        scrollPosition = 1;
    }

    /// <summary>
    /// set new date period
    /// </summary>
    /// <param name="start">start of date period</param>
    /// <param name="end">end of date period</param>
    public void UpdateDateTime(DateTime start, DateTime end)
    {
        startDate = start;
        endDate = end;
    }

    /// <summary>
    /// set the property that will be opened
    /// </summary>
    /// <param name="property">selected property</param>
    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
    }

    /// <summary>
    /// set property image
    /// instantiate rooms of the selected property
    /// </summary>
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
        propertyImageAspectFitter.aspectRatio = backgroundImageAspectFitter.aspectRatio = (float)propertyImage.sprite.texture.width/propertyImage.sprite.texture.height;
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
                RoomButton currentRoom = roomButton.GetComponent<RoomButton>();
                currentRoom.InitializeDateTime(startDate, endDate);
                currentRoom.Initialize(room, null, OpenRoomScreen, null);
                roomButtons.Add(roomButton);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(roomsContentScrollView);
        Canvas.ForceUpdateCanvases();
        propertyRoomScrollRect.verticalNormalizedPosition = scrollPosition;
        if (propertyRoomScrollRect.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
    }

    /// <summary>
    /// open modal calendar canvas with current date set
    /// callback returns selected date
    /// </summary>
    private void ShowModalCalendar()
    {
        calendarScreen.OpenCallendar(startDate, endDate, SetNewDatePeriod, true);
    }

    /// <summary>
    /// callback for the modal calendar to refresh screen with the selected date
    /// </summary>
    /// <param name="startDate">start of date period</param>
    /// <param name="endDate">end of date period</param>
    private void SetNewDatePeriod(DateTime startDate, DateTime endDate)
    {
        this.startDate = startDate;
        this.endDate = endDate;
        scrollPosition = 1;
        Initialize();
    }

    /// <summary>
    /// set to add new room button
    /// </summary>
    public void AddRoomItem()
    {
        IRoom room = currentProperty.AddRoom();
        OpenRoomAdminScreen(room);
    }

    /// <summary>
    /// set on edit property button
    /// </summary>
    public void EditProperty()
    {
        OpenPropertyAdminScreen(currentProperty);
    }

    /// <summary>
    /// OnHiding function - sets the selected period to properties screen on pressing back
    /// </summary>
    public void SetPropertyDate()
    {
        propertiesScreen.UpdateDateTime(startDate, endDate);
    }

    /// <summary>
    /// open the room screen with the reservations
    /// </summary>
    /// <param name="room">selected room</param>
    private void OpenRoomScreen(IRoom room)
    {
        scrollPosition = propertyRoomScrollRect.verticalNormalizedPosition;
        roomScreen.UpdateDateTime(startDate, endDate);
        roomScreen.UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }

    /// <summary>
    /// open add or edit room screen
    /// </summary>
    /// <param name="room"> selected room</param>
    private void OpenRoomAdminScreen(IRoom room)
    {
        navigator.GoTo(roomAdminScreen.GetComponent<NavScreen>());
        roomAdminScreen.SetCurrentPropertyRoom(room);
    }

    /// <summary>
    /// open edit property screen
    /// </summary>
    /// <param name="property">current property</param>
    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreen.GetComponent<NavScreen>());
    }
}
