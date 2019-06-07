using System.Collections.Generic;
using UnityEngine;
using UINavigation;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class ClientsAdminScreen : MonoBehaviour
{
    #region SerializeFieldVariables
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
    [SerializeField]
    private ReservationEditScreen rezerv = null;
    #endregion
    private List<GameObject> reservationButtons = new List<GameObject>();
    private IClient currentClient;
    [SerializeField]
    private ClientsScreen clientsScreen;
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
    }

    public void SetClientsFieldsText()
    {
        clientScreenName.text = currentClient.Name;
        clientScreenPhone.text = currentClient.Number;
        clientScreenAdress.text = currentClient.Adress;
        clientScreenEmail.text = currentClient.Email;
    }

    private void DeleteClientButton(Action actionDelete = null)
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
                clientsScreen.InstantiateClients();
                navigator.GoBack();
                actionDelete?.Invoke();
            },
            CancelCallback = null
        });
    }
    public void DeleteClientCurrent()
    {
        DeleteClientButton();
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
            reservationButton.GetComponent<ReservationButton>().Initialize(reservation, () => rezerv.OpenEditReservation(reservation, UpdateCallBack), DeleteReservation);
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
            ConfirmCallback = () =>
            {
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

    #region SmsPhoneEmail
    public void CallClient()
    {
        Application.OpenURL("tel://" + currentClient.Number);
    }
    public void SendSms()
    {
        Application.OpenURL("sms://" + currentClient.Number);
    }
    public void SendEmail()
    {
        if (currentClient.Email == null)
        {
            //textEmailRequired.text = "Nu exită email înregistrat!";
            Debug.Log("fara email");
        }
        else
        {

            string subject = MyEscapeURL("Custom application development");

            Application.OpenURL("mailto:" + currentClient.Email + "?subject=" + subject);
            Debug.Log(currentClient.Email + "email is:");
        }
    }
    string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }
    #endregion
}
