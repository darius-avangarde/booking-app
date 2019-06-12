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
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private Transform roomAdminScreenTransform = null;
    [SerializeField]
    private Transform roomScreenTransform = null;
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
                roomButton.GetComponent<RoomButton>().Initialize(room, null, OpenRoomScreen);
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
        RoomScreen roomScreenScript = roomScreenTransform.GetComponent<RoomScreen>();
        roomScreenScript.UpdateRoomDetailsFields(room);
        //roomScreenScript.propertiesScreen = this;
        navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen(IRoom room)
    {
        RoomAdminScreen roomAdminScreenScript = roomAdminScreenTransform.GetComponent<RoomAdminScreen>();
        roomAdminScreenScript.SetCurrentPropertyRoom(room);
        //roomAdminScreenScript.propertiesScreen = this;
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        PropertyAdminScreen propertyAdminScreenScript = propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>();
        propertyAdminScreenScript.SetCurrentProperty(property);
        //propertyAdminScreenScript.propertiesScreen = this;
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }
}
