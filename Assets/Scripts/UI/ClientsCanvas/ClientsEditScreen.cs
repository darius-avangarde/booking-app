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
        if (RegexUtilities.IsValidEmail(clientEmail.text.ToString()))
        {
            Debug.Log("Email correct");
            Debug.Log(clientEmail.text);
        }

        else

        {
           
            Debug.Log("Invalid adress");
            textError.text = "Please insert a valid e-mail!";
        }
    }

}
