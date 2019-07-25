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
    private ScrollRect roomScreenScrollRect = null;
    [SerializeField]
    private RectTransform reservationsContent = null;
    [SerializeField]
    private Text roomScreenTitle = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private AspectRatioFitter propertyImageAspectFitter = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private AspectRatioFitter backgroundImageAspectFitter = null;
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
    private float tempPosition = 1;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
        //editButton.onClick.AddListener(() => EditButton());
    }

    /// <summary>
    /// set the position of the scroll rect to the top of the screen
    /// </summary>
    public void ScrollToTop()
    {
        tempPosition = 1;
    }

    /// <summary>
    /// set new date period
    /// </summary>
    /// <param name="start">start of date period</param>
    /// <param name="end">end of date period</param>
    public void UpdateDateTime(DateTime start, DateTime end)
    {
        dateTimeStart = start;
        dateTimeEnd = end;
    }

    /// <summary>
    /// set the current room and property
    /// call the function to update details
    /// </summary>
    /// <param name="room">current rooms</param>
    public void UpdateRoomDetailsFields(IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        currentRoom = room;
        //dayDateTime = date;
        //roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        ScrollToTop();
        UpdateCurrentRoomDetailsFields();
    }

    /// <summary>
    /// update room details
    /// call function ro instantiate reservations
    /// </summary>
    public void UpdateCurrentRoomDetailsFields()
    {
        if (currentProperty.HasRooms)
        {
            roomScreenTitle.text = $"Camera {currentRoom.Name}" ?? Constants.NEW_ROOM;
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
        propertyImageAspectFitter.aspectRatio = backgroundImageAspectFitter.aspectRatio = (float)backgroundImage.sprite.texture.width/backgroundImage.sprite.texture.height;

        //roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        InstantiateReservations();
    }

    /// <summary>
    /// on edit button
    /// edit selected room, or property
    /// </summary>
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

    /// <summary>
    /// open reservation edit screen
    /// </summary>
    public void CreateNewReservation()
    {
        reservationScreen.OpenAddReservation(dateTimeStart, dateTimeEnd, currentRoom, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID)));
    }

    /// <summary>
    /// instantiate current room reservations
    /// </summary>
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
            reservationButton.GetComponent<ReservationButton>().Initialize(reservation, () => reservationScreen.OpenEditReservation(reservation, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID))), false);
            reservationButtonList.Add(reservationButton);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(reservationsContent);
        Canvas.ForceUpdateCanvases();
        roomScreenScrollRect.verticalNormalizedPosition = tempPosition;
        if (roomScreenScrollRect.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
    }

    /// <summary>
    /// on Hiding screen
    /// reset screen details
    /// </summary>
    public void OnHidingSetProperty()
    {
        propertyRoomScreen.SetCurrentProperty(currentProperty);
    }

    /// <summary>
    /// obsolete
    /// delete selected reservation
    /// </summary>
    /// <param name="reservationButton"></param>
    private void DeleteReservation(GameObject reservationButton)
    {
        reservationButtonList.Remove(reservationButton);
        DestroyImmediate(reservationButton);
    }

    /// <summary>
    /// open edit property screen
    /// </summary>
    /// <param name="property">current property</param>
    private void OpenPropertyAdminScreen()
    {
        propertyAdminScreenTransform.OpenPropertyAdminScreen(currentProperty);
    }

    /// <summary>
    /// open edit room screen
    /// </summary>
    /// <param name="room"> selected room</param>
    private void OpenRoomAdminScreen()
    {
        roomAdminScreen.SetCurrentPropertyRoom(currentRoom); 
    }
}
