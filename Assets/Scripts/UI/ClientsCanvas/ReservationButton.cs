using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReservationButton : MonoBehaviour
{
    [SerializeField]
    public Text propertyName = null;
    [SerializeField]
    private Text cameraName = null;
    [SerializeField]
    private Text reservationPeriod = null;
    [SerializeField]
    private Button reservationButton;
    [SerializeField]
    private Button editButton;
    [SerializeField]
    private Button deleteButton;

    public void Initialize(IReservation reservation)//, Action<IReservation> callback, Action<IReservation> editCallBack, Action<IReservation> deleteCallback)
    {
        
        propertyName.text = PropertyDataManager.GetProperty(reservation.PropertyID).Name;//string.IsNullOrEmpty(client.Name) ? Constants.defaultRoomAdminScreenName : reservation.Name;
        cameraName.text = PropertyDataManager.GetProperty(reservation.PropertyID).GetRoom(reservation.RoomID).Name;
        string startPeriod = reservation.Period.Start.ToString("dd/MM/yy");
        string endPeriod = reservation.Period.End.ToString("dd/MM/yy");
        reservationPeriod.text = startPeriod + "  -  " + endPeriod;
        //reservationButton.onClick.AddListener(() => callback(reservation));
        // editButton.onClick.AddListener(() => editCallBack(reservation));
        // deleteButton.onClick.AddListener(() => deleteCallback(reservation));
    }
}
