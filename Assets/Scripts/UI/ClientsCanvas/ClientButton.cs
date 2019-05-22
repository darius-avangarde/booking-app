using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientButton : MonoBehaviour
{
    [SerializeField]
    public Text clientName = null;
    [SerializeField]
    private Text phoneNumber = null;
    public Button clientButton;
    public  Button editButton;
    public Button deleteButton;

    public void Initialize(IClient client, Action<IClient> callback, Action<IClient> editCallBack, Action<IClient> deleteCallback)
    {
        clientName.text = string.IsNullOrEmpty(client.Name) ? Constants.defaultRoomAdminScreenName : client.Name;
        phoneNumber.text = client.Number ;
        clientButton.onClick.AddListener(() => callback(client));
        editButton.onClick.AddListener(() => editCallBack(client));
        deleteButton.onClick.AddListener(() => deleteCallback(client));
    }

    
}
