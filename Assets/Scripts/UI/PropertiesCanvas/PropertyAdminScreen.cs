using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
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
    [SerializeField]
    private Text propertyNamePlaceholder;
    [SerializeField]
    private InputField propertyNameInputField = null;
    [SerializeField]
    private GameObject roomPrefabButton = null;
    [SerializeField]
    private Transform roomsContentScrollView = null;
    private List<GameObject> roomButtons = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        InstantiateRooms();
    }

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
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().UpdateRoomFields(currentProperty, room);
    }

    public void DeleteProperty()
    {
        confirmationDialog.Show(() => {
            PropertyDataManager.DeleteProperty(currentProperty.ID);
            navigator.GoBack();
            propertiesScreenTransform.GetComponent<PropertiesScreen>().InstantiateProperties();
        }, null);
    }

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

    private void OpenRoomAdminScreen(IProperty property, IRoom room)
    {
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().UpdateRoomFields(property, room);
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }
}
