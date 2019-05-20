using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReservationObject : MonoBehaviour
{
    [SerializeField]
    private Text infoText;
    //the first two lines of text (one text object) for property / room Name, and Client name/number
    //the third line is the reservation period


    [SerializeField]
    private Button editButton;
    [SerializeField]
    private Button deleteButton;

    public void InitializeReservation(IClient client, IReservation res, UnityAction<IReservation> editAction, UnityAction<IReservation> deleteAction)
    {
        UpdateObjectUI(
            PropertyDataManager.GetProperty(res.PropertyID).Name
            + ReservationConstants.NEWLINE
            + PropertyDataManager.GetProperty(res.PropertyID).GetRoom(res.RoomID).Name
            , res);
        editButton.onClick.AddListener(() => editAction(res));
        deleteButton.onClick.AddListener(() => deleteAction(res));
    }

    public void InitializeReservation(IRoom room, IReservation reservation, UnityAction<IReservation> editAction, UnityAction<IReservation> deleteAction)
    {
        UpdateObjectUI(
            ClientDataManager.GetClient(reservation.CustomerID).Name
            + ReservationConstants.NEWLINE
            + ClientDataManager.GetClient(reservation.CustomerID).Number
            , reservation);
        editButton.onClick.AddListener(() => editAction(reservation));
        deleteButton.onClick.AddListener(() => deleteAction(reservation));
    }

    private void UpdateObjectUI(string info, IReservation reservation)
    {
        infoText.text = info
            + ReservationConstants.NEWLINE
            + reservation.Period.Start.ToString(Constants.DateTimePrintFormat)
            + Constants.AndDelimiter
            + reservation.Period.End.ToString(Constants.DateTimePrintFormat);
    }

    private void OnDestroy()
    {
        editButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();
    }
}
