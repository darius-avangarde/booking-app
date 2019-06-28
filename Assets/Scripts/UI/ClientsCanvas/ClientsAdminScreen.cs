using System.Collections.Generic;
using UnityEngine;
using UINavigation;
using UnityEngine.UI;
using System;
using System.Linq;

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
    private RectTransform reservationInfoContent = null;
    [SerializeField]
    private GameObject reservationPrefabButton = null;
    [SerializeField]
    private ReservationEditScreen rezerv = null;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private ClientsScreen clientsScreen;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private ScrollRect reservationsScrollView = null;
    #endregion
    private List<GameObject> reservationButtons = new List<GameObject>();
    private IClient currentClient;
    private float scrollPosition = 1;

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

    public void InstantiateReservations()
    {
        scrollRectComponent.ResetAll();
        foreach (var reservationButton in reservationButtons)
        {
            DestroyImmediate(reservationButton);
        }

        foreach (var reservation in ReservationDataManager.GetActiveClientReservations(currentClient.ID).OrderBy(r => r.Period.Start))
        {
            GameObject reservationButton = Instantiate(reservationPrefabButton, reservationInfoContent);
            reservationButton.GetComponent<ReservationButton>().Initialize(reservation, () => rezerv.OpenEditReservation(reservation, UpdateCallBack), true);
            reservationButtons.Add(reservationButton);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(reservationInfoContent);
        Canvas.ForceUpdateCanvases();
        reservationsScrollView.verticalNormalizedPosition = scrollPosition;
        if (reservationsScrollView.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
    }

    public void LastPosition()
    {
        scrollPosition = reservationsScrollView.verticalNormalizedPosition;
    }
    private void UpdateCallBack(IReservation reserv)
    {
        SetCurrentClients(ClientDataManager.GetClient(reserv.CustomerID));
    }

    public void AddReservationForClient()
    {
        LastPosition();
        rezerv.OpenAddReservation(currentClient, UpdateCallBack);
    }

    #region SmsPhoneEmail
    public void CallClient()
    {
        clientsScreen.phoneUS(currentClient);
    }
    public void SendSms()
    {
        clientsScreen.SmsUs(currentClient);
    }
    public void SendEmail()
    {
        if (string.IsNullOrEmpty(currentClient.Email))
        {
            inputManager.Message(Constants.MessageEmail);
        }
        else
        {
            clientsScreen.EmailUs(currentClient);
        }
    }
    #endregion
}
