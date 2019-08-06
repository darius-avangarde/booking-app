﻿using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using static ClientDataManager;
using System.Collections;

public class ClientsScreen : MonoBehaviour
{
    #region SerializeFieldVariables
    [SerializeField]
    private ThemeManager theme = null;
    [SerializeField]
    private Navigator navigator = null;
    [SerializeField]
    private Transform clientEditScreenTransform = null;
    [SerializeField]
    private Shadow clientPrefab;
    [SerializeField]
    private GameObject clientprefabLetter = null;
    [SerializeField]
    private RectTransform clientInfoContent = null;
    [SerializeField]
    private InputField searchField = null;
    [SerializeField]
    private Text headerAddText;
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
    private GameObject deleteButton;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private UI_ScrollRectOcclusion scrollRectComponent = null;
    [SerializeField]
    private ScrollRect clientsScrollView = null;
    [SerializeField]
    private Toggle clientToggle;
    #endregion
    #region PrivateVariables
    private List<GameObject> clientButtons = new List<GameObject>();
    private List<GameObject> letterButtons = new List<GameObject>();
    private bool hasCallBack = false;
    private UnityAction<IClient> saveCallBack;
    private UnityAction<IClient> selectClientCallback;
    private UnityAction<bool> cancelCallback;
    private bool fromReservation = false;
    private float scrollPosition = 1;
    #endregion
    #region AnimationVariables
    public RectTransform Search;
    public RectTransform ClientContainer;
    private Vector2 firstPosition;
    private Vector2 firstPositionContainer;
    #endregion

