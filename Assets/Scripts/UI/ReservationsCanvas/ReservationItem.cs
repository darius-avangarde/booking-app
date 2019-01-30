using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReservationItem : MonoBehaviour
{
    [SerializeField]
    private Text customerName = null;
    [SerializeField]
    private Text reservationPeriod = null;

    public void Initialize(IReservation reservation, Action callback)
    {
        customerName.text = reservation.CustomerName;
        string startPeriod = reservation.Period.Start.ToString("dd/MM/yy");
        string endPeriod = reservation.Period.End.ToString("dd/MM/yy");
        reservationPeriod.text = startPeriod + "  -  " + endPeriod;
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
