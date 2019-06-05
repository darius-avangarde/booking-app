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
    private Button clientButton;
    [SerializeField]
    private Button phoneButton;
    [SerializeField]
    private Button smsButton;
    [SerializeField]
    private Button mailButton;
    [SerializeField]
    private Button editButton;

    public void Initialize(IClient client, Action<IClient> callback, Action<IClient> phoneCallBack, Action<IClient> smsCallback, Action<IClient> mailCallback, Action<IClient> editCallback)
    {
        ClientName.text = client.Name;
        phoneNumber.text = client.Number;
        clientButton.onClick.AddListener(() => callback(client));
        phoneButton.onClick.AddListener(() => phoneCallBack(client));
        smsButton.onClick.AddListener(() => smsCallback(client));
        mailButton.onClick.AddListener(() => mailCallback(client));
        editButton.onClick.AddListener(() => editCallback(client));
    }



    public bool SearchClients(string input)
    {
        bool ok = false;

        if (ClientName.text.ToLower().Trim().StartsWith(input.ToLower().Trim()))
        {
            ok = true;
        }
        return ok;
    }

}
