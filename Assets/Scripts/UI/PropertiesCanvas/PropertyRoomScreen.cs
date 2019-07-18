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
    private PropertyAdminScreen propertyAdminScreen = null;
    [SerializeField]
    private RoomAdminScreen roomAdminScreen = null;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private Text propertyRoomScreenTitle = null;
    [SerializeField]
    private ScrollRect propertyRoomScrollRect = null;
    [SerializeField]
    private RectTransform roomsContentScrollView = null;
    [SerializeField]
    private GameObject roomItemPrefab = null;
    [SerializeField]
    private GameObject roomFloorNumberPrefab = null;
    [SerializeField]
    private Button editButton = null;
    [SerializeField]
    private Button backButton = null;

    private List<GameObject> roomButtons = new List<GameObject>();
    private IProperty currentProperty;
    private float scrollPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        editButton.onClick.AddListener(() => EditProperty());
    }

    private void OnEnable()
    {
        Initialize();
    }

    /// <summary>
    /// set the position of the scroll rect to the top of the screen
    /// </summary>
    public void ScrollToTop()
    {
        scrollPosition = 1;
    }

    /// <summary>
    /// set the property that will be opened
    /// </summary>
    /// <param name="property">selected property</param>
    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
        Initialize();
    }

    /// <summary>
    /// set property image
    /// instantiate rooms of the selected property
    /// </summary>
    public void Initialize()
    {
        //scrollRectComponent.ResetAll();
        foreach (var roomButton in roomButtons)
        {
            DestroyImmediate(roomButton);
        }
        roomButtons = new List<GameObject>();
        if (currentProperty != null)
        {
            propertyRoomScreenTitle.text = string.IsNullOrEmpty(currentProperty.Name) ? Constants.PROPERTY : currentProperty.Name;
            List<IRoom> currentRooms = currentProperty.Rooms.OrderBy(r => r.RoomNumber).ThenBy(r => r.Name).ToList();
            int currentFloor = 0;
            int lastFloor = -1;
            int maxFloors = currentProperty.FloorRooms;
            int roomCounter = 0;
            foreach (var room in currentRooms)
            {
                if (lastFloor != currentFloor )
                {
                    GameObject floorNumber = Instantiate(roomFloorNumberPrefab, roomsContentScrollView);
                    if (currentFloor != 0)
                    {
                        floorNumber.GetComponent<Text>().text = $"Etaj {currentFloor}";
                    }
                    else
                    {
                        floorNumber.GetComponent<Text>().text = $"Etaj P";
                    }
                    lastFloor = currentFloor;
                    roomButtons.Add(floorNumber);
                }
                GameObject roomButton = Instantiate(roomItemPrefab, roomsContentScrollView);
                RoomButton currentRoom = roomButton.GetComponent<RoomButton>();
                currentRoom.Initialize(room, OpenRoomAdminScreen);
                roomButtons.Add(roomButton);
                roomCounter++;
                if(roomCounter >= maxFloors)
                {
                    roomCounter = 0;
                    currentFloor++;
                }
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(roomsContentScrollView);
        Canvas.ForceUpdateCanvases();
        propertyRoomScrollRect.verticalNormalizedPosition = scrollPosition;
        if (propertyRoomScrollRect.content.childCount > 0)
        {
            //scrollRectComponent.Init();
        }
    }

    /// <summary>
    /// set on edit property button
    /// </summary>
    public void EditProperty()
    {
        OpenPropertyAdminScreen(currentProperty);
    }

    /// <summary>
    /// open edit room screen
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
        navigator.GoTo(propertyAdminScreen.GetComponent<NavScreen>());
        propertyAdminScreen.SetCurrentProperty(property);
    }
}
