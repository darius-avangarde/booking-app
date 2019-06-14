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
    private RoomScreen roomScreen = null;
    [SerializeField]
    private GameObject propertyItemPrefab = null;
    [SerializeField]
    private GameObject roomItemPrefab = null;
    [SerializeField]
    private RectTransform filteredPropertiesContent = null;
    [SerializeField]
    private RectTransform disponibilityScrollView = null;
    [SerializeField]
    private RectTransform headerBar = null;
    [SerializeField]
    private RectTransform footerBar = null;
    [SerializeField]
    private Dropdown propertyDropdownList = null;
    [SerializeField]
    private Image backgroundImage = null;
    [SerializeField]
    private Text disponibilityDatePeriod = null;
    [SerializeField]
    private Text availableRoomsNumber = null;
    [SerializeField]
    private Button backButton = null;

    private Action<DateTime, DateTime, List<IRoom>> selectionCallback;
    private Dictionary<string, Dropdown.OptionData> propertyOptions;
    private Dictionary<string, int> propertyDropdownOptions;
    private List<GameObject> propertyItemList = new List<GameObject>();
    private List<GameObject> roomItemList = new List<GameObject>();
    private List<IReservation> reservations = null;
    private IReservation currentReservation;
    private IProperty selectedProperty;
    private DateTime startDate = DateTime.Today.Date;
    private DateTime endDate = DateTime.Today.AddDays(1).Date;
    private float disponibilityHeight;
    private int lastDropdownOption = 0;
    private int nrRooms = 0;
    private bool fromReservation = false;
    private bool shouldSelectRooms = false;
    private bool vibrate = true;

    private void Awake()
    {
        backButton.onClick.AddListener(() => BackButtonFunction());
        disponibilityHeight = disponibilityScrollView.offsetMin.y;
        propertyDropdownList.onValueChanged.AddListener(SelectProperty);
    }

    public void SetDefaultDate()
    {
        startDate = DateTime.Today.Date;
        endDate = DateTime.Today.AddDays(1).Date;
    }

    public void Initialize()
    {
        lastDropdownOption = 0;
        propertyDropdownList.value = lastDropdownOption;
        SelectProperty(lastDropdownOption);
    }

    public void ShowModalCalendar()
    {
        calendarScreen.OpenCallendar(startDate, endDate, SetNewDatePeriod, true);
    }

    private void SetNewDatePeriod(DateTime startDate, DateTime endDate)
    {
        this.startDate = startDate;
        this.endDate = endDate;
        if (selectedProperty != null)
        {
            shouldSelectRooms = true;
        }
        InstantiateProperties();
        if (lastDropdownOption == 0)
        {
            selectedProperty = null;
        }
        if (selectedProperty != null)
        {
            SelectDropdownProperty(selectedProperty);
        }
    }

    public void SelectProperty(int optionIndex)
    {
        if (!shouldSelectRooms)
        {
            CancelSelection();
        }
        if (optionIndex == 0)
        {
            lastDropdownOption = optionIndex;
            InstantiateProperties();
        }
        else
        {
            lastDropdownOption = optionIndex;
            selectedProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(lastDropdownOption).Key);
            disponibilityDatePeriod.text = startDate.Day + "/" + startDate.Month + "/" + startDate.Year
                                + " - " + endDate.Day + "/" + endDate.Month + "/" + endDate.Year;
            if (currentReservation != null)
            {
                reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).Where(r => r != currentReservation).ToList();
            }
            else
            {
                reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();
            }
            InstantiateRooms(selectedProperty);
        }
        shouldSelectRooms = false;
    }

    private void InstantiateProperties()
    {
        StartCoroutine(ExpandHeaderBar(new Vector2(headerBar.sizeDelta.x, 450), new Vector2(disponibilityScrollView.offsetMax.x, -450)));
        if (!shouldSelectRooms)
        {
            CancelSelection();
        }
        int propertyIndex = 0;
        backgroundImage.gameObject.SetActive(false);
        disponibilityDatePeriod.text = startDate.Day + "/" + startDate.Month + "/" + startDate.Year
                                + " - " + endDate.Day + "/" + endDate.Month + "/" + endDate.Year;
        if (currentReservation != null)
        {
            reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).Where(r => r != currentReservation).ToList();
        }
        else
        {
            reservations = ReservationDataManager.GetReservationsBetween(startDate, endDate).ToList();
        }
        propertyOptions = new Dictionary<string, Dropdown.OptionData>();
        propertyDropdownOptions = new Dictionary<string, int>();
        propertyOptions.Add(String.Empty, new Dropdown.OptionData("Toate Proprietățile"));
        propertyDropdownOptions.Add("Toate Proprietățile", propertyIndex);
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
                    bool roomReservation = reservations.Any(r => r.ContainsRoom(room.ID));
                    if (roomReservation)
                    {
                        index++;
                    }
                }
                if (index == property.Rooms.Count())
                {
                    if (selectedProperty != null && selectedProperty.ID == property.ID)
                    {
                        buttonObject.Initialize(property, SelectDropdownProperty, SelectDropdownProperty, null);
                        propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
                        propertyDropdownOptions.Add(property.ID, propertyIndex);
                        propertyItemList.Add(propertyButton);
                        propertyIndex++;
                    }
                    else
                    {
                        Destroy(propertyButton);
                    }
                }
                else
                {
                    buttonObject.Initialize(property, SelectDropdownProperty, SelectDropdownProperty, null);
                    propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
                    propertyDropdownOptions.Add(property.ID, propertyIndex);
                    propertyItemList.Add(propertyButton);
                    propertyIndex++;
                }
            }
            else
            {
                bool roomReservation = reservations.Any(r => r.ContainsRoom(property.GetPropertyRoom().ID));
                if (roomReservation)
                {
                    if (selectedProperty != null && selectedProperty.ID == property.ID)
                    {
                        propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
                        propertyDropdownOptions.Add(property.ID, propertyIndex);
                        propertyIndex++;
                        Destroy(propertyButton);
                    }
                    else
                    {
                        Destroy(propertyButton);
                    }
                }
                else
                {
                    buttonObject.InitializeDateTime(startDate, endDate);
                    if (fromReservation)
                    {
                        buttonObject.Initialize(property, SelectDropdownProperty, SelectDropdownProperty, selectionCallback);
                    }
                    else
                    {
                        buttonObject.Initialize(property, SelectDropdownProperty, SelectDropdownProperty, null);
                    }
                    propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
                    propertyDropdownOptions.Add(property.ID, propertyIndex);
                    propertyItemList.Add(propertyButton);
                    propertyIndex++;
                }
            }
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
        }
        roomItemList = new List<GameObject>();
        propertyItemList = new List<GameObject>();
        if (ImageDataManager.PropertyPhotos.ContainsKey(property.ID))
        {
            backgroundImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[property.ID];
            backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            backgroundImage.gameObject.SetActive(false);
        }
        if (property != null)
        {
            if (property.HasRooms)
            {
                nrRooms = 0;
                StartCoroutine(ExpandHeaderBar(new Vector2(headerBar.sizeDelta.x, 560), new Vector2(disponibilityScrollView.offsetMax.x, -560)));
                foreach (var room in property.Rooms)
                {
                    bool roomReservation = reservations.Any(r => r.ContainsRoom(room.ID));
                    if (!roomReservation)
                    {
                        GameObject roomButton = Instantiate(roomItemPrefab, filteredPropertiesContent);
                        RoomButton roomObject = roomButton.GetComponent<RoomButton>();
                        roomObject.InitializeDateTime(startDate, endDate);
                        if (fromReservation)
                        {
                            roomObject.Initialize(room, this, OpenRoomScreen, selectionCallback);
                        }
                        else
                        {
                            roomObject.Initialize(room, this, OpenRoomScreen, null);
                        }
                        roomItemList.Add(roomButton);
                        nrRooms++;
                    }
                    else
                    {
                        //if room is reserved in the given date time period, remove it from selected rooms
                        if (selectedRooms.Any(r => r.ID == room.ID))
                        {
                            selectedRooms.Remove(room);
                        }
                    }
                }
                availableRoomsNumber.text = Constants.AVAILABLE_ROOMS + nrRooms;
                if (nrRooms == 0)
                {
                    availableRoomsNumber.color = Constants.reservedUnavailableItemColor;
                }
                else
                {
                    availableRoomsNumber.color = Constants.defaultTextColor;
                }
                CheckRoomsSelection();
            }
            else
            {
                if (!reservations.Any(r => r.ContainsRoom(property.GetPropertyRoom().ID)))
                {
                    StartCoroutine(ExpandHeaderBar(new Vector2(headerBar.sizeDelta.x, 450), new Vector2(disponibilityScrollView.offsetMax.x, -450)));
                    GameObject propertyButton = Instantiate(propertyItemPrefab, filteredPropertiesContent);
                    PropertyButton buttonObject = propertyButton.GetComponent<PropertyButton>();
                    buttonObject.InitializeDateTime(startDate, endDate);
                    Debug.Log(fromReservation);
                    if (fromReservation)
                    {
                        buttonObject.Initialize(property, OpenRoomScreen, SelectDropdownProperty, selectionCallback);
                    }
                    else
                    {
                        buttonObject.Initialize(property, OpenRoomScreen, SelectDropdownProperty, null);
                    }
                    propertyItemList.Add(propertyButton);
                }
                else
                {
                    StartCoroutine(ExpandHeaderBar(new Vector2(headerBar.sizeDelta.x, 560), new Vector2(disponibilityScrollView.offsetMax.x, -560)));
                    availableRoomsNumber.text = "Properietatea nu este disponibila";
                    availableRoomsNumber.color = Constants.reservedUnavailableItemColor;
                }
            }
        }
    }

    public void CheckRoomsSelection()
    {
        if (selectedRooms.Count() == 0)
        {
            vibrate = true;
            roomSelection = false;
            StartCoroutine(ExpandFooterBar(new Vector2(disponibilityScrollView.offsetMin.x, disponibilityHeight), new Vector2(footerBar.anchoredPosition.x, -160)));
            foreach (var room in roomItemList)
            {
                RoomButton roomObject = room.GetComponent<RoomButton>();
                roomObject.DisponibilityMarker.color = Constants.availableItemColor;
                if (roomObject.Selected)
                {
                    roomObject.SelectToggleMark();
                }
            }
        }
        else
        {
            if (vibrate)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                VibrationController vib = new VibrationController();
                if (vib.hasVibrator())
                {
                    vib.vibrate(250);
                }
#endif
                vibrate = false;
            }
            roomSelection = true;
            StartCoroutine(ExpandFooterBar(new Vector2(disponibilityScrollView.offsetMin.x, disponibilityHeight + 160), new Vector2(footerBar.anchoredPosition.x, 0)));
            foreach (var room in roomItemList)
            {
                room.GetComponent<RoomButton>().DisponibilityMarker.color = Color.white;
            }
        }
    }

    public void MakeReservation()
    {
        if (fromReservation)
        {
            selectionCallback.Invoke(startDate, endDate, selectedRooms);
        }
        else
        {
            reservationScreenComponent.OpenAddReservation(startDate, endDate, selectedRooms, null);
        }
    }

    public void CancelSelection()
    {
        selectedRooms = new List<IRoom>();
        CheckRoomsSelection();
    }

    public void ClearChanges()
    {
        selectionCallback = null;
        currentReservation = null;
        selectedProperty = null;
        fromReservation = false;
    }

    private void BackButtonFunction()
    {
        if (propertyDropdownList.value == 0 || fromReservation)
        {
            navigator.GoBack();
        }
        else
        {
            propertyDropdownList.value = 0;
        }
    }

    private void SelectDropdownProperty(IProperty property)
    {
        if (property.HasRooms)
        {
            if (lastDropdownOption == propertyDropdownOptions[property.ID])
            {
                SelectProperty(lastDropdownOption);
            }
            else
            {
                lastDropdownOption = propertyDropdownOptions[property.ID];
                propertyDropdownList.value = lastDropdownOption;
            }
        }
        else
        {
            if (lastDropdownOption == propertyDropdownOptions[property.ID])
            {
                SelectProperty(lastDropdownOption);
            }
            else
            {
                lastDropdownOption = propertyDropdownOptions[property.ID];
                propertyDropdownList.value = lastDropdownOption;
            }
        }
    }

    private void SelectDropdownProperty(IRoom propertyRoom)
    {
        selectedProperty = PropertyDataManager.GetProperty(propertyRoom.PropertyID);
        if (lastDropdownOption == propertyDropdownOptions[selectedProperty.ID])
        {
            SelectProperty(lastDropdownOption);
        }
        else
        {
            lastDropdownOption = propertyDropdownOptions[selectedProperty.ID];
            propertyDropdownList.value = lastDropdownOption;
        }
    }

    private void OpenRoomScreen(IRoom room)
    {
        roomScreen.UpdateRoomDetailsFields(room);
        roomScreen.UpdateDateTime(startDate, endDate);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }

    public void OpenDisponibility(IReservation current, DateTime start, DateTime end, List<IRoom> selectedRooms, Action<DateTime, DateTime, List<IRoom>> confirmSelection)
    {
        fromReservation = true;
        selectionCallback = confirmSelection;
        if (current != null)
        {
            currentReservation = current;
        }
        startDate = start;
        endDate = end;
        navigator.GoTo(this.GetComponent<NavScreen>());
        if (selectedRooms != null)
        {
            this.selectedRooms = new List<IRoom>(selectedRooms);
            selectedProperty = PropertyDataManager.GetProperty(selectedRooms[0].PropertyID);
            shouldSelectRooms = true;
            SelectDropdownProperty(selectedProperty);
        }
    }

    private IEnumerator ExpandFooterBar(Vector2 scrollEndSize, Vector2 buttonsEndSize)
    {
        float currentTime = 0;
        while (currentTime < 0.4f)
        {
            currentTime += Time.deltaTime;
            disponibilityScrollView.offsetMin = Vector2.Lerp(disponibilityScrollView.offsetMin, scrollEndSize, currentTime / 0.4f);
            footerBar.anchoredPosition = Vector2.Lerp(footerBar.anchoredPosition, buttonsEndSize, currentTime / 0.4f);
            yield return null;
        }
        disponibilityScrollView.offsetMin = scrollEndSize;
        footerBar.anchoredPosition = buttonsEndSize;
    }

    private IEnumerator ExpandHeaderBar(Vector2 headerEndSize, Vector2 scrollEndSize)
    {
        float currentTime = 0;
        while (currentTime < 0.4f)
        {
            currentTime += Time.deltaTime;
            headerBar.sizeDelta = Vector2.Lerp(headerBar.sizeDelta, headerEndSize, currentTime / 0.4f);
            disponibilityScrollView.offsetMax = Vector2.Lerp(disponibilityScrollView.offsetMax, scrollEndSize, currentTime / 0.4f);
            yield return null;
        }
        headerBar.sizeDelta = headerEndSize;
    }
}
