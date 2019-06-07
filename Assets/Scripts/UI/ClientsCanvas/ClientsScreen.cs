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

    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform clientAdminScreenTransform = null;
    [SerializeField]
    private Transform clientEditScreenTransform = null;
    [SerializeField]
    private Transform clientReservationScreenTransform = null;
    [SerializeField]
    private GameObject clientPrefabButton = null;
    [SerializeField]
    private GameObject clientprefabLetter = null;
    [SerializeField]
    private Transform clientInfoContent = null;
    [SerializeField]
    private InputField searchField = null;
    [SerializeField]
    private Transform addClientButton;
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
    private Text textNameRequired;
    [SerializeField]
    private Text textEmailRequired;
    #region PrivateVariables
    private List<GameObject> clientButtons = new List<GameObject>();
    private List<GameObject> letterButtons = new List<GameObject>();
    private bool hasCallBack = false;
    private UnityAction<IClient> saveCallBack;
    private UnityAction<IClient> selectClientCallback;
    private UnityAction<bool> cancelCallback;
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
        ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, -178 - initialSizeSearch);
        initialClientContainer = ClientContainer.sizeDelta.y;
    }


    public void InstantiateClients()
    {
        foreach (var clientButton in clientButtons)
        {
            Destroy(clientButton);
        }

        foreach (var letterButton in letterButtons)
        {
            Destroy(letterButton);
        }
        clientButtons.Clear();
        letterButtons.Clear();
        foreach (var item in ClientDataManager.GetClientsToDictionary())
        {
            clientprefabLetter.GetComponent<Text>().text = item.Key.ToString().ToUpper();
            GameObject clientLetters = Instantiate(clientprefabLetter, clientInfoContent);
            letterButtons.Add(clientLetters);
            foreach (var client in item.Value)
            {
                GameObject clientButton = Instantiate(clientPrefabButton, clientInfoContent);
                clientButton.GetComponent<ClientButton>().Initialize(client, OpenClientAdminScreen, phoneUS, SmsUs, EmailUs, OpenEditAdminScreen);
                clientButtons.Add(clientButton);
            }
        }
    }

    public void OpenClientReservation(UnityAction<IClient> callback)
    {
        InstantiateClientsButtons();
        selectClientCallback = callback;
    }

    public void InstantiateClientsButtons()
    {
        foreach (var clientButton in clientButtons)
        {
            Destroy(clientButton);
        }
        foreach (var letterButton in letterButtons)
        {
            Destroy(letterButton);
        }
        clientButtons.Clear();
        letterButtons.Clear();
        foreach (var item in ClientDataManager.GetClientsToDictionary())
        {
            clientprefabLetter.GetComponent<Text>().text = item.Key.ToString().ToUpper();
            GameObject clientLetters = Instantiate(clientprefabLetter, clientInfoContent);
            letterButtons.Add(clientLetters);
            foreach (var client in item.Value)
            {
                GameObject clientButton = Instantiate(clientPrefabButton, clientInfoContent);
                clientButton.GetComponent<ClientButton>().InitializeClient(client, OpenClientScreen);
                clientButtons.Add(clientButton);
            }
        }
    }

    public void SaveAddedClient()
    {

        if (String.IsNullOrEmpty(clientName.text))
        {
            textNameRequired.text = Constants.NameRequired;
        }
        else
        {
            Client client = new Client();
            textNameRequired.text = string.Empty;
            client.Name = clientName.text;
            client.Number = clientPhone.text;
            client.Adress = clientAdress.text;
            client.Email = clientEmail.text;
            ClientDataManager.AddClient(client);
            if (hasCallBack)
            {
                saveCallBack.Invoke(client);
                cancelCallback.Invoke(true);
            }
            hasCallBack = false;
        }
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
        if (String.IsNullOrEmpty(clientName.text))
        {
            textNameRequired.text = Constants.NameRequired;
        }
        else

        {
            Client client = new Client();
            textNameRequired.text = "";
            client.Name = clientName.text;
            client.Number = clientPhone.text;
            client.Adress = clientAdress.text;
            client.Email = clientEmail.text;
            ClientDataManager.EditClient(currentClient.ID, client);
        }
    }

    private void OpenClientScreen(IClient client)
    {
        selectClientCallback(client);
        navigator.GoBack();
    }
    private void OpenDeleteAdminScreen(IClient client)
    {
        clientAdminScreenTransform.GetComponent<ClientsAdminScreen>().SetCurrentClient(client);
        clientAdminScreenTransform.GetComponent<ClientsAdminScreen>().DeleteClient(InstantiateClients);
        ClearSearchField();
    }

    private void OpenClientAdminScreen(IClient client)
    {
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

    //------------
    public void EmailUs(IClient currentClient)
    {
        if (currentClient.Email == null)
        {
            textEmailRequired.text = "Nu există email înregistrat!";
        }
        else
        {
            //subject of the mail
            string subject = MyEscapeURL("Custom application development");
            //Open the Default Mail App
            Application.OpenURL("mailto:" + currentClient.Email + "?subject=" + subject);
            Debug.Log(currentClient.Email + "email is:");
        }
    }

    string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    public void phoneUS(IClient currentClient = null)
    {
        Application.OpenURL("tel://" + currentClient.Number);
        Debug.Log(currentClient.Number + "client number");
    }

    public void SmsUs(IClient currentClient = null)
    {
        Application.OpenURL("sms://" + currentClient.Number);
    }

    //-----------

    public void SearchForClient()
    {
        if (!string.IsNullOrEmpty(searchField.text))
        {

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
        }

    }

    public void SetAddButton()
    {
        addClientButton.SetAsFirstSibling();
    }

    public void SetTextOnAddPanel()
    {
        headerBarText.text = "Client nou";
    }

    public void SetTextOnEditPanel()
    {
        headerBarText.text = "Editează client";
    }

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

    //forget about this code. It doesn't exist.
    #region Animations
    public void SearchFieldShow(bool value)
    {
        StartCoroutine(Animate(value ? initialSizeSearch : 0, value));
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
                ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, Mathf.Lerp(initialClientContainer, initialClientContainer - 208, i / timer));
                yield return null;
            }
            Search.sizeDelta = new Vector2(Search.sizeDelta.x, initialSizeSearch);
            ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, initialClientContainer - 208);
        }
        else
        {
            //HIDE animation for search field and clients
            for (float i = 0; i < timer; i += Time.deltaTime)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutContent);
                Search.sizeDelta = new Vector2(Search.sizeDelta.x, Mathf.Lerp(initialSizeSearch, 0, i / timer));
                ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, Mathf.Lerp(initialClientContainer - 208, initialClientContainer, i / timer));
                yield return null;
            }
            Search.sizeDelta = new Vector2(Search.sizeDelta.x, 0);
            ClientContainer.sizeDelta = new Vector2(ClientContainer.sizeDelta.x, initialClientContainer);
        }


        Search.gameObject.SetActive(value);
        //separator.gameobject.setactive(value)
    }
    #endregion
}