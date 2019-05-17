using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class ReservationEditScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator reservationEditNavigator;
    [SerializeField]
    private NavScreen reservationScreen;
    [SerializeField]
    private ModalCalendar modalCalendarDialog;

    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Dropdown propertyDropdown;
    [SerializeField]
    private Dropdown roomDropdown;
    [SerializeField]
    private InputField clientInputField;
    [SerializeField]
    private Text reservationPeriodText;

    private Dictionary<string,Dropdown.OptionData> propertyOptions;
    private Dictionary<string,Dropdown.OptionData> roomOptions;

    private IReservation currentReservation;
    private IRoom currentRoom;
    private IProperty currentProperty;
    private IClient currentClient;

    private List<IReservation> roomReservationList;
    //set these on callback from calendar overlay
    private DateTime start;
    private DateTime end;


    private void Start()
    {
        propertyDropdown.onValueChanged.AddListener(SelectProperty);
        roomDropdown.onValueChanged.AddListener(SelectRoom);
    }

    public void OpenEditReservation(IReservation reservation)
    {
        UpdateEditableOptions(
            reservation,
            ClientDataManager.GetClient(reservation.CustomerID),
            PropertyDataManager.GetProperty(reservation.PropertyID).GetRoom(reservation.RoomID)
            );
        titleText.text = "Modifică rezervarea";
    }

    public void OpenEditReservation(IClient client)
    {
        UpdateEditableOptions(null, client, null);
        titleText.text = "Rezervare nouă";
    }

    public void OpenEditReservation(IRoom room)
    {
        UpdateEditableOptions(null, null, room);
        titleText.text = "Rezervare nouă";
    }

    public void ChangePeriod()
    {
        if(currentRoom != null)
        {
            modalCalendarDialog.Show(
                DateTime.Today,
                currentReservation,
                ReservationDataManager.GetReservations(a => a.RoomID == currentRoom.ID).ToList(),
                SaveAndUpdatedReservationPeriod
                );

            foreach (var item in ReservationDataManager.GetReservations(a => a.RoomID == currentRoom.ID).ToList())
            {
                Debug.Log(item.RoomID);
            }
        }
    }



    public void SelectProperty(int optionIndex)
    {
        if(optionIndex > 0)
        {
            currentProperty = PropertyDataManager.GetProperty(propertyOptions.ElementAt(optionIndex).Key);
        }
        else
        {
            currentProperty = null; //if prop has 1 room also set room
            currentRoom = null;
        }

        UpdateRoomDropdown(currentProperty);
    }

    public void SelectRoom(int optionIndex)
    {
        if(optionIndex > 0)
        {
            currentRoom = currentProperty.GetRoom(roomOptions.ElementAt(optionIndex).Key);
        }
        else
        {
            currentRoom = null;
        }
    }

    private void UpdateEditableOptions(IReservation reservation, IClient client, IRoom room)
    {
        currentReservation = reservation;
        currentRoom = room;
        currentClient = client;


            //update dropdown + data
        UpdatePropertyDropdown();

        //update dropdown + data

        clientInputField.text = (client != null) ? client.Name : Constants.defaultCustomerName;

        if(reservation != null)
        {
            start = reservation.Period.Start;
            end = reservation.Period.End;


            reservationPeriodText.text =
                start.ToString(Constants.DateTimePrintFormat)
                + Constants.AndDelimiter
                + end.ToString(Constants.DateTimePrintFormat);
        }
        else
        {
            start = DateTime.Today;
            reservationPeriodText.text = start.ToString(Constants.DateTimePrintFormat);
        }

    }

    private void UpdatePropertyDropdown()
    {
        int selected = 0;
        bool searching = true;
        currentProperty = null;

        propertyOptions = new Dictionary<string, Dropdown.OptionData>();
        propertyOptions.Add(String.Empty, new Dropdown.OptionData("Alege"));
        foreach(IProperty p in PropertyDataManager.GetProperties())
        {
            propertyOptions.Add(p.ID, new Dropdown.OptionData(p.Name));


            if (currentRoom != null)
            {
                if(searching)
                {
                    selected++;
                }

                if(currentRoom.PropertyID == p.ID)
                {
                    currentProperty = p;
                    searching = false;
                }
            }
        }

        propertyDropdown.options = propertyOptions.Values.ToList();

        propertyDropdown.value = (searching) ? 0 : selected;
        propertyDropdown.RefreshShownValue();

        UpdateRoomDropdown(currentProperty);
    }

    private void UpdateRoomDropdown(IProperty property)
    {
        if(property != null)
        {
            if(property.Rooms.Count() > 1)
            {
                roomDropdown.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                roomDropdown.transform.parent.gameObject.SetActive(false);
                currentRoom = property.Rooms.ToList()[0];
                return;
            }
        }
        else
        {
            roomDropdown.transform.parent.gameObject.SetActive(false);
            return;
        }

        int selected = 0;
        bool searching = true;


        roomOptions = new Dictionary<string, Dropdown.OptionData>();
        roomOptions.Add(String.Empty, new Dropdown.OptionData("Alege"));
        foreach(IRoom r in property.Rooms)
        {
            roomOptions.Add(r.ID, new Dropdown.OptionData(r.Name));

            if (currentRoom != null)
            {
                if(searching)
                {
                    selected++;
                }

                if(currentRoom.ID == r.ID)
                {
                    searching = false;
                }
            }
        }

        roomDropdown.options = roomOptions.Values.ToList();

        roomDropdown.value = (searching) ? 0 : selected;
        roomDropdown.RefreshShownValue();
    }











    private void SaveAndUpdatedReservationPeriod(DateTime _start, DateTime _end)
    {
        start = _start;
        end = _end;

        reservationPeriodText.text =
            start.ToString(Constants.DateTimePrintFormat)
            + Constants.AndDelimiter
            + end.ToString(Constants.DateTimePrintFormat);
    }
}
