using System.Collections;
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
    private IClient currentClient;

    public IClient GetCurrentClient()
    {
        return currentClient;
    }
    public void SetCurrentClient(IClient client)
    {
        currentClient = client;
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
            Message = "Ștergeți clientul?",
            ConfirmText = "Ștergeți",
            CancelText = "Anulați ",
            ConfirmCallback = () =>
            {
                ClientDataManager.DeleteClient(currentClient.ID);
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
}
