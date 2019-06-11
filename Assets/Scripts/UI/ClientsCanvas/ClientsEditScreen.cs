using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClientsEditScreen : MonoBehaviour
{
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
            textError.text = "Inserează o adresă de email validă!";
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
            textNameRequired.text = "Te rog adaugă un număr de telefon!";
            textNameRequired.gameObject.SetActive(true);
        }
        else
        {
            textNameRequired.text = string.Empty;
            textNameRequired.gameObject.SetActive(false);
        }
    }
}