    private void Start()
    {
        firstPosition = new Vector2(0, 0);
        firstPositionContainer = ClientContainer.offsetMax;
    }
    #region AnimationSearch
    public void SearchAnimation()
    {
        if (clientToggle.isOn)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }
    public void FadeIn()
    {
        Search.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Move(Search, ClientContainer, new Vector2(0, -120), new Vector2(0, -260)));
    }
    public void FadeOut()
    {
        Search.gameObject.SetActive(false);
        ClearSearchField();
        StopAllCoroutines();
        StartCoroutine(Move(Search,ClientContainer, firstPosition, firstPositionContainer));
    }
    IEnumerator Move(RectTransform rt,RectTransform container, Vector2 targetPos, Vector2 targetPosContainer)
    {
        float step = 0;
        while (step < 1)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, targetPos, step += Time.deltaTime);
            container.offsetMax = Vector2.Lerp(container.offsetMax, targetPosContainer, step += Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion
    public void InstantiateClients(bool fromReservation = false)
    {
        this.fromReservation = fromReservation;
        scrollRectComponent.ResetAll();
        
        for (int i = 0; i < clientButtons.Count; i++)
        {
            DestroyImmediate(clientButtons[i]);
        }
        for (int i = 0; i < letterButtons.Count; i++)
        {
            DestroyImmediate(letterButtons[i]);
        }
        clientButtons.Clear();
        letterButtons.Clear();
        foreach (var item in ClientDataManager.GetClientsToDictionary())
        {
            clientprefabLetter.GetComponent<Text>().text = item.Key.ToString().ToUpper();
            GameObject clientLetters = Instantiate(clientprefabLetter, clientInfoContent);
            letterButtons.Add(clientLetters);
            SetTheme(clientLetters);
            foreach (Client client in item.Value)
            {
                theme.SetShadow(clientPrefab);
                GameObject clientButton = Instantiate(clientPrefab.gameObject, clientInfoContent);
                
                if (fromReservation)
                    clientButton.GetComponent<ClientButton>().Initialize(client,SetTheme , phoneUS, SmsUs, EmailUs, OpenEditAdminScreen);
                else
                    clientButton.GetComponent<ClientButton>().Initialize(client, SetTheme, phoneUS, SmsUs, EmailUs, OpenEditAdminScreen);
                clientButtons.Add(clientButton);
            }
        }
        /* LayoutRebuilder.ForceRebuildLayoutImmediate(clientInfoContent);
         Canvas.ForceUpdateCanvases();
         clientsScrollView.verticalNormalizedPosition = scrollPosition;
         if (clientsScrollView.content.childCount > 0)
         {
             scrollRectComponent.Init();
         }*/
    }
    public void SetTheme(GameObject myObj)
    {
        Graphic item = myObj.GetComponent<Graphic>();
       theme.SetColor(item);
    }
    public void LastPosition()
    {
        scrollPosition = clientsScrollView.verticalNormalizedPosition;
    }
    public void OpenClientReservation(UnityAction<IClient> callback)
    {
        InstantiateClients(true);
        selectClientCallback = callback;
    }

    private void SetClient(Client client)
    {
        client.Name = clientName.text;
        client.Number = clientPhone.text;
        client.Email = clientEmail.text;
        client.Adress = clientAdress.text;
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

    private void OpenClientScreen(IClient client)
    {
        selectClientCallback(client);
        navigator.GoBack();
    }

    private void OpenEditAdminScreen(IClient client)
    {
        SetTextOnEditPanel();
        saveButton.gameObject.SetActive(false);
        editButton.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true);
        clientEditScreenTransform.GetComponent<ClientsEditScreen>().SetCurrentClient(client);
        navigator.GoTo(clientEditScreenTransform.GetComponent<NavScreen>());
        ClearSearchField();
    }

    #region phoneSmsEmail
    public void EmailUs(IClient currentClient)
    {
        if (string.IsNullOrEmpty(currentClient.Email))
        {
            inputManager.Message(LocalizedText.Instance.MailRequired);
        }
        else
        {
            string subject = MyEscapeURL("Custom application development");
            Application.OpenURL("mailto:" + currentClient.Email + "?subject=" + subject);
        }
    }

    string MyEscapeURL(string url)
    {

        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    public void phoneUS(IClient currentClient = null)
    {
        if (string.IsNullOrEmpty(currentClient.Number))
        {
            inputManager.Message(LocalizedText.Instance.PhoneRequired);
        }
        else
        {
            Application.OpenURL("tel:" + currentClient.Number);
        }
    }

    public void SmsUs(IClient currentClient = null)
    {
        if (string.IsNullOrEmpty(currentClient.Number))
        {
            inputManager.Message(LocalizedText.Instance.PhoneRequired);
        }
        else
        {
            Application.OpenURL("sms:" + currentClient.Number);
        }
    }
    #endregion

    public void SearchForClient()
    {
        if (!string.IsNullOrEmpty(searchField.text))
        {
            for (int i = 0; i < letterButtons.Count; i++)
            {
                letterButtons[i].SetActive(false);
            }
       
            foreach (GameObject client in clientButtons)
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
            for (int i = 0; i < clientButtons.Count; i++)
            {
                clientButtons[i].SetActive(true);
            }
            for (int i = 0; i < letterButtons.Count; i++)
            {
                letterButtons[i].SetActive(true);
            }
        }

    }

    public void SetFunctionsOnAddButton()
    {
        SetTextOnAddPanel();
        saveButton.gameObject.SetActive(true);
        editButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        ClearSearchField();
        navigator.GoTo(clientEditScreenTransform.GetComponent<NavScreen>());
    }

    public void SetTextOnAddPanel()
    {
        headerAddText.text = LocalizedText.Instance.HeaderClients[0];
    }

    public void SetTextOnEditPanel()
    {
        headerAddText.text = LocalizedText.Instance.HeaderClients[1];
    }
    #region ClearFieldsForClient
    public void ClearClientFields()
    {
        clientName.text = string.Empty;
        clientPhone.text = string.Empty;
        clientAdress.text = string.Empty;
        clientEmail.text = string.Empty;
    }
    public void ClearSearchField()
    {
        searchField.text = string.Empty;
    }
    #endregion
}