using System;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using static ClientDataManager;
using System.Collections;

public class ClientsScreen : MonoBehaviour
{
    #region SerializeFieldVariables
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform clientAdminScreenTransform = null;
    [SerializeField]
    private Transform clientEditScreenTransform = null;
    [SerializeField]
    private GameObject clientPrefabButton = null;
    [SerializeField]
    private GameObject clientprefabLetter = null;
    [SerializeField]
    private RectTransform clientInfoContent = null;
    [SerializeField]
    private InputField searchField = null;
    [SerializeField]
    private Text headerBarText;
    [SerializeField]
    private InputField clientName;
    [SerializeField]
    private InputField clientPhone;
    [SerializeField]
    private InputField clientAdress;
    [SerializeField]
    private InputField clientEmail;
    [SerializeField]
    private GameObject saveButton;
    [SerializeField]
    private GameObject editButton;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private ScrollRect clientsScrollView = null;
    #endregion
    #region PrivateVariables
    private List<GameObject> clientButtons = new List<GameObject>();
    private List<GameObject> letterButtons = new List<GameObject>();
    private bool hasCallBack = false;
    private UnityAction<IClient> saveCallBack;
    private UnityAction<IClient> selectClientCallback;
    private UnityAction<bool> cancelCallback;
    private bool fromReservation = false;
    private float scrollPosition = 1;
    #endregion
    #region AnimationVariables
    public RectTransform Search;
    public RectTransform LayoutContent;
    public RectTransform ClientContainer;
    private float initialSizeSearch;
    private float initialClientContainer;
    #endregion

