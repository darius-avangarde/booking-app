using System;
using UnityEngine;
using UnityEngine.UI;

public class ClientButton : MonoBehaviour
{
    [SerializeField]
    public Text clientName = null;
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
        clientName.text = client.Name;
        phoneNumber.text = client.Number ;
        clientButton.onClick.AddListener(() => callback(client));
        editButton.onClick.AddListener(() => editCallBack(client));
        deleteButton.onClick.AddListener(() => deleteCallback(client));
    }

    
}
