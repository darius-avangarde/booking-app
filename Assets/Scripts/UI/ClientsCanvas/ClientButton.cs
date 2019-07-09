using System;
using UnityEngine;
using UnityEngine.UI;

public class ClientButton : MonoBehaviour
{
    [SerializeField]
    private Text ClientName = null;
    [SerializeField]
    private Text phoneNumber = null;
    [SerializeField]
    private Text clientEmail = null;
    [SerializeField]
    private Text clientAdress = null;
    [SerializeField]
    private Button clientButton;
    [SerializeField]
    private Button phoneButton;
    [SerializeField]
    private Button smsButton;
    [SerializeField]
    private Button mailButton;
    [SerializeField]
    private Button editButton;
    [SerializeField]
    private RectTransform clientBtn;
    [SerializeField]
    private Toggle clientToggle;
    public void Initialize(IClient client, /*Action<IClient> callback, */Action<IClient> phoneCallBack, Action<IClient> smsCallback, Action<IClient> mailCallback, Action<IClient> editCallback)
    {
        ClientName.text = client.Name;
        phoneNumber.text = client.Number; //$"{ client.Number} { client.Email}";
        clientEmail.text = client.Email;
        clientAdress.text = client.Adress;
        //clientButton.onClick.AddListener(() => callback(client));
        phoneButton.onClick.AddListener(() => phoneCallBack(client));
        smsButton.onClick.AddListener(() => smsCallback(client));
        mailButton.onClick.AddListener(() => mailCallback(client));
        editButton.onClick.AddListener(() => editCallback(client));
    }

    public void AnimationPrefab()
    {
        if (clientToggle.isOn)
        {
            if (string.IsNullOrEmpty(phoneNumber.text) == false )
            {
                phoneNumber.gameObject.SetActive(true);
            }
            if (string.IsNullOrEmpty(clientEmail.text) == false)
            {
                clientEmail.gameObject.SetActive(true);
            }
           
            //clientAdress.gameObject.SetActive(true);
            clientBtn.sizeDelta = new Vector2(900, 425);
        }
        else
        {
            phoneNumber.gameObject.SetActive(false);
            clientEmail.gameObject.SetActive(false);
            clientAdress.gameObject.SetActive(false);
            clientBtn.sizeDelta = new Vector2(900, 189);
            Debug.Log("stop anim");
        }
    }
    public bool SearchClients(string input)
    {
        bool ok = false;

        if (ClientName.text.ToLower().Trim().StartsWith(input.ToLower().Trim()) || phoneNumber.text.ToLower().Trim().StartsWith(input.ToLower().Trim()) || clientEmail.text.ToLower().Trim().StartsWith(input.ToLower().Trim()) || clientAdress.text.ToLower().Trim().StartsWith(input.ToLower().Trim()))
        {
            ok = true;
        }
        return ok;
    }

}
