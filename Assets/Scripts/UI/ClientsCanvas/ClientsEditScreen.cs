using System;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class ClientsEditScreen : MonoBehaviour
{
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private InputField clientName;
    [SerializeField]
    private InputField clientPhone;
    [SerializeField]
    private InputField clientAdress;
    [SerializeField]
    private InputField clientEmail;
    private IClient currentClient;
    [SerializeField]
    private Text textError;
    [SerializeField]
    private Text textNameRequired;
    [SerializeField]
    private InfoBox infoDialog = null;
    [SerializeField]
    private ClientsScreen clientsScreen;
    public IClient GetCurrentClient()
    {
        return currentClient;
    }

    public void SetCurrentClient(IClient client)
    {
        currentClient = client;
        SetClientsFieldsText();
    }
    public void SetClientsFieldsText()
    {
        clientName.text = currentClient.Name;
        clientPhone.text = currentClient.Number;
        clientAdress.text = currentClient.Adress;
        clientEmail.text = currentClient.Email;
    }
    public void ValidAdress()
    {
        if (RegexUtilities.IsValidEmail(clientEmail.text.ToString()) || String.IsNullOrEmpty(clientEmail.text))
        {
            textError.text = string.Empty;
            textError.gameObject.SetActive(false);
        }
        else
        {
            textError.gameObject.SetActive(true);
            textError.text = Constants.Valid_Email;
        }
    }
    public void ValidateClientName()
    {
        if (String.IsNullOrEmpty(clientName.text) || clientName.text.All(char.IsWhiteSpace))
        {
            textNameRequired.text = Constants.NameRequired;
            textNameRequired.gameObject.SetActive(true);
        }
        else
        {
            textNameRequired.text = string.Empty;
            textNameRequired.gameObject.SetActive(false);
        }
    }
    public void ValidateClientPhone()
    {
        if (String.IsNullOrEmpty(clientPhone.text) || clientPhone.text.All(char.IsWhiteSpace))
        {
            textNameRequired.text = Constants.PhoneRequired;
            textNameRequired.gameObject.SetActive(true);
        }
        else
        {
            textNameRequired.text = string.Empty;
            textNameRequired.gameObject.SetActive(false);
        }
    }

    public void SetTextRequired()
    {
        textNameRequired.gameObject.SetActive(true);
        textNameRequired.text = Constants.Name_Phone_Required;
    }

    public void ShowInfo()
    {
        infoDialog.Show(  $"{Environment.NewLine}*Câmpurile marcate cu steluță sunt obligatorii.");
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
}
