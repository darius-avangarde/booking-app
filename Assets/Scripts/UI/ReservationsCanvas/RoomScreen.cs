using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomScreen : MonoBehaviour
{
    public PropertiesScreen propertiesScreen { get; set; }
    public DisponibilityScreen disponibilityScreen { get; set; }

    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private ReservationEditScreen reservationScreen = null;
    [SerializeField]
    private Transform roomAdminScreenTransform = null;
    [SerializeField]
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private GameObject reservationPrefabButton = null;
    [SerializeField]
    private Transform reservationsContent = null;
    [SerializeField]
    private Text propertyRoomScreenTitle = null;
    [SerializeField]
    private Button backButton;
    //[SerializeField]
    //private Text roomDetails = null;
    private List<GameObject> reservationButtonList = new List<GameObject>();
    private DateTime dayDateTime = DateTime.Today.Date;
    private IProperty currentProperty;
    private IRoom currentRoom;
    private IReservation currentReservation;

    private void Awake()
    {
        backButton.onClick.AddListener(() => OpenPropertiesScreen());
    }

    public void UpdateRoomDetailsFields(IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        //dayDateTime = date;
        currentRoom = room;
        if (currentProperty.HasRooms)
        {
            propertyRoomScreenTitle.text = room.Name ?? Constants.NEW_ROOM;
        }
        else
        {
            propertyRoomScreenTitle.text = currentProperty.Name ?? Constants.NEW_PROPERTY;
        }
        //roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        InstantiateReservations();
    }

    public void UpdateCurrentRoomDetailsFields()
    {
        if (currentProperty.HasRooms)
        {
            propertyRoomScreenTitle.text = currentRoom.Name ?? Constants.NEW_ROOM;
        }
        else
        {
            propertyRoomScreenTitle.text = currentProperty.Name ?? Constants.NEW_PROPERTY;
        }
        //roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        InstantiateReservations();
    }

    public void EditButton()
    {
        if (currentProperty.HasRooms)
        {
            OpenRoomAdminScreen();
        }
        else
        {
            OpenPropertyAdminScreen();
        }
    }

    public void CreateNewReservation()
    {
        reservationScreen.OpenAddReservation(currentRoom, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID)));
    }

    public void InstantiateReservations()
    {
        List<IReservation> orderedRoomReservationList = ReservationDataManager.GetActiveRoomReservations(currentRoom.ID)
                                                    .OrderBy(res => res.Period.Start).ToList();

        foreach (var reservationButton in reservationButtonList)
        {
            Destroy(reservationButton);
        }

        foreach (var reservation in orderedRoomReservationList)
        {
            GameObject reservationButton = Instantiate(reservationPrefabButton, reservationsContent);
            reservationButton.GetComponent<ReservationItem>().Initialize(reservation, () => reservationScreen.OpenEditReservation(reservation, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID))), DeleteReservation);
            reservationButtonList.Add(reservationButton);
        }
    }

    private void DeleteReservation(IReservation reservation)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_DIALOG,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                ReservationDataManager.DeleteReservation(reservation.ID);
                InstantiateReservations();
            },
            CancelCallback = null
        });
    }

    private void OpenPropertyAdminScreen()
    {
        PropertyAdminScreen propertyAdminScreenScript = propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>();
        propertyAdminScreenScript.SetCurrentProperty(currentProperty);
        propertyAdminScreenScript.propertiesScreen = propertiesScreen;
        propertyAdminScreenScript.disponibilityScreen = disponibilityScreen;
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen()
    {
        RoomAdminScreen roomAdminScreenScript = roomAdminScreenTransform.GetComponent<RoomAdminScreen>();
        roomAdminScreenScript.SetCurrentPropertyRoom(currentRoom);
        roomAdminScreenScript.propertiesScreen = propertiesScreen;
        roomAdminScreenScript.disponibilityScreen = disponibilityScreen;
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenPropertiesScreen()
    {
        if (propertiesScreen != null)
        {
            propertiesScreen.OpenRoomDropdown = currentProperty.ID;
            propertiesScreen.Initialize();
            propertiesScreen = null;
            navigator.GoBack();
        }
        if (disponibilityScreen != null)
        {
            disponibilityScreen.OpenRoomDropdown = currentProperty.ID;
            disponibilityScreen.Initialize();
            disponibilityScreen = null;
            navigator.GoBack();
        }
    }
}
