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
    private ThemeManager themeManager = null;
    [SerializeField]
    private NavScreen propertyRoomScreen = null;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private Text propertyRoomScreenTitle = null;
    [SerializeField]
    private ScrollRect propertyRoomScrollRect = null;
    [SerializeField]
    private RectTransform roomsContentScrollView = null;
    [SerializeField]
    private Shadow roomItemPrefab = null;
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
        navigator.GoTo(propertyRoomScreen);
    }

    /// <summary>
    /// set property image
    /// instantiate rooms of the selected property
    /// </summary>
    public void Initialize()
    {
        //scrollRectComponent.ResetAll();
        for (int i = 0; i < roomButtons.Count; i++)
        {
            DestroyImmediate(roomButtons[i]);
        }
        roomButtons = new List<GameObject>();
        if (currentProperty != null)
        {
            propertyRoomScreenTitle.text = string.IsNullOrEmpty(currentProperty.Name) ? Constants.PROPERTY : currentProperty.Name;
            List<IRoom> currentRooms = currentProperty.Rooms.OrderBy(r => r.RoomNumber).ThenBy(r => r.Name).ToList();
            int currentFloor = 0;
            int lastFloor = -1;
            int maxFloors = currentProperty.Floors;
            for (int i = 0; i < currentRooms.Count; i++)
            {
                currentFloor = currentRooms[i].Floor;
                if (lastFloor != currentFloor )
                {
                    GameObject floorNumber = Instantiate(roomFloorNumberPrefab, roomsContentScrollView);
                    if (currentFloor != 0)
                    {
                        floorNumber.GetComponent<Text>().text = $"{LocalizedText.Instance.FloorType[1]} {currentFloor}";
                    }
                    else
                    {
                        floorNumber.GetComponent<Text>().text = $"{LocalizedText.Instance.FloorType[1]} P";
                    }
                    themeManager.SetColor(floorNumber.GetComponent<Graphic>());
                    roomButtons.Add(floorNumber);
                    lastFloor = currentFloor;
                }
                themeManager.SetShadow(roomItemPrefab);
                GameObject roomButton = Instantiate(roomItemPrefab.gameObject, roomsContentScrollView);
                RoomButton currentRoom = roomButton.GetComponent<RoomButton>();
                currentRoom.Initialize(currentRooms[i], OpenRoomAdminScreen, themeManager);
                roomButtons.Add(roomButton);
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
        roomAdminScreen.OpenRoomAdminScreen(room);
    }

    /// <summary>
    /// open edit property screen
    /// </summary>
    /// <param name="property">current property</param>
    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreen.OpenPropertyAdminScreen(property);
    }
}
