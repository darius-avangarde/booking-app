using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    // TODO: we can probably make this private
    public IProperty currentProperty;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform propertiesScreenTransform = null;
    [SerializeField]
    private Transform roomAdminScreenTransform = null;
    [SerializeField]
    private Text propertyScreenTitle = null;
    // TODO: we can probably remove this
    [SerializeField]
    private Text propertyNamePlaceholder;
    [SerializeField]
    private InputField propertyNameInputField = null;
    [SerializeField]
    private GameObject roomPrefabButton = null;
    [SerializeField]
    private Transform roomsContentScrollView = null;
    private List<GameObject> roomButtons = new List<GameObject>();

    // TODO: we don't need to call InstantiateRooms in start if we do it from NavScreen.Showing
    void Start()
    {
        InstantiateRooms();
    }

    // TODO: we should let PropertyAdminScreen initialize itself (through NavScreen.Showing)
    // this way we can limit other scrips' access to simply setting the current property
    public void UpdatePropertyFields(IProperty property)
    {
        currentProperty = property;
        propertyNameInputField.text = currentProperty.Name ?? "";
        propertyScreenTitle.text = currentProperty.Name ?? Constants.defaultProperyAdminScreenName;
        InstantiateRooms();
    }

    public void AddRoomItem()
    {
        IRoom room = currentProperty.AddRoom();
        // TODO: instead of updating the room fields in the room admin screen we should just set the data and let RoomAdminScreen deal with the rest
        // we should minimize the amount of coupling between components (and classes in general)
        // in this case we can get away with just setting the data (property & room) and letting RoomAdminScreen update itself when it needs
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().UpdateRoomFields(currentProperty, room);
    }

    public void DeleteProperty()
    {
        // TODO: made the cancelCallback optional, we can remove the second argument
        confirmationDialog.Show(() => {
            PropertyDataManager.DeleteProperty(currentProperty.ID);
            navigator.GoBack();
            // TODO: PropertiesScreen needs to deal with initializing by itself, we don't need to tell it
            propertiesScreenTransform.GetComponent<PropertiesScreen>().InstantiateProperties();
        }, null);
    }

    // TODO: naming this OnNameChanged is more informative
    public void OnValueChanged(string value)
    {
        propertyScreenTitle.text = value;
        currentProperty.Name = string.IsNullOrEmpty(value) ? Constants.defaultProperyAdminScreenName : value;
    }

    public void InstantiateRooms()
    {
        foreach (var roomButton in roomButtons)
        {
            Destroy(roomButton);
        }
        if (currentProperty != null)
        {
            foreach (var room in currentProperty.Rooms)
            {
                GameObject roomButton = Instantiate(roomPrefabButton, roomsContentScrollView);
                roomButton.GetComponent<RoomItem>().Initialize(room, () => OpenRoomAdminScreen(currentProperty, room));
                roomButtons.Add(roomButton);
            }
        }
    }

    // TODO: property is always going to be currentProperty so we don't need it as a parameter
    private void OpenRoomAdminScreen(IProperty property, IRoom room)
    {
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().UpdateRoomFields(property, room);
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }
}
