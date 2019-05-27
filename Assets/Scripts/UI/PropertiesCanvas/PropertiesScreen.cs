using System;
using System.Linq;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesScreen : MonoBehaviour
{
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

    private Transform roomsContentScrollView = null;
    private List<GameObject> propertyButtons = new List<GameObject>();
    private List<GameObject> roomButtons = new List<GameObject>();
    private int nrRooms = 0;

    private int index = 0;

    public void InstantiateProperties()
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
            }
            else
            {
                propertyButton = Instantiate(propertyWithoutRoomsPrefab, propertyInfoContent);
            }
            string rooms = "Nr. Camere: " + nrRooms;
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
        roomsContentScrollView.gameObject.SetActive(false);
    }

    public void AddPropertyItem()
    {
        IProperty property = PropertyDataManager.AddProperty();
        OpenPropertyAdminScreen(property);
    }

    public void AddRoomItem(IProperty selectedProperty)
    {
        IRoom room = selectedProperty.AddRoom();
        OpenRoomAdminScreen(room);
    }

    public void DeleteProperty(IProperty property)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Ștergeți proprietatea?",
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () =>
            {
                PropertyDataManager.DeleteProperty(property.ID);
                ReservationDataManager.DeleteReservationsForProperty(property.ID);
                InstantiateProperties();
            },
            CancelCallback = null
        });
    }

    public void DeleteRoom(IRoom selectedRoom)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Ștergeți camera?",
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () =>
            {
                IProperty selectedProperty = PropertyDataManager.GetProperty(selectedRoom.PropertyID);
                selectedProperty.DeleteRoom(selectedRoom.ID);
                ReservationDataManager.DeleteReservationsForRoom(selectedRoom.ID);
                InstantiateProperties();
            },
            CancelCallback = null
        });
    }

    private void OpenPropertyAdminScreen(IProperty property)
    {
        propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().SetCurrentProperty(property);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen(IRoom room)
    {
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().SetCurrentPropertyRoom(room);
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomScreen(IRoom room)
    {
        roomScreenTransform.GetComponent<RoomScreen>().UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
    }
}
