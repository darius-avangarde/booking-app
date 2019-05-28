using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class RoomScreen : MonoBehaviour
{
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private ReservationEditScreen reservationScreen = null;
    [SerializeField]
    private Transform roomAdminScreenTransform = null;
    [SerializeField]
    private Transform propertyAdminScreenTransform = null;
    [SerializeField]
    private GameObject reservationPrefabButton = null;
    [SerializeField]
    private Transform reservationsContent = null;
    [SerializeField]
    private Text propertyRoomScreenTitle = null;
    //[SerializeField]
    //private Text roomDetails = null;
    private List<GameObject> reservationButtonList = new List<GameObject>();
    private DateTime dayDateTime = DateTime.Today;
    private IProperty currentProperty;
    private IRoom currentRoom;
    private IReservation currentReservation;

    public void UpdateRoomDetailsFields(/*DateTime date,*/ IRoom room)
    {
        currentProperty = PropertyDataManager.GetProperty(room.PropertyID);
        //dayDateTime = date;
        currentRoom = room;
        if (currentProperty.HasRooms)
        {
            propertyRoomScreenTitle.text = room.Name ?? Constants.NEW_ROOM;
        }
        else
        {
            propertyRoomScreenTitle.text = currentProperty.Name ?? Constants.NEW_PROPERTY;
        }
        //roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        InstantiateReservations();
    }

    public void UpdateCurrentRoomDetailsFields()
    {
        if (currentProperty.HasRooms)
        {
            propertyRoomScreenTitle.text = currentRoom.Name ?? Constants.NEW_ROOM;
        }
        else
        {
            propertyRoomScreenTitle.text = currentProperty.Name ?? Constants.NEW_PROPERTY;
        }
        //roomDetails.text = Constants.SingleBed + room.SingleBeds.ToString() + Constants.AndDelimiter + Constants.DoubleBed + room.DoubleBeds.ToString();
        InstantiateReservations();
    }

    public void EditButton()
    {
        if (currentProperty.HasRooms)
        {
            OpenRoomAdminScreen();
        }
        else
        {
            OpenPropertyAdminScreen();
        }
    }

    public void CreateNewReservation()
    {
        reservationScreen.OpenAddReservation(currentRoom, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID)));
    }

    public void InstantiateReservations()
    {
        List<IReservation> orderedRoomReservationList = ReservationDataManager.GetActiveRoomReservations(currentRoom.ID)
                                                    .OrderBy(res => res.Period.Start).ToList();

        foreach (var reservationButton in reservationButtonList)
        {
            Destroy(reservationButton);
        }

        foreach (var reservation in orderedRoomReservationList)
        {
            GameObject reservationButton = Instantiate(reservationPrefabButton, reservationsContent);
            reservationButton.GetComponent<ReservationItem>().Initialize(reservation,() => reservationScreen.OpenEditReservation(reservation, (r) => UpdateRoomDetailsFields(PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID))), DeleteReservation);
            reservationButtonList.Add(reservationButton);
        }
    }

    private void DeleteReservation(IReservation reservation)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_DIALOG,
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () => {
                ReservationDataManager.DeleteReservation(reservation.ID);
                InstantiateReservations();
            },
            CancelCallback = null
        });
    }

    private void OpenRoomAdminScreen()
    {
        roomAdminScreenTransform.GetComponent<RoomAdminScreen>().SetCurrentPropertyRoom(currentRoom);
        navigator.GoTo(roomAdminScreenTransform.GetComponent<NavScreen>());
    }

    private void OpenPropertyAdminScreen()
    {
        propertyAdminScreenTransform.GetComponent<PropertyAdminScreen>().SetCurrentProperty(currentProperty);
        navigator.GoTo(propertyAdminScreenTransform.GetComponent<NavScreen>());
    }
}
