using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestEdit : MonoBehaviour
{
    [SerializeField]
    ReservationEditScreen edit;

    [SerializeField]
    ReservationsViewScreen viewRect;

    IClient client;
    IRoom room;
    IReservation reservation;
    // Start is called before the first frame update
    void Start()
    {
        client = ClientDataManager.GetClients().ToList()[0];
        IProperty p = PropertyDataManager.GetProperties().ToList()[0];
        room = p.Rooms.ToList()[0];
        if(ReservationDataManager.GetReservations().Count() > 0)
            reservation = ReservationDataManager.GetReservations().ToList()[0];

    }

    public void OpenViewClient()
    {
        if(client != null)
            edit.OpenEditReservation(client);
        else
            Debug.Log("client is null");
    }

    public void OpenViewRoom()
    {
        if(room != null)
            edit.OpenEditReservation(room);
        else
            Debug.Log("room is null");
    }

    public void OpenViewReservation()
    {
        if(reservation != null)
            edit.OpenEditReservation(reservation);
        else
            Debug.Log("reservation is null");
    }



    public void ViewRoomReservations()
    {
        if(room != null)
            viewRect.ViewRoomReservations(room);
        else
            Debug.Log("room is null");
    }

    public void ViewClientReservations()
    {
        if(reservation != null)
            viewRect.ViewClientReservations(client);
        else
            Debug.Log("client is null");
    }

}
