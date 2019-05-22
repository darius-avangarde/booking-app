using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestEdit : MonoBehaviour
{
    [SerializeField]
    ReservationEditScreen edit;

    IClient client;
    IRoom room;
    IReservation reservation;
    // Start is called before the first frame update
    void Start()
    {
        client = ClientDataManager.GetClients().ToList()[0];
        room = PropertyDataManager.GetProperties().ToList()[0].Rooms.ToList()[0];
        reservation = ReservationDataManager.GetReservations().ToList()[0];
    }

    public void OpenViewClient()
    {
        if(client != null)
            edit.OpenEditReservation(client, null);
        else
            Debug.Log("Reservation is null");
    }

    public void OpenViewRoom()
    {
        if(room != null)
            edit.OpenEditReservation(room, null);
        else
            Debug.Log("Reservation is null");
    }

    public void OpenViewReservation()
    {
        if(reservation != null)
            edit.OpenEditReservation(reservation, null);
        else
            Debug.Log("Reservation is null");
    }

}
