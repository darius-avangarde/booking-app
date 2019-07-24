using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClientPicker : MonoBehaviour, IClosable
{
    [SerializeField]
    private InputField searchFieldInput;
    [SerializeField]
    private RectTransform searchResultsContent;
    [SerializeField]
    private GameObject searchResultsParent;
    [SerializeField]
    private GameObject clientPickerObjectPrefab;
    [SerializeField]
    private DropDownAnimationExtended dropDownAnimation;

    [Space]
    [SerializeField]
    private GameObject closeButton;
    [SerializeField]
    private float getClientsDelay = 0.5f;
    [SerializeField]
    private RectTransform loadIndicator;
    [SerializeField]
    private Text noMatchesText;

    private List<ClientPickerObject> clientObjects = new List<ClientPickerObject>();
    private UnityAction<IClient> setCallback;
    private bool isOpen = false;


    public void SetCallback(UnityAction<IClient> setClientCallback, IClient selectedClient = null)
    {
        noMatchesText.gameObject.SetActive(false);
        searchFieldInput.onValueChanged.RemoveAllListeners();
        searchFieldInput.text = selectedClient != null ? selectedClient.Name : string.Empty;
        setCallback = setClientCallback;
        searchFieldInput.onValueChanged.AddListener((s) => InitiateClientSearch());
    }

    public void InitiateClientSearch(bool isTap = false)
    {
        closeButton.SetActive(true);
        InputManager.CurrentlyOpenClosable = this;
        isOpen = true;
        searchResultsParent.SetActive(true);
        if(!isTap) CancelSearch();
        StartCoroutine(DelayedGetClients());
    }

    public void Close()
    {
        closeButton.SetActive(false);
        InputManager.CurrentlyOpenClosable = null;
        loadIndicator.rotation = Quaternion.identity;
        isOpen = false;
        CancelSearch();
        searchResultsParent.SetActive(false);
        loadIndicator.gameObject.SetActive(false);
        loadIndicator.rotation = Quaternion.identity;
    }

    private void UpdateSearchFieldText(IClient client)
    {
        searchFieldInput.onValueChanged.RemoveAllListeners();
        searchFieldInput.text = client.Name;
        searchFieldInput.onValueChanged.AddListener((s) => InitiateClientSearch());
    }

    private void UpdateClientList(List<IClient> matches)
    {
        while(matches.Count > clientObjects.Count)
        {
            clientObjects.Add(Instantiate(clientPickerObjectPrefab, searchResultsContent).GetComponent<ClientPickerObject>());
        }

        for (int i = 0; i < clientObjects.Count; i++)
        {
            if(i < matches.Count)
                clientObjects[i].SetAndEnable(matches[i], ClientButtonAction);
            else
                clientObjects[i].Disable();
        }

        if(matches.Count == 0)
        {
            noMatchesText.gameObject.SetActive(true);
            noMatchesText.text = "Nu s-a găsit niciun client";
        }
        else
        {
            noMatchesText.gameObject.SetActive(false);
        }
    }

    private void ClientButtonAction(IClient client)
    {
        UpdateSearchFieldText(client);
        setCallback?.Invoke(client);
        Close();
    }

    private void CancelSearch()
    {
        StopAllCoroutines();
        loadIndicator.gameObject.SetActive(false);
    }

    private IEnumerator DelayedGetClients()
    {
        loadIndicator.gameObject.SetActive(true);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime/getClientsDelay){
            loadIndicator.Rotate(Vector3.forward, -5);
            yield return null;
        }
        loadIndicator.gameObject.SetActive(false);
        loadIndicator.rotation = Quaternion.identity;

        UpdateClientList(GetMatchingClients(searchFieldInput.text.ToLower()));
        dropDownAnimation.ManualAnimationTrigger();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(searchResultsContent);
    }

    private List<IClient> GetMatchingClients(string inputString)
    {
        if(string.IsNullOrEmpty(inputString))
        {
            return ClientDataManager.GetClients().ToList();
        }
        return ClientDataManager.GetClients().Where(c => c.Name.ToLower().StartsWith(inputString) || c.Number.ToLower().Contains(inputString)).OrderBy(c => c.Name).ToList();
    }
}
