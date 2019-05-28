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

    ///<summary>
    /// Initializes the reservation object with the relevant reservation data for the provided client or room.
    /// Also sets the edit and delete button actions
    ///</summary>
    public void InitializeReservation(bool isClient, IReservation res, UnityAction<IReservation> editAction, UnityAction<IReservation,GameObject> deleteAction)
    {
        UpdateObjectUI(
              ((isClient) ? PropertyDataManager.GetProperty(res.PropertyID).Name : ClientDataManager.GetClient(res.CustomerID).Name)
            + Constants.NEWLINE
            + ((isClient) ? PropertyDataManager.GetProperty(res.PropertyID).GetRoom(res.RoomID).Name : ClientDataManager.GetClient(res.CustomerID).Number)
            , res);
        editButton.onClick.AddListener(() => editAction(res));
        deleteButton.onClick.AddListener(() => deleteAction(res, gameObject));
    }

    //Updates the reservation information text
    private void UpdateObjectUI(string info, IReservation reservation)
    {
        infoText.text = info
            + Constants.NEWLINE
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
