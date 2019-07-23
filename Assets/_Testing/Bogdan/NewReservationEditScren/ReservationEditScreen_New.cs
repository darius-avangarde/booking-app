using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UINavigation;

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
    private Text screenTitleText;
    [SerializeField]
    private Text startDateText;
    [SerializeField]
    private Text endDateText;

    [SerializeField]
    private Text errorText;

    [SerializeField]
    private Dropdown propertyDropdown;
    [SerializeField]
    private Dropdown roomDropdown;
    [SerializeField]
    private GameObject roomDropdownParent;
    [SerializeField]
    private Dropdown preAlertDropdown;

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
        UpdatePreAletDropdown();
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
                confirmationCallback?.Invoke(currentReservation);
                navigator.GoBack();
                inputManager.Message(Constants.RESERVATION_MODIFIED);
            };
            confirmationDialog.Show(editConfirmation);
        }
        else
        {
            IReservation newReservation = ReservationDataManager.AddReservation(
                resRooms,
                resClient,
                resStartDate.Value,
                resEndDate.Value
            );

            confirmationCallback?.Invoke(newReservation);
            navigator.GoBack();
            inputManager.Message(Constants.RESERVATION_SAVED);
        }

        //get pre hours from prealertdict in hours
    }

    public void Cancel()
    {
        navigator.GoBack();
    }

    public void SetDate(bool isStart)
    {
        modalCalendar.OpenCallendar((d) => SetDatesCallback(d, isStart), (isStart) ? resStartDate : resEndDate);
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
        screenTitleText.text = currentReservation == null ? "Rezervare nouă" : "Modifică rezervarea";
        startDateText.text = resStartDate == null ? "Din" : resStartDate.Value.ToString(Constants.DateTimePrintFormat);
        endDateText.text = resEndDate == null ? "Până în" : resEndDate.Value.ToString(Constants.DateTimePrintFormat);
        clientPicker.SetCallback((c) => resClient = c, resClient);
        UpdatePropertyDropdown();
        UpdateRoomsDropdown();
    }

    private void SelectProperty(int index)
    {
        resProperty = propertyOptions[index];
        resRooms.Clear();
        if(resProperty != null && !resProperty.HasRooms) resRooms.Add(resProperty.GetRoom(resProperty.GetPropertyRoomID));
        UpdateRoomsDropdown();
    }

    private void SelectRoomSingle(int index)
    {
        resRooms.Clear();
        resRooms.Add(roomOptions[index]);
    }

    private void SelectRoomToggle(int index, bool select)
    {
        if(select)
        {
            resRooms.Add(roomOptions[index]);
        }
        else
        {
            resRooms.Remove(roomOptions[index]);
        }
    }

    private void SelectPreAlert(int index)
    {
        resPreAlertDays = Constants.PreAlertDict.ElementAt(index).Key;
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

    private void UpdateRoomsDropdown()
    {
        roomDropdown.onValueChanged.RemoveAllListeners();
        if(resProperty != null && resProperty.HasRooms)
        {
            //select rooms from resrooms
            roomDropdownParent.SetActive(true);
            List<IRoom> allRooms = resProperty.Rooms.ToList();
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
            optionList.Add(new Dropdown.OptionData(Constants.CHOOSE));
            roomOptions = new List<IRoom>();
            roomOptions.Add(null);

            for (int i = 0; i < allRooms.Count; i++)
            {
                Dropdown.OptionData option = new Dropdown.OptionData(allRooms[i].Name/*, prop type sprite*/); //option image = roomtype image
                optionList.Add(option);
                roomOptions.Add(allRooms[i]);
            }
            roomDropdown.options = optionList;
        }
        else
        {
            roomDropdownParent.SetActive(false);
        }
        roomDropdown.onValueChanged.AddListener(SelectRoomSingle);
    }

    private void UpdatePreAletDropdown()
    {
        List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();
        for (int i = 0; i < Constants.PreAlertDict.Count; i++)
        {
            optionList.Add(new Dropdown.OptionData(Constants.PreAlertDict.ElementAt(i).Value));
        }
        preAlertDropdown.options = optionList;
    }

    private bool InputIsValid()
    {
        if(resClient == null)
        {
            errorText.text = Constants.ERR_CLIENT;
            return false;
        }

        if(resStartDate == null)
        {
            errorText.text = Constants.ERR_DATES_START;
            return false;
        }

        if(resEndDate == null)
        {
            errorText.text = Constants.ERR_DATES_END;
            return false;
        }

        if(resProperty == null)
        {
            errorText.text = Constants.ERR_CLIENT;
            return false;
        }

        if(resRooms.Count == 0)
        {
            errorText.text = Constants.ERR_ROOM;
            return false;
        }

        if(startDateText == endDateText)
        {
            errorText.text = Constants.ERR_DATES;
            return false;
        }

        if (OverlapsOtherReservation(resStartDate.Value, resEndDate.Value))
        {
            errorText.text = Constants.ERR_PERIOD;
            return false;
        }

        errorText.text = string.Empty;
        return true;
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
