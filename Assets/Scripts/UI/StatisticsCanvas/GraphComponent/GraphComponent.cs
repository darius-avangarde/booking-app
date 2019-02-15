using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject barPrefab;
    [SerializeField]
    private Transform barsContainer = null;
    private int totalDaysPeriod = 0;

    public void Initialize(List<IRoom> filteredRooms, DateTime startDateTime, DateTime endDateTime, List<IReservation> reservationList, int xAxisTypeValueIndex)
    {
        totalDaysPeriod = (endDateTime - startDateTime).Days;
        GameObject bar = Instantiate(barPrefab, barsContainer);
    }
}
