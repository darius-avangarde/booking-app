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
    private List<GameObject> clientButtons = new List<GameObject>();
    private bool hasCallBack = false;
    private UnityAction<IClient> saveCallBack;
    private UnityAction<bool> cancelCallback;
    public RectTransform Search;
    public RectTransform LayoutContent;
    private float initialSizeSearch;

    private void Start()
    {
        initialSizeSearch = Search.rect.height;
        Search.gameObject.SetActive(false);
    }
    public void InstantiateClients()
    {
        foreach (var clientButton in clientButtons)
        {
            Destroy(clientButton);
        }
        clientButtons.Clear();
        foreach (var client in ClientDataManager.GetClients())
        {
            GameObject clientButton = Instantiate(clientPrefabButton, clientInfoContent);
            clientButton.GetComponent<ClientButton>().Initialize(client, OpenClientAdminScreen, phoneUS, SmsUs, EmailUs, OpenEditAdminScreen);//,OpenDeleteAdminScreen);
            clientButtons.Add(clientButton);
        }
    }

    public void InstantiateClientsButtons()
    {
        foreach (var clientButton in clientButtons)
        {
            Destroy(clientButton);
        }
        clientButtons.Clear();
        foreach (var client in ClientDataManager.GetClients())
        {
            GameObject clientButton = Instantiate(clientPrefabButton, clientInfoContent);
            clientButton.GetComponent<ClientButton>().InitializeClient(client, OpenClientScreen);
            Debug.Log("instantiate");
            clientButtons.Add(clientButton);
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
            textNameRequired.text = "";
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
        Debug.Log("callback");
        navigator.GoTo(clientReservationScreenTransform.GetComponent<NavScreen>());
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
        clientName.text = "";
        clientPhone.text = "";
        clientAdress.text = "";
        clientEmail.text = "";
    }

    public void ClearSearchField()
    {
        searchField.text = "";
    }

    public void SearchFieldShow(bool value)
    {
        StartCoroutine(Animate(value ? initialSizeSearch : 0, value));
    }
    private IEnumerator Animate(float target, bool value)
    {
        if(value)
        {
            Search.sizeDelta = new Vector2(Search.sizeDelta.x, 0);
            Search.gameObject.SetActive(true);
        }
        float timer = 0.15f;
        float init = Search.sizeDelta.y;
        for (float i = 0; i < timer; i += Time.deltaTime)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutContent);
            Search.sizeDelta = new Vector2(Search.sizeDelta.x, Mathf.Lerp(init, target, i / timer));
            yield return null;
        }

        Search.sizeDelta = new Vector2(Search.sizeDelta.x, target);
        Search.gameObject.SetActive(value);
    }
}