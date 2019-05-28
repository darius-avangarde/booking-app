using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestEdit : MonoBehaviour
{
    [SerializeField]
    ReservationEditScreen edit;
    [SerializeField]
    Text testText;
    [SerializeField]
    ModalCalendarNew newCalendar;

    [SerializeField]
    Color
    unavailale = Constants.unavailableItemColor,
    selected = Constants.selectedItemColor,
    reservedAvailable = Constants.reservedAvailableItemColor,
    reserverUnavail = Constants.reservedUnavailableItemColor,
    avail = Constants.availableItemColor;

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


        List<IReservation> rlist = ReservationDataManager.GetReservations().OrderBy(r => r.Period.Start).ToList();
        for (int i = 0; i < rlist.Count; i++)
        {
            testText.text +=  Constants.NEWLINE + ClientDataManager.GetClient(rlist[i].CustomerID).Name + Constants.NEWLINE
                + PropertyDataManager.GetProperty(rlist[i].PropertyID).GetRoom(rlist[i].RoomID).Name + Constants.NEWLINE
                + rlist[i].Period.Start.ToString(Constants.DateTimePrintFormat) + " - " + rlist[i].Period.End.ToString(Constants.DateTimePrintFormat)+ Constants.NEWLINE;
        }

    }

    public void OpenViewClient()
    {
        if(client != null)
            edit.OpenAddReservation(client, (r) => DebugC(r,true));
        else
            Debug.Log("client is null");
    }

    public void OpenViewRoom()
    {
        if(room != null)
            edit.OpenAddReservation(room, (r) => DebugC(r,true));
        else
            Debug.Log("room is null");
    }

    public void OpenViewReservation()
    {
        if(reservation != null)
            edit.OpenEditReservation(reservation, (r) => DebugC(r,true));
        else
            Debug.Log("reservation is null");
    }

    public void OpenNewCalendar()
    {
        //newCalendar.OpenCallendar(reservation, ReservationDataManager.GetActiveRoomReservations(room.ID).ToList(), (s,e) => Debug.Log(s.ToShortDateString() + " - " + e.ToShortDateString()));
        newCalendar.OpenCallendar(System.DateTime.Today,  (s,e) => Debug.Log(s.ToShortDateString() + " - " + e.ToShortDateString()));
    }
    private void DebugC(IReservation r, bool isEdit)
    {
        Debug.Log("Confirmed " + ((isEdit) ? "edit" : "add") + " reservation for: " + ClientDataManager.GetClient(r.CustomerID).Name +  " in room: " + PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID).Name);
    }
}
