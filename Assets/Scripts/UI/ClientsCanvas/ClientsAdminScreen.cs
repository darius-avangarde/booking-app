using System.Collections.Generic;
using UnityEngine;
using UINavigation;
using UnityEngine.UI;
using System;

public class ClientsAdminScreen : MonoBehaviour
{
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Text clientScreenName = null;
    [SerializeField]
    private Text clientScreenPhone = null;
    [SerializeField]
    private Text clientScreenAdress = null;
    [SerializeField]
    private Text clientScreenEmail = null;
    [SerializeField]
    private Transform addReservationButton;
    [SerializeField]
    private Transform reservationInfoContent = null;
    [SerializeField]
    private GameObject reservationPrefabButton = null;
    private List<GameObject> reservationButtons = new List<GameObject>();
    private IClient currentClient;
    [SerializeField]
    private ReservationEditScreen rezerv = null;

    public IClient GetCurrentClient()
    {
        return currentClient;
    }
    public void SetCurrentClient(IClient client)
    {
        currentClient = client;
    }

    public void SetCurrentClients(IClient client)
    {
        currentClient = client;
        SetClientsFieldsText();
        Debug.Log(client.Name);
    }

    public void SetClientsFieldsText()
    {
        clientScreenName.text = currentClient.Name;
        clientScreenPhone.text = currentClient.Number;
        clientScreenAdress.text = currentClient.Adress;
        clientScreenEmail.text = currentClient.Email;
    }

    public void DeleteClientButton()
    {
        DeleteClient();
    }
    public void DeleteClient(Action actionDelete = null)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_CLIENT,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                ClientDataManager.DeleteClient(currentClient.ID);
                ReservationDataManager.DeleteReservationsForClient(currentClient.ID);
                navigator.GoBack();
                actionDelete?.Invoke();
            },
            CancelCallback = null
        });
    }

    public void SetAddReservationButton()
    {
        addReservationButton.SetAsFirstSibling();
    }

    public void InstantiateReservations()
    {
        foreach (var reservationButton in reservationButtons)
        {
            Destroy(reservationButton);
        }

        foreach (var reservation in ReservationDataManager.GetActiveClientReservations(currentClient.ID))
        {
            GameObject reservationButton = Instantiate(reservationPrefabButton, reservationInfoContent);
            reservationButton.GetComponent<ReservationButton>().Initialize(reservation, () => rezerv.OpenEditReservation(reservation, UpdateCallBack),DeleteReservation);
            reservationButtons.Add(reservationButton);
        }
    }

    private void UpdateCallBack(IReservation reserv)

    {
        SetCurrentClients(ClientDataManager.GetClient(reserv.CustomerID));
    }
    private void DeleteReservation(IReservation reservation)
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_DIALOG,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () => {
                ReservationDataManager.DeleteReservation(reservation.ID);
                InstantiateReservations();
            },
            CancelCallback = null
        });
    }

    public void AddReservationForClient()
    {
        rezerv.OpenAddReservation(currentClient, UpdateCallBack);
    }
   
}
