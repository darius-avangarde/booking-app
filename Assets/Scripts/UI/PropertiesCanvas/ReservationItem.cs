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
    private Text customerPhone = null;
    [SerializeField]
    private Text reservationPeriod = null;
    [SerializeField]
    private Button editReservationButton = null;

    public void Initialize(IReservation reservation, Action editCallback)
    {
        customerName.text = string.IsNullOrEmpty(reservation.CustomerName) ? Constants.defaultCustomerName : ClientDataManager.GetClient(reservation.CustomerID).Name;
        customerPhone.text = string.IsNullOrEmpty(reservation.CustomerName) ? Constants.defaultCustomerName : ClientDataManager.GetClient(reservation.CustomerID).Number;
        string startPeriod = reservation.Period.Start.ToString("dd/MM/yy");
        string endPeriod = reservation.Period.End.ToString("dd/MM/yy");
        reservationPeriod.text = startPeriod + "  -  " + endPeriod;
        editReservationButton.onClick.AddListener(() => editCallback());
    }
}
