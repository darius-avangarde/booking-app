using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RezervationsScreen_New : MonoBehaviour
{
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Dropdown propertyDropdown;
    [SerializeField]
    private Dropdown roomDropdown;
    [SerializeField]
    private InputField customerInputField;

    private Dictionary<string,Dropdown.OptionData> propertyOptions;
    private Dictionary<string,Dropdown.OptionData> roomOptions;

    private string selectedPropertyID = string.Empty;
    private string selectedRoomID = string.Empty;
    private string customerID = string.Empty;

    private IReservation_New selectedReservation = null;

    //set these on callback from calendar overlay
    private DateTime start;
    private DateTime end;


    public void OpenRezervationScreen(IRoom room)
    {
        UpdateDropdowns(room);
        selectedReservation = null;
    }

    public void OpenRezervationScreen(IReservation_New reservation)
    {
        UpdateDropdowns(PropertyDataManager.GetProperty(reservation.PropertyID).GetRoom(reservation.RoomID));
        selectedReservation = reservation;
    }

    // public void OpenRezervationScreen(ICustomer cutomer)
    // {
    //    InitializePropertyDropdown(null);
    // }

    public void SelectProperty(int selectedValue)
    {
        selectedPropertyID = propertyOptions.ElementAt(selectedValue).Key;
        //if(property has rooms)
        //{
        //    InitializeRoomDropdown()
        //    roomDropdown.gameObject.SetActive(true);
        //}
        //else
        //{
            //set room id as well
        //    roomDropdown.gameObject.SetActive(false);
        //}
    }
    public void SelectRoom(int selectedValue)
    {
        selectedRoomID = propertyOptions.ElementAt(selectedValue).Key;
    }

    public bool CommitChanges()
    {
        if(customerInputField.text != null) //check fields/ dropdowns and reservation date for overlap with other non deleted reservations
        {
            return false;
        }

        if(selectedReservation != null)
        {
            UpdateReservation(selectedReservation);
        }
        else
        {
            CreateReservation();
        }
        //go back tru navigator;

        return true;
    }

    private void CreateReservation()
    {
        ReservationDataManager_New.AddReservation(
            PropertyDataManager.GetProperty(selectedPropertyID).GetRoom(selectedRoomID),
            customerID,
            start,
            end
        );
    }

    private void UpdateReservation(IReservation_New reservation)
    {
        ReservationDataManager_New.EditReservation(reservation, PropertyDataManager.GetProperty(selectedPropertyID).GetRoom(selectedRoomID), customerID, start, end);
    }


    private void UpdateDropdowns(IRoom room)
    {
        IProperty property = PropertyDataManager.GetProperty(room.PropertyID);
        UpdatePropertyDropdown(property);
        if(property.Rooms.Count() > 1)
        {
            UpdateRoomDropdown(property, room);
            roomDropdown.gameObject.SetActive(true);
        }
        else
        {
            roomDropdown.gameObject.SetActive(false);
        }

    }

    ///Initializes and selects the corect dropdown int option if the property parameter is not null
    private void UpdatePropertyDropdown(IProperty property)
    {
        propertyOptions = new Dictionary<string,Dropdown.OptionData>();
        int selected = 0;
        bool foundSelected = false;

        foreach (var p in PropertyDataManager.GetProperties())
        {
            propertyOptions.Add(p.ID ,new Dropdown.OptionData(p.Name));
            if(!foundSelected && property != null)
            {
                if(property.ID == p.ID)
                {
                    foundSelected = true;
                }
                selected++;
            }
        }

        propertyDropdown.options = propertyOptions.Values.ToList();

        propertyDropdown.value = selected;

        if(property != null)
        {
            SelectProperty(selected);
        }
    }

    ///Initializes and selects the corect dropdown int option if the room parameter is not null
    private void UpdateRoomDropdown(IProperty property, IRoom room)
    {
        roomOptions = new Dictionary<string,Dropdown.OptionData>();
        int selected = 0;
        bool foundSelected = false;

        foreach (var r in property.Rooms)
        {
            roomOptions.Add(r.ID ,new Dropdown.OptionData(r.Name));
            if(!foundSelected && room != null)
            {
                if(room.ID == r.ID)
                {
                    foundSelected = true;
                }
                selected++;
            }
        }

        roomDropdown.options = propertyOptions.Values.ToList();

        roomDropdown.value = selected;

        if(room != null)
        {
            SelectRoom(selected);
        }
    }
}
