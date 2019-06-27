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
    private ReservationEditScreen reservationScreenComponent = null;
    [SerializeField]
    private ModalCalendarNew calendarScreen = null;
    [SerializeField]
    private RoomScreen roomScreen = null;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private GameObject propertyItemPrefab = null;
    [SerializeField]
    private GameObject roomItemPrefab = null;
    [SerializeField]
    private ScrollRect disponibilityScrollRect = null;
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
    private AspectRatioFitter backgroundAspectRatioFitter = null;
    [SerializeField]
    private Text disponibilityDatePeriod = null;
    [SerializeField]
    private Text availableNumber = null;
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
    private float disponibilityScrollHeight;
    private float scrollPosition = 1;
    private int lastDropdownOption = 0;
    private int nrRooms = 0;
    private bool fromReservation = false;
    private bool shouldSelectRooms = false;
    private bool vibrate = true;

    private void Awake()
    {
        backButton.onClick.AddListener(() => BackButtonFunction());
        disponibilityScrollHeight = disponibilityScrollView.offsetMin.y;
        propertyDropdownList.onValueChanged.AddListener(SelectProperty);
    }

    /// <summary>
    /// set the position of the scroll rect to the top of the screen
    /// </summary>
    public void ScrollToTop()
    {
        scrollPosition = 1;
    }

    /// <summary>
    /// resets the date to the current date when the user opens the screen from main menu
    /// </summary>
    public void SetDefaultDate()
    {
        startDate = DateTime.Today.Date;
        endDate = DateTime.Today.AddDays(1).Date;
        lastDropdownOption = 0;
        ScrollToTop();
    }

    /// <summary>
    /// set dropdown value
    /// select the dropdown property
    /// </summary>
    public void Initialize()
    {
        propertyDropdownList.value = lastDropdownOption;
        SelectProperty(lastDropdownOption);
    }

    /// <summary>
    /// open modal calendar
    /// at close the callback sets the new date period
    /// </summary>
    public void ShowModalCalendar()
    {
        calendarScreen.OpenCallendar(startDate, endDate, SetNewDatePeriod, true);
    }

    /// <summary>
    /// callback for modal calendar to refresh with selected date period
    /// </summary>
    /// <param name="startDate">start of seleced period</param>
    /// <param name="endDate">end of seleced period</param>
    private void SetNewDatePeriod(DateTime startDate, DateTime endDate)
    {
        this.startDate = startDate;
        this.endDate = endDate;
        if (selectedProperty != null)
        {
            shouldSelectRooms = true;
        }
        ScrollToTop();
        InstantiateProperties();
        if (lastDropdownOption == 0)
        {
            selectedProperty = null;
            shouldSelectRooms = false;
        }
        if (selectedProperty != null)
        {
            SelectDropdownProperty(selectedProperty);
        }
    }

    /// <summary>
    /// select one of the properties form the dropdown menu
    /// </summary>
    /// <param name="optionIndex"></param>
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
        ScrollToTop();
    }

    /// <summary>
    /// instantiate all available properties for current period
    /// exclude current reservations and last selected room
    /// update available rooms text
    /// </summary>
    private void InstantiateProperties()
    {
        scrollRectComponent.ResetAll();
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
            DestroyImmediate(roomButton);
        }
        foreach (var propertyItem in propertyItemList)
        {
            DestroyImmediate(propertyItem);
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
                        buttonObject.InitializeDateTime(startDate, endDate);
                        buttonObject.Initialize(property, true,  SelectDropdownProperty, SelectDropdownProperty, null);
                        propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
                        propertyDropdownOptions.Add(property.ID, propertyIndex);
                        propertyItemList.Add(propertyButton);
                        propertyIndex++;
                    }
                    else
                    {
                        DestroyImmediate(propertyButton);
                    }
                }
                else
                {
                    buttonObject.InitializeDateTime(startDate, endDate);
                    buttonObject.Initialize(property, true, SelectDropdownProperty, SelectDropdownProperty, null);
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
                        DestroyImmediate(propertyButton);
                    }
                    else
                    {
                        DestroyImmediate(propertyButton);
                    }
                }
                else
                {
                    buttonObject.InitializeDateTime(startDate, endDate);
                    if (fromReservation)
                    {
                        buttonObject.Initialize(property, true, OpenRoomScreen, SelectDropdownProperty, selectionCallback);
                    }
                    else
                    {
                        buttonObject.Initialize(property, true, OpenRoomScreen, SelectDropdownProperty, null);
                    }
                    propertyOptions.Add(property.ID, new Dropdown.OptionData(property.Name));
                    propertyDropdownOptions.Add(property.ID, propertyIndex);
                    propertyItemList.Add(propertyButton);
                    propertyIndex++;
                }
            }
        }
        StartCoroutine(ExpandHeaderBar(new Vector2(headerBar.sizeDelta.x, 560), new Vector2(disponibilityScrollView.offsetMax.x, -560)));
        availableNumber.text = $"{Constants.AVAILABLE_PROPERTIES} {propertyItemList.Count()}";
        availableNumber.color = Color.white;
        propertyDropdownList.options = propertyOptions.Values.ToList();
        propertyDropdownList.RefreshShownValue();
        LayoutRebuilder.ForceRebuildLayoutImmediate(filteredPropertiesContent);
        Canvas.ForceUpdateCanvases();
        disponibilityScrollRect.verticalNormalizedPosition = scrollPosition;
        if (disponibilityScrollRect.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
    }

    /// <summary>
    /// instantiate all rooms of a certain property
    /// exclude rooms from current reservation
    /// set background picture of selected property
    /// </summary>
    /// <param name="property">selected property</param>
    private void InstantiateRooms(IProperty property)
    {
        scrollRectComponent.ResetAll();
        foreach (var propertyItem in propertyItemList)
        {
            DestroyImmediate(propertyItem);
        }
        foreach (var roomButton in roomItemList)
        {
            DestroyImmediate(roomButton);
        }
        roomItemList = new List<GameObject>();
        propertyItemList = new List<GameObject>();
        if (ImageDataManager.PropertyPhotos.ContainsKey(property.ID))
        {
            backgroundImage.sprite = (Sprite)ImageDataManager.BlurPropertyPhotos[property.ID];
            backgroundImage.gameObject.SetActive(true);
            backgroundAspectRatioFitter.aspectRatio = (float)backgroundImage.sprite.texture.width / backgroundImage.sprite.texture.height;
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
                availableNumber.text = Constants.AVAILABLE_ROOMS + nrRooms;
                if (nrRooms == 0)
                {
                    availableNumber.color = Constants.reservedUnavailableItemColor;
                }
                else
                {
                    availableNumber.color = Color.white;
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
                    if (fromReservation)
                    {
                        buttonObject.Initialize(property, true, OpenRoomScreen, SelectDropdownProperty, selectionCallback);
                    }
                    else
                    {
                        buttonObject.Initialize(property, true, OpenRoomScreen, SelectDropdownProperty, null);
                    }
                    propertyItemList.Add(propertyButton);
                }
                else
                {
                    StartCoroutine(ExpandHeaderBar(new Vector2(headerBar.sizeDelta.x, 560), new Vector2(disponibilityScrollView.offsetMax.x, -560)));
                    availableNumber.text = Constants.ERR_DISPONIBILITY;
                    availableNumber.color = Constants.reservedUnavailableItemColor;
                }
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(filteredPropertiesContent);
        Canvas.ForceUpdateCanvases();
        disponibilityScrollRect.verticalNormalizedPosition = scrollPosition;
        if (disponibilityScrollRect.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
    }

    /// <summary>
    /// check if room selection mode is on
    /// update rooms to room selection mode
    /// or refresh rooms to normal mode
    /// </summary>
    public void CheckRoomsSelection()
    {
        if (selectedRooms.Count() == 0)
        {
            vibrate = true;
            roomSelection = false;
            StartCoroutine(ExpandFooterBar(new Vector2(disponibilityScrollView.offsetMin.x, disponibilityScrollHeight), new Vector2(footerBar.anchoredPosition.x, -160)));
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
            StartCoroutine(ExpandFooterBar(new Vector2(disponibilityScrollView.offsetMin.x, disponibilityScrollHeight + 160), new Vector2(footerBar.anchoredPosition.x, 0)));
            foreach (var room in roomItemList)
            {
                room.GetComponent<RoomButton>().DisponibilityMarker.color = Color.white;
            }
        }
    }

    /// <summary>
    /// send selected rooms list and selected date period to reservation screen
    /// </summary>
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

    /// <summary>
    /// reset room selection
    /// </summary>
    public void CancelSelection()
    {
        selectedRooms = new List<IRoom>();
        CheckRoomsSelection();
    }

    /// <summary>
    /// clear all changes to default value
    /// </summary>
    public void ClearChanges()
    {
        selectionCallback = null;
        currentReservation = null;
        selectedProperty = null;
        fromReservation = false;
    }

    /// <summary>
    /// if a property is selected, on back button should revert to all properties before going to main screen
    /// </summary>
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

    /// <summary>
    /// select a property from the dropdown
    /// </summary>
    /// <param name="property">selected property</param>
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

    /// <summary>
    /// select a property from the dropdown
    /// this is used for properties without rooms
    /// </summary>
    /// <param name="propertyRoom">room of selected property</param>
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

    /// <summary>
    /// default action for room objects
    /// open room screen with the selected room
    /// </summary>
    /// <param name="room"></param>
    private void OpenRoomScreen(IRoom room)
    {
        scrollPosition = disponibilityScrollRect.verticalNormalizedPosition;
        roomScreen.UpdateRoomDetailsFields(room);
        roomScreen.UpdateDateTime(startDate, endDate);
        navigator.GoTo(roomScreen.GetComponent<NavScreen>());
    }

    /// <summary>
    /// function callback to open disponibility screen
    /// </summary>
    /// <param name="current">current reservation</param>
    /// <param name="start">start of date period</param>
    /// <param name="end">end of date period</param>
    /// <param name="selectedRooms">selected rooms list</param>
    /// <param name="confirmSelection">callback for reservation screen</param>
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

    /// <summary>
    /// animation to expand the footer bar when a room is selected
    /// </summary>
    /// <param name="scrollEndSize">scroll view final size</param>
    /// <param name="buttonsEndSize">footer bar final size</param>
    /// <returns></returns>
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

    /// <summary>
    /// animation to expand header bar
    /// the field with the available rooms
    /// </summary>
    /// <param name="headerEndSize">header bar final size</param>
    /// <param name="scrollEndSize">scroll rect component final size</param>
    /// <returns></returns>
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
