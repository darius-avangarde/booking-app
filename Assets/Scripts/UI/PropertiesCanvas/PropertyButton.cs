using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UINavigation;

public class PropertyButton : MonoBehaviour
{
    public Navigator navigator { get; set; } = null;
    public RectTransform layoutContent { get; set; }
    public Transform roomAdminScreenTransform { get; set; } = null;
    [SerializeField]
    private Transform roomsContentScrollView;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Text nrOfRooms = null;
    [SerializeField]
    private Button propertyButtonItem;
    [SerializeField]
    private Button editPropertyButton;
    [SerializeField]
    private GameObject roomPrefabButton = null;
    private List<GameObject> roomButtons = new List<GameObject>();
    private IProperty selectedProperty;

    public void Initialize(IProperty property, Action<IProperty> callback)
    {
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        InstantiateRooms(property);
        editPropertyButton.onClick.AddListener(() => callback(property));
        if (!property.HasRooms)
        {
            //propertyButtonItem.onClick.AddListener(() => roomButtons[0].GetComponent<RoomButton>().OpenRoom());
        }
        else
        {
            nrOfRooms.text = string.IsNullOrEmpty(property.NrRooms) ? Constants.defaultProperyAdminScreenNrRooms : ("Nr. Camere: " + property.NrRooms);
            propertyButtonItem.onClick.AddListener(() =>
            {
                roomsContentScrollView.gameObject.SetActive(roomsContentScrollView.gameObject.activeInHierarchy ? false : true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutContent);
                Canvas.ForceUpdateCanvases();
            });
        }
        selectedProperty = property;
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
                property.HasRooms = true;
                GameObject roomButton = Instantiate(roomPrefabButton, roomsContentScrollView);
                roomButton.GetComponent<RoomButton>().Initialize(room, OpenRoomAdminScreen);
                roomButtons.Add(roomButton);
            }
        }
        roomsContentScrollView.gameObject.SetActive(false);
    }

    public void AddRoomItem()
    {
        IRoom room = selectedProperty.AddRoom();
        OpenRoomAdminScreen(room);
    }

    public void DeleteProperty()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Ștergeți proprietatea?",
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () =>
            {
                PropertyDataManager.DeleteProperty(selectedProperty.ID);
                ReservationDataManager.DeleteReservationsForProperty(selectedProperty.ID);
            },
            CancelCallback = null
        });
    }

    private void OpenRoomAdminScreen(IRoom room)
    {
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().SetCurrentPropertyRoom(selectedProperty, room);
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }
}
