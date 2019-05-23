﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UINavigation;

public class PropertyButton : MonoBehaviour
{
    [SerializeField]
    private Transform roomsContentScrollView = null;
    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Text nrOfRooms = null;
    [SerializeField]
    private Button propertyButtonItem = null;
    [SerializeField]
    private Button editPropertyButton = null;
    [SerializeField]
    private Button deletePropertyButton = null;
    [SerializeField]
    private GameObject roomPrefabButton = null;
    [SerializeField]
    private GameObject roomArrowLeft = null;
    [SerializeField]
    private GameObject roomArrowDown = null;

    private List<GameObject> roomButtons = new List<GameObject>();
    private Navigator navigator;
    private ConfirmationDialog confirmationDialog;
    private IProperty selectedProperty;
    private RectTransform layoutContent;
    private Transform roomAdminScreenTransform;
    private Transform roomScreenTransform;

    public void Initialize(IProperty property, Navigator nav, ConfirmationDialog ConfirmPopUp, RectTransform layout, Transform roomAdminScreen, Transform roomScreen, Action<IProperty> editCallback, Action<IProperty> deleteCallback)
    {
        navigator = nav;
        layoutContent = layout;
        roomAdminScreenTransform = roomAdminScreen;
        roomScreenTransform = roomScreen;
        confirmationDialog = ConfirmPopUp;

        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.defaultProperyAdminScreenName : property.Name;
        editPropertyButton.onClick.AddListener(() => editCallback(property));
        deletePropertyButton.onClick.AddListener(() => deleteCallback(property));
        if (!property.HasRooms)
        {
            propertyButtonItem.onClick.AddListener(() => OpenRoomScreen(property.GetRoom(property.GetPropertyRoomID)));
            //propertyButtonItem.onClick.AddListener(() => OpenRoomReservationScreen(property.GetPropertyRoom()));
        }
        else
        {
            InstantiateRooms(property);
            propertyButtonItem.onClick.AddListener(() =>
            {
                roomsContentScrollView.gameObject.SetActive(roomsContentScrollView.gameObject.activeInHierarchy ? false : true);
                if (roomsContentScrollView.gameObject.activeInHierarchy)
                {
                    roomArrowLeft.SetActive(false);
                    roomArrowDown.SetActive(true);
                }
                else
                {
                    roomArrowLeft.SetActive(true);
                    roomArrowDown.SetActive(false);
                }
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
                roomButton.GetComponent<RoomButton>().Initialize(room, OpenRoomScreen, OpenRoomAdminScreen, DeleteRoom);
                roomButtons.Add(roomButton);
            }
        }
        nrOfRooms.text = string.IsNullOrEmpty(property.NrRooms) ? Constants.defaultProperyAdminScreenNrRooms : ("Nr. Camere: " + property.NrRooms);
        roomsContentScrollView.gameObject.SetActive(false);
    }

    public void AddRoomItem()
    {
        IRoom room = selectedProperty.AddRoom();
        OpenRoomAdminScreen(room);
    }

    public void DeleteRoom(IRoom selectedRoom)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = "Ștergeți camera?",
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () => {
                selectedProperty.DeleteRoom(selectedRoom.ID);
                ReservationDataManager.DeleteReservationsForRoom(selectedRoom.ID);
                InstantiateRooms(selectedProperty);
            },
            CancelCallback = null
        });
    }

    private void OpenRoomAdminScreen(IRoom room)
    {
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().SetCurrentPropertyRoom(selectedProperty, room);
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomScreen(IRoom room)
    {
        roomScreenTransform.GetComponent<RoomScreen>().UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
    }
}
