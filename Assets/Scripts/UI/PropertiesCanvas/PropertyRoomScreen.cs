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
    private RoomScreen roomScreen = null;
    [SerializeField]
    private Transform roomsContentScrollView = null;
    [SerializeField]
    private Text propertyRoomScreenTitle = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private GameObject roomItemPrefab = null;
    [SerializeField]
    private Button backButton = null;

    private List<GameObject> roomButtons = new List<GameObject>();
    private IProperty currentProperty;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
    }

    public void SetCurrentProperty(IProperty property)
    {
        currentProperty = property;
        propertyRoomScreenTitle.text = string.IsNullOrEmpty(currentProperty.Name) ? Constants.PROPERTY : currentProperty.Name;
        if (ImageDataManager.PropertyPhotos.ContainsKey(property.ID))
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[property.ID];
        }
        else
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[Constants.defaultPropertyPicture];
        }
    }

    public void Initialize()
    {
        foreach (var roomButton in roomButtons)
        {
            Destroy(roomButton);
        }
        if (currentProperty != null)
        {
            foreach (var room in currentProperty.Rooms)
            {
                GameObject roomButton = Instantiate(roomItemPrefab, roomsContentScrollView);
                roomButton.GetComponent<RoomButton>().Initialize(room, null, OpenRoomScreen, null);
                roomButtons.Add(roomButton);
            }
        }
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

    private void OpenRoomScreen(IRoom room)
    {
        roomScreen.UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen(IRoom room)
    {
        roomAdminScreen.SetCurrentPropertyRoom(room);
        navigator.GoTo(roomAdminScreen.GetComponent<NavScreen>());
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreen.GetComponent<NavScreen>());
    }
}
