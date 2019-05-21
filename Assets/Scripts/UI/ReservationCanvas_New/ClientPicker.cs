using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClientPicker : MonoBehaviour
{

    [SerializeField]
    private ReservationEditScreen editScreen;
    [SerializeField]
    private GameObject clientObjPrefab;

    [SerializeField]
    private InputField clientNameField;
    [SerializeField]
    private GameObject scrolrect;
    [SerializeField]
    private Transform clientObjContainer;
    [SerializeField]
    private float searchDelay = 1.0f;
    [SerializeField]
    private Transform searchLoadImage;


    private List<GameObject> clientsInList = new List<GameObject>();
    private string currentQuerry;
    private WaitForSeconds waitTime;

    private void Start()
    {
        clientNameField.onValueChanged.AddListener(FilterClients);
        waitTime = new WaitForSeconds(searchDelay);
    }

    private void OnDestroy()
    {
        clientNameField.onValueChanged.RemoveAllListeners();
    }

    public void AddNewClient()
    {
        Debug.Log("Going to new client screen");
        //navigator go to new client screen
        //perhaps give back action as well
    }

    private void FilterClients(string value)
    {
        if(editScreen.AllowSearch)
        {
            if(!scrolrect.activeSelf)
            {
                scrolrect.SetActive(true);
            }

            StopAllCoroutines();
            ClearObjects();
            currentQuerry = value;
            StartCoroutine(DelayedDisplay());
        }
        else
        {
            if(scrolrect.activeSelf)
            {
                scrolrect.SetActive(false);
            }
        }
    }

    private IEnumerator DelayedDisplay()
    {
        searchLoadImage.gameObject.SetActive(true);
        for (float t = 0; t < searchDelay; t += Time.deltaTime/searchDelay)
        {
            searchLoadImage.rotation *= Quaternion.AngleAxis(-10, Vector3.forward);
            yield return null;
        }
        searchLoadImage.gameObject.SetActive(false);

        List<IClient> matches = ClientDataManager.GetClients()
            .Where(c => CompareString(currentQuerry, c.Name)).ToList();

        for (int c = 0; c < matches.Count; c++)
        {
            clientsInList.Add(Instantiate(clientObjPrefab, clientObjContainer));
            clientsInList[c].GetComponent<ClientPickerObject>().Initialize(matches[c], SelectAction);
        }
    }

    private void ClearObjects()
    {
        foreach (GameObject g in clientsInList)
        {
            Destroy(g);
        }
        clientsInList.Clear();
    }

    private void SelectAction(IClient client)
    {
        editScreen.SetClient(client);
        scrolrect.SetActive(false);
    }

    private bool CompareString(string querry, string comparison)
    {
        querry = querry.ToLower();
        comparison = comparison.ToLower();
        return comparison.Contains(querry) && comparison.StartsWith(querry);
    }
}
