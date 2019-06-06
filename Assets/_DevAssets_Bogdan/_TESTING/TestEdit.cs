using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestEdit : MonoBehaviour
{
    [SerializeField]
    private ReservationEditScreen resEd;

    IProperty p;
    List<IRoom> rS = new List<IRoom>();
    List<IRoom> rM = new List<IRoom>();
    IReservation rSin;
    IReservation rMul;

    DateTime start;
    DateTime end;

    private void Start()
    {
        start = DateTime.Today.AddDays(3);
        end = DateTime.Today.AddDays(6);

        p = PropertyDataManager.GetProperties().ToList()[1];
        rS.Add(p.Rooms.ToList()[0]);
        rM = p.Rooms.ToList();
        rSin = ReservationDataManager.GetReservations().ToList()[1];
        rMul = ReservationDataManager.GetReservations().ToList()[0];

        //ReservationDataManager.GetReservations().ToList()[0].EditReservation(rM, ClientDataManager.GetClients().ToList()[0], DateTime.Today.AddDays(65).Date, DateTime.Today.AddDays(70).Date);
    }

    public void openSingleRoom()
    {
        resEd.OpenAddReservation(start, end, rS, (r) => Debug.Log("Reservation room count = " + r.RoomIDs.Count));
    }

    public void openMulRoom()
    {
        resEd.OpenAddReservation(start, end, rM, (r) => Debug.Log("Reservation room count = " + r.RoomIDs.Count));
    }

    public void openSingleRes()
    {
        resEd.OpenEditReservation(rSin, (r) => Debug.Log("Reservation room count = " + r.RoomIDs.Count), () => Debug.Log("delete callback"));
    }

    public void openMulRes()
    {
        resEd.OpenEditReservation(rMul, (r) => Debug.Log("Reservation room count = " + r.RoomIDs.Count), () => Debug.Log("delete callback"));
    }
}
