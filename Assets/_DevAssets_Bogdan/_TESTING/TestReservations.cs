using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestReservations : MonoBehaviour
{
    [SerializeField]
    private Text deletedRez;
    [SerializeField]
    private Text activeRez;
    [SerializeField]
    private ModalCalendarNew modCal;

    public void GetDeletedRez()
    {
        deletedRez.text = string.Empty;

        foreach (IReservation r in ReservationDataManager.GetDeletedReservations())
        {
            deletedRez.text += r.Period.Start.ToString(Constants.DateTimePrintFormat) + Constants.AndDelimiter + r.Period.End.ToString(Constants.DateTimePrintFormat) + Constants.NEWLINE
                + r.CustomerName + Constants.NEWLINE
                + PropertyDataManager.GetProperty(r.PropertyID).Name + ", " + PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID).Name
                + Constants.NEWLINE + Constants.NEWLINE;
        }
    }

    public void GetRez()
    {
        activeRez.text = string.Empty;

        foreach (IReservation r in ReservationDataManager.GetReservations())
        {
            activeRez.text += r.Period.Start.ToString(Constants.DateTimePrintFormat) + Constants.AndDelimiter + r.Period.End.ToString(Constants.DateTimePrintFormat) + Constants.NEWLINE
                + r.CustomerName + Constants.NEWLINE
                + PropertyDataManager.GetProperty(r.PropertyID).Name + ", " + PropertyDataManager.GetProperty(r.PropertyID).GetRoom(r.RoomID).Name
                + Constants.NEWLINE + Constants.NEWLINE;
        }
    }

    public void OpenModCal()
    {
        modCal.OpenCallendar(System.DateTime.Today, (s,e) => Debug.Log(s.ToString(Constants.DateTimePrintFormat) + " - " + e.ToString(Constants.DateTimePrintFormat)));
    }
}
