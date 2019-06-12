﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ClientPicker : MonoBehaviour
{

    [SerializeField]
    private ReservationEditScreen reservationEdit = null;
    [SerializeField]
    private ClientsScreen clientScreen = null;

    [Space]
    [SerializeField]
    private InputField clientNameField = null;
    [SerializeField]
    private Transform searchLoadImage = null;
    [SerializeField]
    private GameObject clientObjPrefab = null;
    [SerializeField]
    private GameObject scrolRectGameObj = null;
    [SerializeField]
    private Transform clientObjContainer = null;
    [SerializeField]
    private GameObject addClientButton = null;
    [SerializeField]
    private GameObject noClientsFoundText = null;

    [Space]
    [SerializeField]
    private float searchDelay = 1.0f;
    [SerializeField]
    private int maxClientsInPicker = 100;

    [Space]
    [SerializeField]
    private UnityEvent onClickAddNewClient = null;


    private List<GameObject> clientsInList = new List<GameObject>();
    private string currentQuerry;
    private WaitForSeconds waitTime;
    private ScrollRect scrolRect;

    private void OnEnable()
    {
        clientNameField.onValueChanged.AddListener(FilterClients);
        waitTime = new WaitForSeconds(searchDelay);
        addClientButton.SetActive(false);
        scrolRectGameObj.SetActive(false);
        noClientsFoundText.SetActive(false);
        searchLoadImage.gameObject.SetActive(false);
        scrolRect = scrolRectGameObj.GetComponentInChildren<ScrollRect>();
    }

    //Add client function attached to the add new client button in the client picker scrollrect
    public void AddNewClient()
    {
        clientScreen.OpenAddClient(SelectAction, NewClientCancel);
        clientScreen.ClearClientFields();
        clientScreen.SetTextOnAddPanel();
        if(onClickAddNewClient != null)
        {
            onClickAddNewClient.Invoke();
        }
        scrolRectGameObj.SetActive(false);
    }

    public void NewClientCancel(bool b)
    {
        if (b)
        {
            reservationEdit.AllowEdit = true;
        }
    }

    //triggered when the user taps outside the scrolview rect containing the client names
    public void CancelSearch()
    {
        SelectAction(null);
    }

    //callback assigned to the client picker objects (sets the active client in the edit reservation screen and concludes search)
    internal void SelectAction(IClient client)
    {
        //reservationEdit.SetClient(client);
        scrolRectGameObj.SetActive(false);
        addClientButton.SetActive(false);
    }

    //Filters the input field value on change if allowed to search, and starts load coroutine before populating scrolrect with matching clients
    private void FilterClients(string value)
    {
        if(reservationEdit.AllowEdit)
        {
            if(!scrolRectGameObj.activeSelf)
            {
                scrolRectGameObj.SetActive(true);
            }

            noClientsFoundText.SetActive(false);
            StopAllCoroutines();
            ClearObjects();
            currentQuerry = value;
            StartCoroutine(DelayedDisplay());
        }
        else if(scrolRectGameObj.activeSelf)
        {
            scrolRectGameObj.SetActive(false);
        }
    }

    //coroutine handling waiting for designated time before fetching data and populating scrolrect
    private IEnumerator DelayedDisplay()
    {
        addClientButton.SetActive(false);
        searchLoadImage.gameObject.SetActive(true);
        for (float t = 0; t < searchDelay; t += Time.deltaTime/searchDelay)
        {
            searchLoadImage.rotation *= Quaternion.AngleAxis(-10, Vector3.forward);
            yield return null;
        }
        searchLoadImage.gameObject.SetActive(false);

        List<IClient> matches = ClientDataManager.GetClients()
            .Where(c => CompareString(currentQuerry, c.Name)).OrderBy(cl => cl.Name).ToList();

        scrolRect.normalizedPosition = Vector2.zero;
        addClientButton.SetActive(true);

        for (int c = 0; c < Mathf.Min(matches.Count, maxClientsInPicker); c++)
        {
            clientsInList.Add(Instantiate(clientObjPrefab, clientObjContainer));
            clientsInList[c].GetComponent<ClientPickerObject>().Initialize(matches[c], SelectAction);
        }

        if(matches.Count == 0)
        {
            noClientsFoundText.SetActive(true);
        }
    }

    //clears curently created objects within the client picker scrolrect
    private void ClearObjects()
    {
        foreach (GameObject g in clientsInList)
        {
            Destroy(g);
        }
        clientsInList.Clear();
    }

    //compares two strings, returns true if the string begins with querry;
    private bool CompareString(string querry, string comparison)
    {
        querry = querry.ToLower();
        comparison = comparison.ToLower().Trim();
        return comparison.StartsWith(querry);
    }
}
