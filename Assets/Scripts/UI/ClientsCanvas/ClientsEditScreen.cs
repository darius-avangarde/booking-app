using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientsEditScreen : MonoBehaviour
{
    [SerializeField]
    private Transform clientEditScreenTransform = null;
    [SerializeField]
    private InputField clientName;
    [SerializeField]
    private InputField clientPhone;
    [SerializeField]
    private InputField clientAdress;
    [SerializeField]
    private InputField clientEmail;
    private IClient currentClient;

    public IClient GetCurrentClient()
    {
        return currentClient;
    }

    public void EditCurrentClient()
    {
        //SetCurrentClient(currentClient);
        //var currentClient = clientEditScreenTransform.GetComponent<ClientsAdminScreen>().GetCurrentClient();
        //SetClientsFieldsText();
    }

    public void SetCurrentClient(IClient client)
    {
        currentClient = client;
        Debug.Log("client name is:" + currentClient.Name);
        SetClientsFieldsText();
    }
    public void SetClientsFieldsText()
    {
        clientName.text =  currentClient.Name;
        clientPhone.text = currentClient.Number;
        clientAdress.text = currentClient.Adress;
        clientEmail.text = currentClient.Email;
    }

    

}
