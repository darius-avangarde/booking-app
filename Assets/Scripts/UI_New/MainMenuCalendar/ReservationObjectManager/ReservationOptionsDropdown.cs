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
    private InputManager inputManager;

    private IReservation currentReservation;
    private ConfirmationDialogOptions confirmationDialogOptions = new ConfirmationDialogOptions();

    public void OpenReservationMenu(IReservation clickReservation)
    {
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
        confirmationDialogOptions.Message = $"Sunteți sigur că vreți să ștergeți rezervarea pentru {currentReservation.CustomerName}";
        modalConfirmation.Show(confirmationDialogOptions);
    }

    private void DeleteReservationCallback()
    {
        calendarManager.JumpToDate(DateTime.Today.Date);
        inputManager.Message("Rezervarea a fost ștearsă cu succes");
    }
}
