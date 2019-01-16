using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertyAdminScreen : MonoBehaviour
{
    public IProperty currentProperty;
    [SerializeField]
    private Navigator navigator;
    [SerializeField]
    private Transform roomAdminScreenTransform;
    [SerializeField]
    private Text propertyScreenTitle;
    [SerializeField]
    private Text propertyNamePlaceholder;
    [SerializeField]
    private InputField propertyNameInputField;
    [SerializeField]
    private GameObject roomPrefabButton;
    [SerializeField]
    private Transform roomsContentScrollView;
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
    }
    public void AddRoomItem()
    {
        IRoom room = currentProperty.AddRoom();
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().UpdateRoomFields(currentProperty, room);
    }

    public void DeleteProperty()
    {
        PropertyDataManager.DeleteProperty(currentProperty.ID);
    }
    
    public void OnValueChanged(string value)
    {
        propertyScreenTitle.text = value;
        currentProperty.Name = string.IsNullOrEmpty(value) ? propertyNamePlaceholder.text : value;
    }

    public void InstantiateRooms()
    {
        foreach (var roomButton in roomButtons)
        {
            Destroy(roomButton);
        }

        foreach (var property in PropertyDataManager.GetProperties())
        {
            foreach (var room in property.Rooms)
            {
                GameObject roomButton = Instantiate(roomPrefabButton, roomsContentScrollView);
                roomButton.GetComponent<RoomItem>().Initialize(room, () => OpenRoomAdminScreen(property, room));
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
