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
    private Button editButton;
    [SerializeField]
    private Button deleteButton;
    

    public void Initialize(IClient client, Action<IClient> callback, Action<IClient> editCallBack, Action<IClient> deleteCallback)
    {
        ClientName.text = client.Name;
        phoneNumber.text = client.Number ;
        clientButton.onClick.AddListener(() => callback(client));
        editButton.onClick.AddListener(() => editCallBack(client));
        deleteButton.onClick.AddListener(() => deleteCallback(client));
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
