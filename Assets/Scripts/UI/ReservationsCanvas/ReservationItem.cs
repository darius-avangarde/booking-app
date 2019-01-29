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
        if (reservation.Period != null)
        {
            reservationPeriod.text = reservation.Period.Start.ToString();
        }
        GetComponent<Button>().onClick.AddListener(() => callback());
    }
}
