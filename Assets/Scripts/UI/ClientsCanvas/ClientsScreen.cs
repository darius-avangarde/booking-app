using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;
using static ClientDataManager;

public class ClientsScreen : MonoBehaviour
{

    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform clientAdminScreenTransform = null;
    [SerializeField]
    private Transform clientEditScreenTransform = null;
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
    private List<GameObject> clientButtons = new List<GameObject>();
   

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
            clientButton.GetComponent<ClientButton>().Initialize(client, OpenClientAdminScreen,OpenEditAdminScreen, OpenDeleteAdminScreen);
            clientButtons.Add(clientButton);
        }
    }
    
    public void SaveAddedClient()
    {
        Client client = new Client();
        if (clientName.text == " ")
        {
            textNameRequired.text = Constants.NameRequired;
        }
        else
       
        {
            textNameRequired.text = " ";
            client.Name = clientName.text;
            client.Number = clientPhone.text;
            client.Adress = clientAdress.text;
            client.Email = clientEmail.text;
            ClientDataManager.AddClient(client);
        }
        
    }

    public void EditClient()
    {
        var currentClient = clientAdminScreenTransform.GetComponent<ClientsAdminScreen>().GetCurrentClient();
        Client client = new Client();
        if (clientName.text == " ")
        {
            textNameRequired.text = Constants.NameRequired;
        }
        else
      
        {
            textNameRequired.text = " ";
            client.Name = clientName.text;
            client.Number = clientPhone.text;
            client.Adress = clientAdress.text;
            client.Email = clientEmail.text;
            ClientDataManager.EditClient(currentClient.ID, client);
        }
    }


    private void OpenDeleteAdminScreen(IClient client)
    {
        clientAdminScreenTransform.GetComponent<ClientsAdminScreen>().SetCurrentClient(client);
        clientAdminScreenTransform.GetComponent<ClientsAdminScreen>().DeleteClient(InstantiateClients);
    }

    private void OpenClientAdminScreen(IClient client)
    {
        clientEditScreenTransform.GetComponent<ClientsEditScreen>().SetCurrentClient(client);
        clientAdminScreenTransform.GetComponent<ClientsAdminScreen>().SetCurrentClient(client);
        navigator.GoTo(clientAdminScreenTransform.GetComponent<NavScreen>());
    }
    private void OpenEditAdminScreen(IClient client)
    {
        SetTextOnEditPanel();
        saveButton.gameObject.SetActive(false);
        editButton.gameObject.SetActive(true);
        clientEditScreenTransform.GetComponent<ClientsEditScreen>().SetCurrentClient(client);
        navigator.GoTo(clientEditScreenTransform.GetComponent<NavScreen>());
    }

    public void SearchForClient()
    {
        if (!string.IsNullOrEmpty(searchField.text))
        {

            foreach (var client in clientButtons)
            {
              
                if(client.GetComponent<ClientButton>().SearchClients(searchField.text))
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
        headerBarText.text = "Client Nou";
    }

    public void SetTextOnEditPanel()
    {
        headerBarText.text = "Editeaza client";
    }

    public void ClearClientFields()
    {
        clientName.text = " ";
        clientPhone.text = " ";
        clientAdress.text = " ";
        clientEmail.text = " ";
    }
    
}