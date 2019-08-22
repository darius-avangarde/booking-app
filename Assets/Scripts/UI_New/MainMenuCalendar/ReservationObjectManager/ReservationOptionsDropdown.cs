using UnityEngine;
using System;

public class ReservationOptionsDropdown : MonoBehaviour
{
    [SerializeField]
    private RectTransform optionsRect;
    [SerializeField]
    private ReservationsCalendarManager calendarManager;
    [SerializeField]
    private ReservationEditScreen_New reservationEdit;
    [SerializeField]
    private ConfirmationDialog modalConfirmation;
    [SerializeField]
    private ClientsScreen clientsScreen;
    [SerializeField]
    private NotificationManager notificatoinManager;
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private GameObject[] futureOnlyOptions;
    [SerializeField]
    private UnityEngine.UI.Text clientNameText;

    private IReservation currentReservation;
    private ConfirmationDialogOptions confirmationDialogOptions = new ConfirmationDialogOptions();

    public void OpenReservationMenu(IReservation clickReservation)
    {
        clientNameText.text = clickReservation.CustomerName;
        bool isPast = clickReservation.Period.End.Date <= DateTime.Today.Date;
        foreach(GameObject g in futureOnlyOptions)
        {
            g.SetActive(!isPast);
        }

        currentReservation = clickReservation;
        optionsRect.position = Input.mousePosition;
        gameObject.SetActive(true);
    }

    public void CloseReservationMenu()
    {
        gameObject.SetActive(false);
    }

    public void CallClient()
    {
        CloseReservationMenu();
        clientsScreen.phoneUS(ClientDataManager.GetClient(currentReservation.CustomerID));
    }

    public void MessageClient()
    {
        CloseReservationMenu();
        clientsScreen.SmsUs(ClientDataManager.GetClient(currentReservation.CustomerID));
    }

    public void EmailClient()
    {
        CloseReservationMenu();
        clientsScreen.EmailUs(ClientDataManager.GetClient(currentReservation.CustomerID));
    }

    public void EditReservation()
    {
        CloseReservationMenu();
        reservationEdit.OpenEditScreen((r) => calendarManager.JumpToDate(r.Period.Start.Date), currentReservation);
    }

    public void DeleteReservation()
    {
        CloseReservationMenu();
        confirmationDialogOptions.ConfirmCallback = DeleteReservationCallback;
        confirmationDialogOptions.Message = $"{LocalizedText.Instance.ReservationModified[3]} {currentReservation.CustomerName}";
        modalConfirmation.Show(confirmationDialogOptions);
    }

    private void DeleteReservationCallback()
    {
        notificatoinManager.DeleteNotification(currentReservation);
        ReservationDataManager.DeleteReservation(currentReservation.ID);
        calendarManager.JumpToDate(DateTime.Today.Date);
        inputManager.Message(LocalizedText.Instance.ReservationModified[2]);
    }
}