    private void Start()
    {
        //initializations for different sizes
        initialSizeSearch = Search.rect.height;
        initialClientContainer = ClientContainer.sizeDelta.y;

        Search.gameObject.SetActive(false);
        ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, -30 - initialSizeSearch);
        initialClientContainer = ClientContainer.sizeDelta.y;
    }

    public void InstantiateClients(bool fromReservation = false)
    {
        this.fromReservation = fromReservation;
        scrollRectComponent.ResetAll();
        foreach (var clientButton in clientButtons)
        {
            DestroyImmediate(clientButton);
        }

        foreach (var letterButton in letterButtons)
        {
            DestroyImmediate(letterButton);
        }
        //clientButtons.Clear();
        //letterButtons.Clear();
        foreach (var item in ClientDataManager.GetClientsToDictionary())
        {
            clientprefabLetter.GetComponent<Text>().text = item.Key.ToString().ToUpper();
            GameObject clientLetters = Instantiate(clientprefabLetter, clientInfoContent);
            letterButtons.Add(clientLetters);
            foreach (var client in item.Value)
            {
                GameObject clientButton = Instantiate(clientPrefabButton, clientInfoContent);
                if (fromReservation)
                    clientButton.GetComponent<ClientButton>().Initialize(client, OpenClientScreen, phoneUS, SmsUs, EmailUs, OpenEditAdminScreen);
                else
                    clientButton.GetComponent<ClientButton>().Initialize(client, OpenClientAdminScreen, phoneUS, SmsUs, EmailUs, OpenEditAdminScreen);
                clientButtons.Add(clientButton);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(clientInfoContent);
        Canvas.ForceUpdateCanvases();
        clientsScrollView.verticalNormalizedPosition = scrollPosition;
        if (clientsScrollView.content.childCount > 0)
        {
            scrollRectComponent.Init();
        }
    }

    public void LastPosition()
    {
        scrollPosition = clientsScrollView.verticalNormalizedPosition;
    }
    public void OpenClientReservation(UnityAction<IClient> callback)
    {
        InstantiateClients(true);
        selectClientCallback = callback;
    }


    public void SaveAddedClient()
    {
        if (String.IsNullOrEmpty(clientName.text) || clientName.text.All(char.IsWhiteSpace) || String.IsNullOrEmpty(clientPhone.text) || clientPhone.text.All(char.IsWhiteSpace))
        {
            clientEditScreenTransform.GetComponent<ClientsEditScreen>().SetTextRequired();
            return;
        }
        else
        {
            Client client = new Client();
            SetClient(client);
            if ((String.IsNullOrEmpty(clientEmail.text) == false && RegexUtilities.IsValidEmail(clientEmail.text.ToString()) == true) || String.IsNullOrEmpty(clientEmail.text))
            {
                ClientDataManager.AddClient(client);
                navigator.GoBack();
            }

            if (hasCallBack)
            {
                saveCallBack.Invoke(client);
                cancelCallback.Invoke(true);
            }
            hasCallBack = false;
        }

        InstantiateClients(fromReservation);
    }

    private void SetClient(Client client)
    {
        client.Name = clientName.text;
        client.Number = clientPhone.text;
        client.Email = clientEmail.text;
        client.Adress = clientAdress.text;
    }
    public void CancelAddClient()
    {
        if (hasCallBack)
        {
            hasCallBack = false;
            saveCallBack = null;
            cancelCallback.Invoke(true);
            cancelCallback = null;
        }
    }

    public void OpenAddClient(UnityAction<IClient> clientAdded, UnityAction<bool> clientCancel)
    {
        hasCallBack = true;
        saveCallBack = clientAdded;
        cancelCallback = clientCancel;
    }

    public void EditClient()
    {
        var currentClient = clientEditScreenTransform.GetComponent<ClientsEditScreen>().GetCurrentClient();
        if (String.IsNullOrEmpty(clientName.text) || String.IsNullOrEmpty(clientPhone.text))
        {
            return;
        }
        else
        {
            Client client = new Client();
            SetClient(client);
            if ((String.IsNullOrEmpty(clientEmail.text) == false && RegexUtilities.IsValidEmail(clientEmail.text.ToString()) == true) || String.IsNullOrEmpty(clientEmail.text))
                ClientDataManager.EditClient(currentClient.ID, client);
        }

        InstantiateClients(fromReservation);
    }

    private void OpenClientScreen(IClient client)
    {
        selectClientCallback(client);
        navigator.GoBack();
    }

    private void OpenClientAdminScreen(IClient client)
    {
        LastPosition();
        clientEditScreenTransform.GetComponent<ClientsEditScreen>().SetCurrentClient(client);
        clientAdminScreenTransform.GetComponent<ClientsAdminScreen>().SetCurrentClient(client);
        navigator.GoTo(clientAdminScreenTransform.GetComponent<NavScreen>());
        ClearSearchField();
    }
    private void OpenEditAdminScreen(IClient client)
    {
        SetTextOnEditPanel();
        saveButton.gameObject.SetActive(false);
        editButton.gameObject.SetActive(true);
        clientEditScreenTransform.GetComponent<ClientsEditScreen>().SetCurrentClient(client);
        navigator.GoTo(clientEditScreenTransform.GetComponent<NavScreen>());
        ClearSearchField();
    }

    #region phoneSmsEmail
    public void EmailUs(IClient currentClient)
    {
        if (string.IsNullOrEmpty(currentClient.Email))
        {
            inputManager.Message(Constants.MessageEmail);
        }
        else
        {
            string subject = MyEscapeURL("Custom application development");
            Application.OpenURL("mailto:" + currentClient.Email + "?subject=" + subject);
        }
    }

    string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    public void phoneUS(IClient currentClient = null)
    {
        Application.OpenURL("tel:" + currentClient.Number);
    }

    public void SmsUs(IClient currentClient = null)
    {
        Application.OpenURL("sms:" + currentClient.Number);
    }
    #endregion

    public void SearchForClient()
    {
        if (!string.IsNullOrEmpty(searchField.text))
        {
            foreach (var item in letterButtons)
            {
                item.SetActive(false);
            }
            foreach (var client in clientButtons)
            {

                if (client.GetComponent<ClientButton>().SearchClients(searchField.text))
                {
                    client.SetActive(true);

                }
                else
                {
                    client.SetActive(false);
                }
            }

        }
        else
        {
            foreach (var item in clientButtons)
            {
                item.SetActive(true);
            }
            foreach (var item in letterButtons)
            {
                item.SetActive(true);
            }
        }

    }

    public void SetFunctionsOnAddButton()
    {
        SetTextOnAddPanel();
        saveButton.gameObject.SetActive(true);
        editButton.gameObject.SetActive(false);
        ClearSearchField();
    }

    public void SetTextOnAddPanel()
    {
        headerBarText.text = Constants.NEW_CLIENT;
    }

    public void SetTextOnEditPanel()
    {
        headerBarText.text = Constants.EDIT_CLIENT;
    }
    #region ClearFieldsForClient
    public void ClearClientFields()
    {
        clientName.text = string.Empty;
        clientPhone.text = string.Empty;
        clientAdress.text = string.Empty;
        clientEmail.text = string.Empty;
    }
    public void ClearSearchField()
    {
        searchField.text = string.Empty;
    }
    #endregion

    #region Animations
    public void SearchFieldShow(bool value)
    {
        StartCoroutine(Animate(value ? initialSizeSearch : 0, value));
        if(!value)
        {
            ClearSearchField();
        }
    }
    private IEnumerator Animate(float target, bool value)
    {
        float timer = 0.15f;
        float init = Search.sizeDelta.y;

        if (value)
        {
            //SHOW animation for search field and clients
            Search.sizeDelta = new Vector2(Search.sizeDelta.x, 0);
            Search.gameObject.SetActive(true);
            for (float i = 0; i < timer; i += Time.deltaTime)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutContent);
                Search.sizeDelta = new Vector2(Search.sizeDelta.x, Mathf.Lerp(0, initialSizeSearch, i / timer));
                ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, Mathf.Lerp(initialClientContainer, initialClientContainer - 280, i / timer));
                yield return null;
            }
            Search.sizeDelta = new Vector2(Search.sizeDelta.x, initialSizeSearch);
            ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, initialClientContainer - 280);
        }
        else
        {
            //HIDE animation for search field and clients
            for (float i = 0; i < timer; i += Time.deltaTime)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutContent);
                Search.sizeDelta = new Vector2(Search.sizeDelta.x, Mathf.Lerp(initialSizeSearch, 0, i / timer));
                ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, Mathf.Lerp(initialClientContainer - 280, initialClientContainer, i / timer));
                yield return null;
            }
            Search.sizeDelta = new Vector2(Search.sizeDelta.x, 0);
            ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, initialClientContainer);
        }


        Search.gameObject.SetActive(value);
    }
    #endregion
}
