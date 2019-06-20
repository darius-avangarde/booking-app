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
    private AspectRatioFitter propertyImageAspectFitter = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private AspectRatioFitter backgroundImageAspectFitter = null;
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
    }

    public void Initialize()
    {
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
            Destroy(roomButton);
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
        navigator.GoTo(roomAdminScreen.GetComponent<NavScreen>());
        roomAdminScreen.SetCurrentPropertyRoom(room);
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreen.SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreen.GetComponent<NavScreen>());
    }
}
