using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestEdit : MonoBehaviour
{
    [SerializeField]
    ReservationsViewScreen view;

    IClient client;
    IRoom room;

    // Start is called before the first frame update
    void Start()
    {

        client = ClientDataManager.GetClients().ToList()[0];
        IReservation r = ReservationDataManager.GetReservations().ToList()[0];
        room = PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID);
        //editScreen.OpenEditReservation(clients[0]);
    }

    public void OpenViewC()
    {
        view.ViewClientReservations(client);
    }

    public void OpenViewR()
    {
        view.ViewRoomReservations(room);
    }

}
