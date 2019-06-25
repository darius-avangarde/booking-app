using System;
using UnityEngine;
using UnityEngine.UI;

public class ReservationButton : MonoBehaviour
{
    [SerializeField]
    public Text propertyOrCutomerName = null;
    [SerializeField]
    private Text cameraNameOrCustomer = null;
    [SerializeField]
    private Text reservationPeriod = null;
    [SerializeField]
    private Button reservationButton;
    [SerializeField]
    private Button editButton;
    private IProperty currentProperty;
    public void Initialize(IReservation reservation, Action editCallback, bool choose = false)
    {
        if (choose == true)
        {
            currentProperty = PropertyDataManager.GetProperty(reservation.PropertyID);
            propertyOrCutomerName.text = currentProperty.Name;
            if (currentProperty.HasRooms)
            {
                if (reservation.RoomIDs.Count == 1)
                {
                    cameraNameOrCustomer.text = currentProperty.GetRoom(reservation.RoomID).Name;
                }
                else
                {
                    cameraNameOrCustomer.text = $"{reservation.RoomIDs.Count} {Constants.ROOMS_SELECTED}";
                }
            }
            else
            {
                cameraNameOrCustomer.text = string.Empty;
            }
        }
        else
        {
            propertyOrCutomerName.text = string.IsNullOrEmpty(reservation.CustomerName) ? Constants.defaultCustomerName : ClientDataManager.GetClient(reservation.CustomerID).Name;
            cameraNameOrCustomer.text = string.IsNullOrEmpty(reservation.CustomerName) ? Constants.defaultCustomerName : ClientDataManager.GetClient(reservation.CustomerID).Number;
        }
        string startPeriod = reservation.Period.Start.ToString("dd/MM/yy");
        string endPeriod = reservation.Period.End.ToString("dd/MM/yy");
        reservationPeriod.text = startPeriod + "  -  " + endPeriod;
        editButton.onClick.AddListener(() => editCallback());
    }
}
