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
    private Transform propertyRoomScreenTransform = null;
    [SerializeField]
    private Transform roomScreenTransform = null;
    [SerializeField]
    private GameObject disponibilityPropertyPrefab = null;
    [SerializeField]
    private GameObject disponibilityRoomPrefab = null;
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
    private int selectedDropdownOption = 0;
    private int nrRooms = 0;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
    }
    
    public void Initialize()
    {
        SelectProperty(selectedDropdownOption);
    }
    
    public void ShowModalCalendar()
    {
        calendarScreen.OpenCallendar(startDate, SetNewDatePeriod);
    }
    
    private void SetNewDatePeriod(DateTime startDate, DateTime endDate)
    {
        this.startDate = startDate;
        this.endDate = endDate;
        SelectProperty(selectedDropdownOption);
    }
    
    private void SelectProperty(int optionIndex)
    {
        selectedDropdownOption = optionIndex;
        if (optionIndex != 0)
        {
            disponibilityDatePeriod.text = startDate.Day + " " + Constants.MonthNamesDict[startDate.Month] + " " + startDate.Year 
                                    + " - " + endDate.Day + " " + Constants.MonthNamesDict[endDate.Month] + " " + endDate.Year;
            reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();
            selectedProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(optionIndex).Key);
            foreach (var propertyItem in disponibilityScreenItemList)
            {
                Destroy(propertyItem);
            }
            GameObject propertyButton;
            PropertyButton buttonObject;
            if (selectedProperty.HasRooms)
            {
                propertyButton = Instantiate(disponibilityPropertyPrefab, filteredPropertiesContent);
                buttonObject = propertyButton.GetComponent<PropertyButton>();
                if (nrRooms == 0)
                {
                    Destroy(propertyButton);
                }
            }
            else
            {
                propertyButton = Instantiate(disponibilityPropertyPrefab, filteredPropertiesContent);
                buttonObject = propertyButton.GetComponent<PropertyButton>();
                buttonObject.InitializeDateTime(startDate, endDate);
                bool roomReservation = reservations.Any(r => r.RoomID == selectedProperty.GetPropertyRoom().ID);
                if (roomReservation)
                {
                    Destroy(propertyButton);
                }
            }
            string disponibleRooms = Constants.AVAILABLE_ROOMS + nrRooms;
            buttonObject.Initialize(selectedProperty, OpenRoomScreen, InstantiateRooms);
            disponibilityScreenItemList.Add(propertyButton);
            nrRooms = 0;
        }
        else
        {
            selectedProperty = null;
            UpdateDisponibilityContent(startDate, endDate);
        }
    }
    
    private void UpdateDisponibilityContent(DateTime startDate, DateTime endDate)
    {
        disponibilityDatePeriod.text = startDate.Day + " " + Constants.MonthNamesDict[startDate.Month] + " " + startDate.Year 
                                + " - " + endDate.Day + " " + Constants.MonthNamesDict[endDate.Month] + " " + endDate.Year;
        reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();
        propertyOptions = new Dictionary<string, Dropdown.OptionData>();
        propertyOptions.Add(String.Empty, new Dropdown.OptionData("Toate Proprietatile"));
    
        foreach (var propertyItem in disponibilityScreenItemList)
        {
            Destroy(propertyItem);
        }
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            PropertyButton buttonObject;
            if (property.HasRooms)
            {
                propertyButton = Instantiate(disponibilityPropertyPrefab, filteredPropertiesContent);
                buttonObject = propertyButton.GetComponent<PropertyButton>();
                if (nrRooms == 0)
                {
                    Destroy(propertyButton);
                }
            }
            else
            {
                propertyButton = Instantiate(disponibilityPropertyPrefab, filteredPropertiesContent);
                buttonObject = propertyButton.GetComponent<PropertyButton>();
                buttonObject.InitializeDateTime(startDate, endDate);
                bool roomReservation = reservations.Any(r => r.RoomID == property.GetPropertyRoom().ID);
                if (roomReservation)
                {
                    Destroy(propertyButton);
                }
            }
            string disponibleRooms = Constants.AVAILABLE_ROOMS + nrRooms;
            //buttonObject.Initialize(property, filteredPropertiesContent, true, disponibleRooms, null, OpenRoomScreen, OpenPropertyAdminScreen, DeleteProperty);
            propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
            disponibilityScreenItemList.Add(propertyButton);
            nrRooms = 0;
        }
        propertyDropdownList.options = propertyOptions.Values.ToList();
        propertyDropdownList.RefreshShownValue();
    }

    private void InstantiateRooms(IProperty property)
    {
        foreach (var propertyItem in disponibilityScreenItemList)
        {
            Destroy(propertyItem);
        }
        foreach (var roomButton in roomButtons)
        {
            Destroy(roomButton);
        }
        if (property != null)
        {
            foreach (var room in property.Rooms)
            {
                bool roomReservation = reservations.Any(r => r.RoomID == room.ID);
                if (!roomReservation)
                {
                    GameObject roomButton = Instantiate(disponibilityRoomPrefab, roomsContentScrollView);
                    RoomButton RoomObject = roomButton.GetComponent<RoomButton>();
                    RoomObject.InitializeDateTime(startDate, endDate);
                    RoomObject.Initialize(room, OpenRoomScreen);
                    roomButtons.Add(roomButton);
                    nrRooms++;
                }
            }
        }
    }

    private void OpenRoomScreen(IRoom room)
    {
        RoomScreen roomScreenScript = roomScreenTransform.GetComponent<RoomScreen>();
        roomScreenScript.UpdateRoomDetailsFields(room);
        navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
    }
}
