﻿using System;
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
    private IProperty currentProperty;
    public void Initialize(IReservation reservation, Action editCallback)
    {
        currentProperty = PropertyDataManager.GetProperty(reservation.PropertyID);
        propertyName.text = currentProperty.Name;
        if (currentProperty.HasRooms)
        {
            if(reservation.RoomIDs.Count == 1)
            {
                cameraName.text = currentProperty.GetRoom(reservation.RoomID).Name;
            }
            else
            {
                cameraName.text =$"{reservation.RoomIDs.Count} {Constants.ROOMS_SELECTED}";
            }
        }
        else
        {
            cameraName.text = string.Empty;
        }
        string startPeriod = reservation.Period.Start.ToString("dd/MM/yy");
        string endPeriod = reservation.Period.End.ToString("dd/MM/yy");
        reservationPeriod.text = startPeriod + "  -  " + endPeriod;
        editButton.onClick.AddListener(() => editCallback());
    }
}
