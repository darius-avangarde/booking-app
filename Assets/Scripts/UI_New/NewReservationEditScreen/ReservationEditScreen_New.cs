using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UINavigation;
using System.Collections;

public class ReservationEditScreen_New : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator;
    [SerializeField]
    private NavScreen navScreen;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private ConfirmationDialog confirmationDialog;
    [SerializeField]
    private ConfirmationDialogOptions editConfirmation;
    [SerializeField]
    private ClientPicker clientPicker;
    [SerializeField]
    private ModalCalendar modalCalendar;
    [SerializeField]
    private NotificationManager notificationManager;
    [SerializeField]
    private SettingsManager settings;

    [SerializeField]
    private ScrollRect screenScrollRect;


    [Space]
    [SerializeField]
    private Text screenTitleText;
    [SerializeField]
    private Text startDateText;
    [SerializeField]
    private Text endDateText;
    [SerializeField]
    private Text errorText;

    [Space]
    [SerializeField]
    private Dropdown propertyDropdown;
    [SerializeField]
    private RoomToggleDropdown roomDropdown;
    [SerializeField]
    private GameObject roomDropdownParent;
    [SerializeField]
    private Dropdown preAlertDropdown;
    [SerializeField]
    private GameObject preAlertDropdownParent;

    private UnityAction<IReservation> confirmationCallback;
    private List<IProperty> propertyOptions;
    private List<IRoom> roomOptions;

    #region MandatoryReservationData
        private IReservation currentReservation;
        private DateTime? resStartDate;
        private DateTime? resEndDate;
        private IClient resClient;
        private IProperty resProperty;
        private List<IRoom> resRooms = new List<IRoom>();
        private int resPreAlertDays;
    #endregion

    private void Start()
    {
        editConfirmation = new ConfirmationDialogOptions();
        UpdatePreAletDropdown();
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => UpdatePreAletDropdown());
        //add to update event of loc manager
    }

    //for day header item
    public void OpenEditScreen(UnityAction<IReservation> confirmCallback, IProperty property, DateTime reservationStartDate)
    {
        confirmationCallback = confirmCallback;
        ClearData();
        resStartDate = reservationStartDate;
        resProperty = property;
        resRooms.Clear();
        if(!resProperty.HasRooms)
            resRooms.Add(resProperty.GetRoom(resProperty.GetPropertyRoomID));
        UpdateAllUI();
        navigator.GoTo(navScreen);
    }

    //for room column item(w/o date) / unreserved day column item (w date)
    public void OpenEditScreen(UnityAction<IReservation> confirmCallback, IProperty property, IRoom room, DateTime? reservationStartDate = null)
    {
        confirmationCallback = confirmCallback;
        ClearData();
        resStartDate = reservationStartDate;
        resProperty = property;
        resRooms.Clear();
        resRooms.Add(room);
        UpdateAllUI();
        navigator.GoTo(navScreen);
    }

    //for editing a specific reservation
    public void OpenEditScreen(UnityAction<IReservation> confirmCallback, IReservation reservation)
    {
        confirmationCallback = confirmCallback;
        ClearData();
        currentReservation = reservation;
        resStartDate = reservation.Period.Start.Date;
        resEndDate = reservation.Period.End.Date;
        resClient = ClientDataManager.GetClient(reservation.CustomerID);
        resProperty = PropertyDataManager.GetProperty(reservation.PropertyID);
        resRooms = resProperty.Rooms.Where(r => reservation.ContainsRoom(r.ID)).ToList();
        UpdateAllUI();
        navigator.GoTo(navScreen);
    }

    public void SaveReservation()
    {
        if(!InputIsValid())
            return;

        if (currentReservation != null)
        {
            editConfirmation.ConfirmCallback = () =>
            {
                currentReservation.EditReservation(
                    resRooms,
                    resClient,
                    resStartDate.Value,
                    resEndDate.Value
                );
                navigator.GoBack();
                confirmationCallback?.Invoke(currentReservation);
                inputManager.Message(Constants.RESERVATION_MODIFIED);
            };
            confirmationDialog.Show(editConfirmation);

            notificationManager.RegisterNotification(currentReservation, LocalizedText.Instance.PreAlertDictFunction.ElementAt(preAlertDropdown.value).Key);
        }
        else
        {
            IReservation newReservation = ReservationDataManager.AddReservation(
                resRooms,
                resClient,
                resStartDate.Value,
                resEndDate.Value
            );

            navigator.GoBack();
            confirmationCallback?.Invoke(newReservation);
            inputManager.Message(Constants.RESERVATION_SAVED);

            notificationManager.RegisterNotification(newReservation, LocalizedText.Instance.PreAlertDictFunction.ElementAt(preAlertDropdown.value).Key);
        }

        //get pre hours from prealertdict in hours
    }

    public void Cancel()
    {
        navigator.GoBack();
    }

    public void SetDate(bool isStart)
    {
        modalCalendar.OpenCallendar((d) => SetDatesCallback(d, isStart), (isStart) ? resStartDate :  (resEndDate == null && resStartDate != null) ? resStartDate.Value.AddDays(1) : resEndDate);
    }

    public void BeginSelectRooms()
    {
        roomDropdown.OpenRoomDropdown();
    }

    private void ClearData()
    {
        currentReservation = null;
        resStartDate = null;
        resEndDate = null;
        resClient = null;
        resProperty = null;
        resRooms.Clear();
    }

    private void UpdateAllUI()
    {
        screenTitleText.text = currentReservation == null ? LocalizedText.Instance.ReservationHeader[0] : LocalizedText.Instance.ReservationHeader[1];
        startDateText.text = resStartDate == null ? LocalizedText.Instance.Prepositions[0] : resStartDate.Value.ToString(Constants.DateTimePrintFormat);
        endDateText.text = resEndDate == null ? LocalizedText.Instance.Prepositions[1] : resEndDate.Value.ToString(Constants.DateTimePrintFormat);
        clientPicker.SetCallback((c) => resClient = c, resClient);
        UpdatePropertyDropdown();
        UpdateRoomDropdown();

        preAlertDropdownParent.SetActive(settings.ReadData().settings.ReceiveNotifications);
        preAlertDropdown.value = settings.ReadData().settings.PreAlertTime;
        SetErrorAnState(null);
    }

    private void SelectProperty(int index)
    {
        resProperty = propertyOptions[index];
        resRooms.Clear();
        if(resProperty != null && !resProperty.HasRooms) resRooms.Add(resProperty.GetRoom(resProperty.GetPropertyRoomID));
        UpdateRoomDropdown();
    }

    private void SelectRooms(List<IRoom> rooms)
    {
        resRooms = rooms;
    }

    private void SelectPreAlert(int index)
    {
        resPreAlertDays = LocalizedText.Instance.PreAlertDictFunction.ElementAt(index).Key;
    }

    private void SetDatesCallback(DateTime date, bool isStart)
    {
        if(isStart)
        {
            resStartDate = date.Date;
        }
        else if (resStartDate.HasValue && resStartDate.Value > date.Date)
        {
            resEndDate = resStartDate;
            resStartDate = date.Date;
        }
        else
        {
            resEndDate = date.Date;
        }

        startDateText.text = resStartDate.HasValue ? resStartDate.Value.ToString(Constants.DateTimePrintFormat) : "Din";
        endDateText.text = resEndDate.HasValue ? resEndDate.Value.ToString(Constants.DateTimePrintFormat) : "Până în";
    }

    private void UpdatePropertyDropdown()
    {
        propertyDropdown.onValueChanged.RemoveAllListeners();
        List<IProperty> properties = PropertyDataManager.GetProperties().ToList();
        List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
        propertyOptions = new List<IProperty>();
        optionList.Add(new Dropdown.OptionData(Constants.CHOOSE));
        propertyOptions.Add(null);

        int selected = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            if(resProperty != null && resProperty.ID == properties[i].ID)
                selected = i + 1;

            Dropdown.OptionData option = new Dropdown.OptionData(properties[i].Name/*, prop type sprite*/);
            optionList.Add(option);
            propertyOptions.Add(properties[i]);
        }

        propertyDropdown.options = optionList;
        propertyDropdown.value = selected;
        propertyDropdown.RefreshShownValue();
        propertyDropdown.onValueChanged.AddListener(SelectProperty);
    }

    private void UpdateRoomDropdown()
    {
        roomDropdownParent.SetActive(resProperty.HasRooms);

        if(!resProperty.HasRooms)
            return;

        List<IRoom> allRooms = resProperty.Rooms.ToList();
        roomDropdown.InitializeRoomDropdown(SelectRooms, allRooms, resRooms);
    }

    private void UpdatePreAletDropdown()
    {
        List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
        for (int i = 0; i < LocalizedText.Instance.PreAlertDictFunction.Count; i++)
        {
            optionList.Add(new Dropdown.OptionData(LocalizedText.Instance.PreAlertDictFunction.ElementAt(i).Value));
        }

        preAlertDropdown.options = optionList;
        preAlertDropdown.value = settings.ReadData().settings.PreAlertTime;
    }

    private bool InputIsValid()
    {
        if(resClient == null)
        {
            if(!string.IsNullOrEmpty(clientPicker.CurrentInputText))
            {
                ClientDataManager.Client c = new ClientDataManager.Client();
                c.Name = clientPicker.CurrentInputText;
                ClientDataManager.AddClient(c);
                resClient = c;
            }
            else
            {
                SetErrorAnState(Constants.ERR_CLIENT);
                return false;
            }
        }

        if(resStartDate == null)
        {
            SetErrorAnState(Constants.ERR_DATES_START);
            return false;
        }

        if(resEndDate == null)
        {
            SetErrorAnState(Constants.ERR_DATES_END);
            return false;
        }

        if(resProperty == null)
        {
            SetErrorAnState(Constants.ERR_CLIENT);
            return false;
        }

        if(resRooms.Count == 0)
        {
            SetErrorAnState(Constants.ERR_ROOM);
            return false;
        }

        if(startDateText == endDateText)
        {
            SetErrorAnState(Constants.ERR_DATES);
            return false;
        }

        if (OverlapsOtherReservation(resStartDate.Value, resEndDate.Value))
        {
            SetErrorAnState(Constants.ERR_PERIOD);
            return false;
        }

        SetErrorAnState(string.Empty);
        return true;
    }

    private void SetErrorAnState(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        if(!string.IsNullOrEmpty(message))
        {
            StopAllCoroutines();
            StartCoroutine(LerpToZero());
        }
    }

    private IEnumerator LerpToZero()
    {
        float from = screenScrollRect.verticalNormalizedPosition;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/0.15f){
            screenScrollRect.verticalNormalizedPosition = Mathf.Lerp(from, 0, t);
            yield return null;
        }
        screenScrollRect.verticalNormalizedPosition = 0;
    }

    private bool OverlapsOtherReservation(DateTime start, DateTime end)
    {
        string currentResId = (currentReservation != null) ? currentReservation.ID : Constants.defaultCustomerName;

        return ReservationDataManager.GetReservations().Where(r => resRooms.Any(room => r.ContainsRoom(room.ID))
            && r.ID != currentResId)  //get room reservation excluding current curent
            .Any(r =>
            ((start.Date > r.Period.Start.Date && start.Date < r.Period.End.Date) //start in period
            || (end.Date > r.Period.Start.Date   && end.Date < r.Period.End.Date)   //end in period
            || (start.Date < r.Period.Start.Date && end.Date > r.Period.End.Date)   //selection engulfs other reservation
            || r.Period.Start.Date == resStartDate.Value.Date || r.Period.End.Date == resEndDate.Value.Date   //start or end coincide
            ));
    }
}
