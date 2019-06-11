using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class DisponibilityScreen : MonoBehaviour
{
    public bool roomSelection { get; set; } = false;
    public List<IRoom> selectedRooms = new List<IRoom>();

    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private ReservationEditScreen reservationScreenComponent = null;
    [SerializeField]
    private ModalCalendarNew calendarScreen = null;
    [SerializeField]
    private Transform roomScreenTransform = null;
    [SerializeField]
    private GameObject propertyItemPrefab = null;
    [SerializeField]
    private GameObject roomItemPrefab = null;
    [SerializeField]
    private RectTransform filteredPropertiesContent = null;
    [SerializeField]
    private RectTransform disponibilityScrollView = null;
    [SerializeField]
    private Dropdown propertyDropdownList = null;
    [SerializeField]
    private Text disponibilityDatePeriod = null;
    [SerializeField]
    private Text availableRoomsNumber = null;
    [SerializeField]
    private Button backButton = null;

    private Dictionary<string, Dropdown.OptionData> propertyOptions;
    private Dictionary<string, int> propertyDropdownOptions;
    private List<GameObject> propertyItemList = new List<GameObject>();
    private List<GameObject> roomItemList = new List<GameObject>();
    private List<IReservation> reservations = null;
    private IReservation currentReservation;
    private IProperty selectedProperty;
    private DateTime startDate = DateTime.Today.Date;
    private DateTime endDate = DateTime.Today.AddDays(1).Date;
    private int lastDropdownOption = 0;
    private int nrRooms = 0;

    private void Awake()
    {
        backButton.onClick.AddListener(() => navigator.GoBack());
    }

    public void Initialize()
    {
        CheckRoomsSelection();
        SelectProperty(lastDropdownOption);
    }

    public void ShowModalCalendar()
    {
        calendarScreen.OpenCallendar(startDate, SetNewDatePeriod);
    }

    private void SetNewDatePeriod(DateTime startDate, DateTime endDate)
    {
        this.startDate = startDate;
        this.endDate = endDate;
        SelectProperty(lastDropdownOption);
    }

    public void SelectProperty(int optionIndex)
    {
        if (optionIndex == 0)
        {
            lastDropdownOption = optionIndex;
            InstantiateProperties();
        }
        else
        {
            lastDropdownOption = optionIndex;
            selectedProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(lastDropdownOption).Key);
            disponibilityDatePeriod.text = startDate.Day + " " + Constants.MonthNamesDict[startDate.Month] + " " + startDate.Year
                                    + " - " + endDate.Day + " " + Constants.MonthNamesDict[endDate.Month] + " " + endDate.Year;
            reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();
            InstantiateRooms(selectedProperty);
        }
    }

    private void InstantiateProperties()
    {
        int propertyIndex = 0;
        disponibilityDatePeriod.text = startDate.Day + "/" + startDate.Month + "/" + startDate.Year
                                + " - " + endDate.Day + "/" + endDate.Month + "/" + endDate.Year;
        reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();
        propertyOptions = new Dictionary<string, Dropdown.OptionData>();
        propertyDropdownOptions = new Dictionary<string, int>();
        propertyOptions.Add(String.Empty, new Dropdown.OptionData("Toate Proprietatile"));
        propertyDropdownOptions.Add("Toate Proprietatile", propertyIndex);
        propertyIndex++;
        foreach (var roomButton in roomItemList)
        {
            Destroy(roomButton);
        }
        foreach (var propertyItem in propertyItemList)
        {
            Destroy(propertyItem);
        }
        roomItemList = new List<GameObject>();
        propertyItemList = new List<GameObject>();
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton = Instantiate(propertyItemPrefab, filteredPropertiesContent);
            PropertyButton buttonObject = propertyButton.GetComponent<PropertyButton>();
            if (property.HasRooms)
            {
                int index = 0;
                foreach (var room in property.Rooms)
                {
                    bool roomReservation = reservations.Any(r => r.RoomID == room.ID);
                    if (roomReservation)
                    {
                        index++;
                    }
                    if (currentReservation != null)
                    {
                        if (roomReservation && reservations.Any(r => r.RoomID == currentReservation.ID))
                        {
                             index--;
                        }
                    }
                }
                if (index == property.Rooms.Count())
                {
                    Destroy(propertyButton);
                }
            }
            else
            {
                buttonObject.InitializeDateTime(startDate, endDate);
                bool roomReservation = reservations.Any(r => r.RoomID == property.GetPropertyRoom().ID);
                if (roomReservation)
                {
                    Destroy(propertyButton);
                }
            }
            buttonObject.Initialize(property, SelectDropdownProperty, SelectDropdownProperty);
            propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
            propertyDropdownOptions.Add(property.ID, propertyIndex);
            propertyItemList.Add(propertyButton);
            propertyIndex++;
        }
        propertyDropdownList.options = propertyOptions.Values.ToList();
        propertyDropdownList.RefreshShownValue();
    }

    private void InstantiateRooms(IProperty property)
    {
        foreach (var propertyItem in propertyItemList)
        {
            Destroy(propertyItem);
        }
        foreach (var roomButton in roomItemList)
        {
            Destroy(roomButton);
            nrRooms = 0;
        }
        roomItemList = new List<GameObject>();
        propertyItemList = new List<GameObject>();
        if (property != null)
        {
            if (property.HasRooms)
            {
                foreach (var room in property.Rooms)
                {
                    bool roomReservation = reservations.Any(r => r.RoomID == room.ID);
                    if (!roomReservation)
                    {
                        GameObject roomButton = Instantiate(roomItemPrefab, filteredPropertiesContent);
                        RoomButton roomObject = roomButton.GetComponent<RoomButton>();
                        roomObject.InitializeDateTime(startDate, endDate);
                        roomObject.Initialize(room, this, OpenRoomScreen);
                        roomItemList.Add(roomButton);
                        nrRooms++;
                    }
                }
                availableRoomsNumber.text = Constants.AVAILABLE_ROOMS + nrRooms;
            }
            else
            {
                GameObject propertyButton = Instantiate(propertyItemPrefab, filteredPropertiesContent);
                PropertyButton buttonObject = propertyButton.GetComponent<PropertyButton>();
                buttonObject.Initialize(property, OpenRoomScreen, SelectDropdownProperty);
                propertyItemList.Add(propertyButton);
            }
        }
    }

    public void CheckRoomsSelection()
    {
        if (selectedRooms.Count() == 0)
        {
            roomSelection = false;
            StartCoroutine(ExpandFooterBar(new Vector2(disponibilityScrollView.sizeDelta.x, 1354)));
        }
        else
        {
            roomSelection = true;
            StartCoroutine(ExpandFooterBar(new Vector2(disponibilityScrollView.sizeDelta.x, 1194)));
        }
    }

    public void MakeReservation()
    {
        reservationScreenComponent.OpenAddReservation(startDate, endDate, selectedRooms, null);
    }

    public void CancelSelection()
    {
        selectedRooms = new List<IRoom>();
        CheckRoomsSelection();
        foreach (var room in roomItemList)
        {
            RoomButton roomObject = room.GetComponent<RoomButton>();
            if (roomObject.selected)
            {
                roomObject.SelectToggleMark();
            }
        }
    }

    private void SelectDropdownProperty(IProperty property)
    {
        lastDropdownOption = propertyDropdownOptions[property.ID];
        propertyDropdownList.value = lastDropdownOption;
    }

    private void SelectDropdownProperty(IRoom propertyRoom)
    {
        lastDropdownOption = propertyDropdownOptions[PropertyDataManager.GetProperty(propertyRoom.PropertyID).ID];
        propertyDropdownList.value = lastDropdownOption;
    }

    private void OpenRoomScreen(IRoom room)
    {
        if (!roomSelection)
        {
            RoomScreen roomScreenScript = roomScreenTransform.GetComponent<RoomScreen>();
            roomScreenScript.UpdateRoomDetailsFields(room);
            navigator.GoTo(roomScreenTransform.GetComponent<NavScreen>());
        }
        CheckRoomsSelection();
    }

    public void OpenDisponibility(DateTime start, DateTime end, List<IRoom> selectedRooms)
    {
        navigator.GoTo(this.GetComponent<NavScreen>());
        startDate = start;
        endDate = end;
        this.selectedRooms = selectedRooms;
        SelectDropdownProperty(selectedRooms[0]);
    }

    private IEnumerator ExpandFooterBar(Vector2 endSize)
    {
        float currentTime = 0;
        while (currentTime < 0.4f)
        {
            currentTime += Time.deltaTime;
            disponibilityScrollView.sizeDelta = Vector2.Lerp(disponibilityScrollView.sizeDelta, endSize, currentTime / 0.4f);
            yield return null;
        }
        disponibilityScrollView.sizeDelta = endSize;
    }
}
