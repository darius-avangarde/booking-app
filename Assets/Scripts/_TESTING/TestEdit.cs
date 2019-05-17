using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestEdit : MonoBehaviour
{
    [SerializeField]
    ReservationEditScreen editScreen;

    // Start is called before the first frame update
    void Start()
    {
        // List<IProperty> props = PropertyDataManager.GetProperties().ToList();
        // editScreen.OpenEditReservation(props[0].Rooms.ToList()[0]);

        // List<IClient> clients = ClientDataManager.GetClients().ToList();
        // editScreen.OpenEditReservation(clients[0]);

        List<IReservation> res = ReservationDataManager.GetReservations().ToList();
        editScreen.OpenEditReservation(res[2]);

    }

}
