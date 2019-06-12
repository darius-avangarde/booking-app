using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomScreen : MonoBehaviour
{
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
    private Transform reservationsContent = null;
    [SerializeField]
    private Text roomScreenTitle = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private Image disponibilityMarker = null;
    [SerializeField]
    private GameObject reservationPrefabButton = null;
    [SerializeField]
    private Button backButton = null;
    [SerializeField]
    private Button editButton = null;
    //[SerializeField]
    //private Text roomDetails = null;
    private List<GameObject> reservationButtonList = new List<GameObject>();
    private DateTime dateTimeStart = DateTime.Today.Date;
    private DateTime dateTimeEnd = DateTime.Today.AddDays(1).Date;
    private IProperty currentProperty;
    private IRoom currentRoom;
    private IReservation currentReservation;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        editButton.onClick.AddListener(() => EditButton());
    }

    public void UpdateRoomDetailsFields(IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        //dayDateTime = date;
        //roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        //UpdateCurrentRoomDetailsFields();
    }

    public void UpdateCurrentRoomDetailsFields()
    {
        if (currentProperty.HasRooms)
        {
            roomScreenTitle.text = currentRoom.Name ?? Constants.NEW_ROOM;
        }
        else
        {
            roomScreenTitle.text = currentProperty.Name ?? Constants.NEW_PROPERTY;
        }
        bool reservations = ReservationDataManager.GetReservationsBetween(dateTimeStart, dateTimeEnd)
        .Any(r => r.RoomID == currentRoom.ID);
        if (reservations)
        {
            disponibilityMarker.color = Constants.reservedUnavailableItemColor;
        }
        else
        {
            disponibilityMarker.color = Constants.availableItemColor;
        }
        if (ImageDataManager.PropertyPhotos.ContainsKey(currentProperty.ID))
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
            backgroundImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[currentProperty.ID];
            backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[Constants.defaultPropertyPicture];
            backgroundImage.gameObject.SetActive(false);
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
            reservationButton.GetComponent<ReservationItem>().Initialize(reservation, () => reservationScreen.OpenEditReservation(reservation, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID))));
            reservationButtonList.Add(reservationButton);
        }
    }

    private void DeleteReservation(GameObject reservationButton)
    {
        reservationButtonList.Remove(reservationButton);
        Destroy(reservationButton);
    }

    private void OpenPropertyAdminScreen()
    {
        PropertyAdminScreen propertyAdminScreenScript = propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>();
        propertyAdminScreenScript.SetCurrentProperty(currentProperty);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen()
    {
        RoomAdminScreen roomAdminScreenScript = roomAdminScreenTransform.GetComponent<RoomAdminScreen>();
        roomAdminScreenScript.SetCurrentPropertyRoom(currentRoom);
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }
}
