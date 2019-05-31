using System;
using System.Linq;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesScreen : MonoBehaviour
{
    public string OpenRoomDropdown { get; set; }

    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private Transform roomAdminScreenTransform = null;
    [SerializeField]
    private Transform roomScreenTransform = null;
    [SerializeField]
    private GameObject propertyWithRoomsPrefab = null;
    [SerializeField]
    private GameObject propertyWithoutRoomsPrefab = null;
    [SerializeField]
    private GameObject roomPrefabButton = null;
    [SerializeField]
    private Transform addPropertyButton = null;
    [SerializeField]
    private RectTransform propertyInfoContent = null;
    [SerializeField]
    private Button backButton;

    private Transform roomsContentScrollView = null;
    private List<GameObject> propertyButtons = new List<GameObject>();
    private List<GameObject> roomButtons = new List<GameObject>();
    private int nrRooms = 0;
    private int index = 0;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
    }

    public void Initialize()
    {
        foreach (var propertyButton in propertyButtons)
        {
            Destroy(propertyButton);
        }
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            if (property.HasRooms)
            {
                propertyButton = Instantiate(propertyWithRoomsPrefab, propertyInfoContent);
                PropertyButton buttonObject = propertyButton.GetComponent<PropertyButton>();
                roomsContentScrollView = buttonObject.RoomsContentScrollView;
                roomButtons = buttonObject.RoomButtons;
                InstantiateRooms(property);
                if (property.ID == OpenRoomDropdown)
                {
                    buttonObject.OpenRoomContents();
                }
                else
                {
                    roomsContentScrollView.gameObject.SetActive(false);
                }
            }
            else
            {
                propertyButton = Instantiate(propertyWithoutRoomsPrefab, propertyInfoContent);
            }
            string rooms = Constants.ROOMS_NUMBER + nrRooms;
            propertyButton.GetComponent<PropertyButton>().Initialize(property, propertyInfoContent, false, rooms, AddRoomItem, OpenRoomScreen, OpenPropertyAdminScreen, DeleteProperty);
            propertyButtons.Add(propertyButton);
            nrRooms = 0;
            index++;
        }
        addPropertyButton.SetSiblingIndex(index);
    }

    private void InstantiateRooms(IProperty property)
    {
        foreach (var roomButton in roomButtons)
        {
            Destroy(roomButton);
        }
        if (property != null)
        {
            foreach (var room in property.Rooms)
            {
                GameObject roomButton = Instantiate(roomPrefabButton, roomsContentScrollView);
                roomButton.GetComponent<RoomButton>().Initialize(room, OpenRoomScreen, OpenRoomAdminScreen, DeleteRoom);
                roomButtons.Add(roomButton);
                nrRooms++;
            }
        }
    }

    public void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        OpenPropertyAdminScreen(property);
    }

    private void AddRoomItem(IProperty selectedProperty)
    {
        IRoom room = selectedProperty.AddRoom();
        OpenRoomAdminScreen(room);
    }

    private void DeleteProperty(IProperty property)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_PROPERTY,
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () =>
            {
                PropertyDataManager.DeleteProperty(property.ID);
                ReservationDataManager.DeleteReservationsForProperty(property.ID);
                Initialize();
            },
            CancelCallback = null
        });
    }

    private void DeleteRoom(IRoom selectedRoom)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_ROOM,
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () =>
            {
                IProperty selectedProperty = PropertyDataManager.GetProperty(selectedRoom.PropertyID);
                selectedProperty.DeleteRoom(selectedRoom.ID);
                ReservationDataManager.DeleteReservationsForRoom(selectedRoom.ID);
                Initialize();
            },
            CancelCallback = null
        });
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        PropertyAdminScreen propertyAdminScreenScript = propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>();
        propertyAdminScreenScript.SetCurrentProperty(property);
        propertyAdminScreenScript.propertiesScreen = this;
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen(IRoom room)
    {
        RoomAdminScreen roomAdminScreenScript = roomAdminScreenTransform.GetComponent<RoomAdminScreen>();
        roomAdminScreenScript.SetCurrentPropertyRoom(room);
        roomAdminScreenScript.propertiesScreen = this;
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomScreen(IRoom room)
    {
        roomScreenTransform.GetComponent<RoomScreen>().UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
    }
}
