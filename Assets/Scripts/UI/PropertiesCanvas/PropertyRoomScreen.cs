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
    private GameObject roomItemPrefab = null;
    [SerializeField]
    private Button editButton = null;
    [SerializeField]
    private Button backButton = null;

    private List<RoomButton> roomButtonsList = new List<RoomButton>();
    private IProperty currentProperty;
    private float scrollPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        editButton.onClick.AddListener(() => EditProperty());
    }

    private void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            InstantiateRoomButtonObject();
        }
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
        navigator.GoTo(propertyRoomScreen);
        StartCoroutine(SetCurrentPropertyRoutine(property));
    }

    public IEnumerator SetCurrentPropertyRoutine(IProperty property)
    {
        Handheld.StartActivityIndicator();
        currentProperty = property;
        Initialize();
        yield return new WaitForEndOfFrame();
        Handheld.StopActivityIndicator();
    }

    /// <summary>
    /// set property image
    /// instantiate rooms of the selected property
    /// </summary>
    public void Initialize()
    {
        //scrollRectComponent.ResetAll();
        if (currentProperty != null)
        {
            propertyRoomScreenTitle.text = currentProperty.Name;
            List<IRoom> currentRooms = currentProperty.Rooms.OrderBy(r => r.RoomNumber).ThenBy(r => r.Name).ToList();
            int neededObjects = currentRooms.Count + currentProperty.Floors;
            if (neededObjects != roomButtonsList.Count)
            {
                //Create New Objects as needed
                for (int i = roomButtonsList.Count - 1; i < neededObjects; i++)
                {
                    InstantiateRoomButtonObject();
                }

                //Disable unused objects
                for (int i = roomButtonsList.Count - 1; i >= neededObjects; i--)
                {
                    roomButtonsList[i].gameObject.SetActive(false);
                }
            }

            int currentFloor = 0;
            int lastFloor = -1;
            int maxFloors = currentProperty.Floors;
            int currentRoomsCounter = 0;
            for (int i = 0; i <= neededObjects; i++)
            {
                currentFloor = currentRooms[currentRoomsCounter].Floor;
                if (lastFloor != currentFloor )
                {
                    RoomButton floorNumber = roomButtonsList[i];
                    floorNumber.gameObject.SetActive(true);
                    if (currentFloor != 0)
                    {
                        floorNumber.InitializeFloor($"{LocalizedText.Instance.FloorType[1]} {currentFloor}");
                    }
                    else
                    {
                        floorNumber.InitializeFloor($"{LocalizedText.Instance.FloorType[1]} P");
                    }
                    lastFloor = currentFloor;
                    continue;
                }
                RoomButton currentRoom = roomButtonsList[i];
                currentRoom.gameObject.SetActive(true);
                currentRoom.InitializeRoom(currentRooms[currentRoomsCounter], OpenRoomAdminScreen);
                currentRoomsCounter++;
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
    /// function to instantiate new room object for pool
    /// </summary>
    public void InstantiateRoomButtonObject()
    {
        GameObject roomButtonObject = Instantiate(roomItemPrefab.gameObject, roomsContentScrollView);
        roomButtonObject.SetActive(false);
        RoomButton roomButton = roomButtonObject.GetComponent<RoomButton>();
        roomButton.InitializeTheme(themeManager);
        roomButtonsList.Add(roomButton);
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
