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
    private PropertyRoomScreen propertyRoomScreen = null;
    [SerializeField]
    private RoomAdminScreen roomAdminScreen = null;
    [SerializeField]
    private PropertyAdminScreen propertyAdminScreenTransform = null;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private Transform reservationsContent = null;
    [SerializeField]
    private Text roomScreenTitle = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private AspectRatioFitter backgroundImageAspectFitter = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private AspectRatioFitter propertyImageAspectFitter = null;
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

    public void UpdateDateTime(DateTime start, DateTime end)
    {
        dateTimeStart = start;
        dateTimeEnd = end;
    }

    public void UpdateRoomDetailsFields(IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        //dayDateTime = date;
        //roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        UpdateCurrentRoomDetailsFields();
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
        .Any(r => r.ContainsRoom(currentRoom.ID));
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
            backgroundImage.sprite = (Sprite)ImageDataManager.BlurPropertyPhotos[currentProperty.ID];
            backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[Constants.defaultPropertyPicture];
            backgroundImage.gameObject.SetActive(false);
        }
        backgroundImageAspectFitter.aspectRatio = propertyImageAspectFitter.aspectRatio = (float)backgroundImage.sprite.texture.width/backgroundImage.sprite.texture.height;

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
        reservationScreen.OpenAddReservation(dateTimeStart, dateTimeEnd, currentRoom, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID)));
    }

    public void InstantiateReservations()
    {
        scrollRectComponent.ResetAll();
        List<IReservation> orderedRoomReservationList = ReservationDataManager.GetActiveRoomReservations(currentRoom.ID)
                                                    .OrderBy(res => res.Period.Start).ToList();

        foreach (var reservationButton in reservationButtonList)
        {
            DestroyImmediate(reservationButton);
        }

        foreach (var reservation in orderedRoomReservationList)
        {
            GameObject reservationButton = Instantiate(reservationPrefabButton, reservationsContent);
            reservationButton.GetComponent<ReservationItem>().Initialize(reservation, () => reservationScreen.OpenEditReservation(reservation, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID))));
            reservationButtonList.Add(reservationButton);
        }
        scrollRectComponent.Init();
    }

    public void OnHidingSetProperty()
    {
        propertyRoomScreen.SetCurrentProperty(currentProperty);
    }

    private void DeleteReservation(GameObject reservationButton)
    {
        reservationButtonList.Remove(reservationButton);
        DestroyImmediate(reservationButton);
    }

    private void OpenPropertyAdminScreen()
    {
        propertyAdminScreenTransform.SetCurrentProperty(currentProperty);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen()
    {
        navigator.GoTo(roomAdminScreen.GetComponent<NavScreen>());
        roomAdminScreen.SetCurrentPropertyRoom(currentRoom); 
    }
}
