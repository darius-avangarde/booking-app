using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReservationObject : MonoBehaviour
{
    [SerializeField]
    private Text infoText; //the first two lines of text (one text object) for property / room Name, and Client name/number
    [SerializeField]
    private Text periodText;


    [SerializeField]
    private Button editButton;
    [SerializeField]
    private Button deleteButton;

    public void InitializeReservation(IClient client, IReservation reservation, UnityAction<IReservation> editAction, UnityAction<IReservation> deleteAction)
    {
        UpdateObjectUI(client.Name + "\n" + client.Number,reservation);
        editButton.onClick.AddListener(() => editAction(reservation));
        deleteButton.onClick.AddListener(() => deleteAction(reservation));
    }

    public void InitializeReservation(IRoom room, IReservation reservation, UnityAction<IReservation> editAction, UnityAction<IReservation> deleteAction)
    {
        UpdateObjectUI(PropertyDataManager.GetProperty(room.PropertyID).Name + "\n" + room.Name, reservation);
        editButton.onClick.AddListener(() => editAction(reservation));
        deleteButton.onClick.AddListener(() => deleteAction(reservation));
    }

    private void UpdateObjectUI(string info, IReservation reservation)
    {
        infoText.text = info;
        periodText.text =
            reservation.Period.Start.ToString(Constants.DateTimePrintFormat)
            + Constants.AndDelimiter
            + reservation.Period.End.ToString(Constants.DateTimePrintFormat);
    }

    private void OnDestroy()
    {
        editButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();
    }
}
