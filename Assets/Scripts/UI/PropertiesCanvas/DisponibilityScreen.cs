using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class DisponibilityScreen : MonoBehaviour
{
    public string OpenRoomDropdown { get; set; }

    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private ModalCalendarNew calendarScreen = null;
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
    private RectTransform filteredPropertiesContent = null;
    [SerializeField]
    private Dropdown propertyDropdownList = null;
    [SerializeField]
    private Text disponibilityDatePeriod = null;
    [SerializeField]
    private Button backButton = null;

    private Dictionary<string, Dropdown.OptionData> propertyOptions;
    private List<GameObject> disponibilityScreenItemList = new List<GameObject>();
    private List<GameObject> roomButtons = new List<GameObject>();
    private List<IReservation> reservations = null;
    private IProperty selectedProperty;
    private Transform roomsContentScrollView = null;
    private IDateTimePeriod datePeriod = ReservationDataManager.DefaultPeriod();
    private DateTime startDate = DateTime.Today.Date;
    private DateTime endDate = DateTime.Today.AddDays(1).Date;
    private int selectedDropdown = 0;
    private int nrRooms = 0;

    //private void Awake()
    //{
    //    backButton.onClick.AddListener(() => navigator.GoBack());
    //}
    //
    //public void Initialize()
    //{
    //    SelectProperty(selectedDropdown);
    //}
    //
    //public void ShowModalCalendar()
    //{
    //    calendarScreen.OpenCallendar(startDate, SetNewDatePeriod);
    //}
    //
    //private void SetNewDatePeriod(DateTime startDate, DateTime endDate)
    //{
    //    this.startDate = startDate;
    //    this.endDate = endDate;
    //    SelectProperty(selectedDropdown);
    //}
    //
    //private void SelectProperty(int optionIndex)
    //{
    //    selectedDropdown = optionIndex;
    //    if (optionIndex != 0)
    //    {
    //        disponibilityDatePeriod.text = startDate.Day + " " + Constants.MonthNamesDict[startDate.Month] + " " + startDate.Year 
    //                                + " - " + endDate.Day + " " + Constants.MonthNamesDict[endDate.Month] + " " + endDate.Year;
    //        reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();
    //        selectedProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(optionIndex).Key);
    //        foreach (var propertyItem in disponibilityScreenItemList)
    //        {
    //            Destroy(propertyItem);
    //        }
    //        GameObject propertyButton;
    //        PropertyButton buttonObject;
    //        if (selectedProperty.HasRooms)
    //        {
    //            propertyButton = Instantiate(propertyWithRoomsPrefab, filteredPropertiesContent);
    //            buttonObject = propertyButton.GetComponent<PropertyButton>();
    //            roomsContentScrollView = buttonObject.RoomsContentScrollView;
    //            roomButtons = buttonObject.RoomButtons;
    //            buttonObject.OpenRoomContents();
    //            InstantiateRooms(selectedProperty);
    //            if (nrRooms == 0)
    //            {
    //                Destroy(propertyButton);
    //            }
    //            buttonObject.OpenRoomContents();
    //        }
    //        else
    //        {
    //            propertyButton = Instantiate(propertyWithoutRoomsPrefab, filteredPropertiesContent);
    //            buttonObject = propertyButton.GetComponent<PropertyButton>();
    //            buttonObject.InitializeDateTime(startDate, endDate);
    //            bool roomReservation = reservations.Any(r => r.RoomID == selectedProperty.GetPropertyRoom().ID);
    //            if (roomReservation)
    //            {
    //                Destroy(propertyButton);
    //            }
    //        }
    //        string disponibleRooms = Constants.AVAILABLE_ROOMS + nrRooms;
    //        buttonObject.Initialize(selectedProperty, filteredPropertiesContent, true, disponibleRooms, null, OpenRoomScreen, OpenPropertyAdminScreen, DeleteProperty);
    //        disponibilityScreenItemList.Add(propertyButton);
    //        nrRooms = 0;
    //    }
    //    else
    //    {
    //        selectedProperty = null;
    //        UpdateDisponibilityContent(startDate, endDate);
    //    }
    //}
    //
    //private void UpdateDisponibilityContent(DateTime startDate, DateTime endDate)
    //{
    //    disponibilityDatePeriod.text = startDate.Day + " " + Constants.MonthNamesDict[startDate.Month] + " " + startDate.Year 
    //                            + " - " + endDate.Day + " " + Constants.MonthNamesDict[endDate.Month] + " " + endDate.Year;
    //    reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();
    //    propertyOptions = new Dictionary<string, Dropdown.OptionData>();
    //    propertyOptions.Add(String.Empty, new Dropdown.OptionData("Toate Proprietatile"));
    //
    //    foreach (var propertyItem in disponibilityScreenItemList)
    //    {
    //        Destroy(propertyItem);
    //    }
    //    foreach (var property in PropertyDataManager.GetProperties())
    //    {
    //        GameObject propertyButton;
    //        PropertyButton buttonObject;
    //        if (property.HasRooms)
    //        {
    //            propertyButton = Instantiate(propertyWithRoomsPrefab, filteredPropertiesContent);
    //            buttonObject = propertyButton.GetComponent<PropertyButton>();
    //            roomsContentScrollView = buttonObject.RoomsContentScrollView;
    //            roomButtons = buttonObject.RoomButtons;
    //            InstantiateRooms(property);
    //            if (nrRooms == 0)
    //            {
    //                Destroy(propertyButton);
    //            }
    //            if (property.ID == OpenRoomDropdown)
    //            {
    //                buttonObject.OpenRoomContents();
    //                OpenRoomDropdown = string.Empty;
    //            }
    //            else
    //            {
    //                roomsContentScrollView.gameObject.SetActive(false);
    //            }
    //        }
    //        else
    //        {
    //            propertyButton = Instantiate(propertyWithoutRoomsPrefab, filteredPropertiesContent);
    //            buttonObject = propertyButton.GetComponent<PropertyButton>();
    //            buttonObject.InitializeDateTime(startDate, endDate);
    //            bool roomReservation = reservations.Any(r => r.RoomID == property.GetPropertyRoom().ID);
    //            if (roomReservation)
    //            {
    //                Destroy(propertyButton);
    //            }
    //        }
    //        string disponibleRooms = Constants.AVAILABLE_ROOMS + nrRooms;
    //        buttonObject.Initialize(property, filteredPropertiesContent, true, disponibleRooms, null, OpenRoomScreen, OpenPropertyAdminScreen, DeleteProperty);
    //        propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
    //        disponibilityScreenItemList.Add(propertyButton);
    //        nrRooms = 0;
    //    }
    //    propertyDropdownList.options = propertyOptions.Values.ToList();
    //    propertyDropdownList.RefreshShownValue();
    //}

    //private void InstantiateRooms(IProperty property)
    //{
    //    foreach (var roomButton in roomButtons)
    //    {
    //        Destroy(roomButton);
    //    }
    //    if (property != null)
    //    {
    //        foreach (var room in property.Rooms)
    //        {
    //            bool roomReservation = reservations.Any(r => r.RoomID == room.ID);
    //            if (!roomReservation)
    //            {
    //                GameObject roomButton = Instantiate(roomPrefabButton, roomsContentScrollView);
    //                RoomButton RoomObject = roomButton.GetComponent<RoomButton>();
    //                RoomObject.InitializeDateTime(startDate, endDate);
    //                RoomObject.Initialize(room, OpenRoomScreen, OpenRoomAdminScreen, DeleteRoom);
    //                roomButtons.Add(roomButton);
    //                nrRooms++;
    //            }
    //        }
    //    }
    //}

    //private void DeleteProperty(IProperty property)
    //{
    //    confirmationDialog.Show(new ConfirmationDialogOptions
    //    {
    //        Message = Constants.DELETE_PROPERTY,
    //        ConfirmText = Constants.DELETE_CONFIRM,
    //        CancelText = Constants.DELETE_CANCEL,
    //        ConfirmCallback = () =>
    //        {
    //            PropertyDataManager.DeleteProperty(property.ID);
    //            ReservationDataManager.DeleteReservationsForProperty(property.ID);
    //            SelectProperty(selectedDropdown);
    //        },
    //        CancelCallback = null
    //    });
    //}
    //
    //private void DeleteRoom(IRoom selectedRoom)
    //{
    //    confirmationDialog.Show(new ConfirmationDialogOptions
    //    {
    //        Message = Constants.DELETE_ROOM,
    //        ConfirmText = Constants.DELETE_CONFIRM,
    //        CancelText = Constants.DELETE_CANCEL,
    //        ConfirmCallback = () =>
    //        {
    //            IProperty selectedProperty = PropertyDataManager.GetProperty(selectedRoom.PropertyID);
    //            selectedProperty.DeleteRoom(selectedRoom.ID);
    //            ReservationDataManager.DeleteReservationsForRoom(selectedRoom.ID);
    //            SelectProperty(selectedDropdown);
    //        },
    //        CancelCallback = null
    //    });
    //}

    private void OpenPropertyAdminScreen(IProperty property)
    {
        PropertyAdminScreen propertyAdminScreenScript = propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>();
        propertyAdminScreenScript.SetCurrentProperty(property);
        //propertyAdminScreenScript.disponibilityScreen = this;
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomAdminScreen(IRoom room)
    {
        RoomAdminScreen roomAdminScreenScript = roomAdminScreenTransform.GetComponent<RoomAdminScreen>();
        roomAdminScreenScript.SetCurrentPropertyRoom(room);
        //roomAdminScreenScript.disponibilityScreen = this;
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenRoomScreen(IRoom room)
    {
        RoomScreen roomScreenScript = roomScreenTransform.GetComponent<RoomScreen>();
        roomScreenScript.UpdateRoomDetailsFields(room);
        //roomScreenScript.disponibilityScreen = this;
        navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
    }
}
