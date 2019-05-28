using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class DisponibilityScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private ModalCalendar calendarScreen = null;
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
    private Dropdown propertyDropdownList;
    [SerializeField]
    private Text disponibilityDatePeriod = null;

    private Dictionary<string, Dropdown.OptionData> propertyOptions;
    private List<GameObject> disponibilityScreenItemList = new List<GameObject>();
    private List<GameObject> roomButtons = new List<GameObject>();
    private List<IReservation> reservations = null;
    private IProperty selectedProperty;
    private Transform roomsContentScrollView = null;
    private IDateTimePeriod datePeriod = ReservationDataManager.DefaultPeriod();
    private DateTime startDate = DateTime.Today;
    private DateTime endDate = DateTime.Today;
    private int selectedDropdown = 0;
    private int nrRooms = 0;

    public void InitializeDisponibility()
    {
        SelectProperty(selectedDropdown);
    }

    public void SelectProperty(int optionIndex)
    {
        selectedDropdown = optionIndex;
        if (optionIndex != 0)
        {
            selectedProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(optionIndex).Key);
            foreach (var propertyItem in disponibilityScreenItemList)
            {
                Destroy(propertyItem);
            }
            GameObject propertyButton;
            if (selectedProperty.HasRooms)
            {
                propertyButton = Instantiate(propertyWithRoomsPrefab, filteredPropertiesContent);
                PropertyButton buttonObject = propertyButton.GetComponent<PropertyButton>();
                roomsContentScrollView = buttonObject.RoomsContentScrollView;
                roomButtons = buttonObject.RoomButtons;
                InstantiateRooms(selectedProperty);
                if (nrRooms == 0)
                {
                    Destroy(propertyButton);
                }
                roomsContentScrollView.gameObject.SetActive(true);
            }
            else
            {
                propertyButton = Instantiate(propertyWithoutRoomsPrefab, filteredPropertiesContent);
                IEnumerable<IReservation> roomReservation = reservations.Where(r => r.RoomID == selectedProperty.GetPropertyRoom().ID);
                if (roomReservation.Count() != 0)
                {
                    Destroy(propertyButton);
                }
            }
            string disponibleRooms = "Camere disponibile: " + nrRooms;
            propertyButton.GetComponent<PropertyButton>().Initialize(selectedProperty, filteredPropertiesContent, true, disponibleRooms, null, OpenRoomScreen, OpenPropertyAdminScreen, DeleteProperty);
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
        disponibilityDatePeriod.text = startDate.Day + " " + Constants.MonthNamesDict[startDate.Month] + " " + startDate.Year;
        if (endDate != startDate)
        {
            disponibilityDatePeriod.text += " - " + endDate.Day + " " + Constants.MonthNamesDict[endDate.Month] + " " + endDate.Year;
        }
        reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();

        foreach (var res in reservations)
        {
            Debug.Log(PropertyDataManager.GetProperty(res.PropertyID).Name + " " + PropertyDataManager.GetProperty(res.PropertyID).GetRoom(res.RoomID).Name);
        }
        propertyOptions = new Dictionary<string, Dropdown.OptionData>();
        propertyOptions.Add(String.Empty, new Dropdown.OptionData("Toate Proprietatile"));

        foreach (var propertyItem in disponibilityScreenItemList)
        {
            Destroy(propertyItem);
        }
        foreach (var property in PropertyDataManager.GetProperties())
        {
            GameObject propertyButton;
            if (property.HasRooms)
            {
                propertyButton = Instantiate(propertyWithRoomsPrefab, filteredPropertiesContent);
                PropertyButton buttonObject = propertyButton.GetComponent<PropertyButton>();
                roomsContentScrollView = buttonObject.RoomsContentScrollView;
                roomButtons = buttonObject.RoomButtons;
                InstantiateRooms(property);
                if (nrRooms == 0)
                {
                    Destroy(propertyButton);
                }
            }
            else
            {
                propertyButton = Instantiate(propertyWithoutRoomsPrefab, filteredPropertiesContent);
                IEnumerable<IReservation> roomReservation = reservations.Where(r => r.RoomID == property.GetPropertyRoom().ID);
                if (roomReservation.Count() != 0)
                {
                    Destroy(propertyButton);
                }
            }
            string disponibleRooms = "Camere disponibile: " + nrRooms;
            propertyButton.GetComponent<PropertyButton>().Initialize(property, filteredPropertiesContent, true, disponibleRooms, null, OpenRoomScreen, OpenPropertyAdminScreen, DeleteProperty);
            propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
            disponibilityScreenItemList.Add(propertyButton);
            nrRooms = 0;
        }
        propertyDropdownList.options = propertyOptions.Values.ToList();
        propertyDropdownList.RefreshShownValue();
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
                IEnumerable<IReservation> roomReservation = reservations.Where(r => r.RoomID == room.ID);
                if (roomReservation.Count() == 0)
                {
                    GameObject roomButton = Instantiate(roomPrefabButton, roomsContentScrollView);
                    roomButton.GetComponent<RoomButton>().Initialize(room, OpenRoomScreen, OpenRoomAdminScreen, DeleteRoom);
                    roomButtons.Add(roomButton);
                    nrRooms++;
                }
            }
        }
        roomsContentScrollView.gameObject.SetActive(false);
    }

    public void ShowModalCalendar()
    {
        //calendarScreen.Show(startDateTime, endDateTime, UpdateDisponibilityContent);
        startDate = new DateTime(2019, 5, 27);
        endDate = new DateTime(2019, 6, 1);
        SelectProperty(selectedDropdown);
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
                SelectProperty(selectedDropdown);
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
                SelectProperty(selectedDropdown);
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

