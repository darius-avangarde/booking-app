using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ClientPicker : MonoBehaviour
{

    [SerializeField]
    private ReservationEditScreen reservationEdit;
    [SerializeField]
    private ClientsScreen clientEdit;

    [Space]
    [SerializeField]
    private InputField clientNameField;
    [SerializeField]
    private Transform searchLoadImage;
    [SerializeField]
    private GameObject clientObjPrefab;
    [SerializeField]
    private GameObject scrolRectGameObj;
    [SerializeField]
    private Transform clientObjContainer;
    [SerializeField]
    private GameObject addClientButton;
    [SerializeField]
    private GameObject noClientsFoundText;

    [Space]
    [SerializeField]
    private float searchDelay = 1.0f;
    [SerializeField]
    private int maxClientsInPicker = 100;

    [Space]
    [SerializeField]
    private UnityEvent onClickAddNewClient;


    private List<GameObject> clientsInList = new List<GameObject>();
    private string currentQuerry;
    private WaitForSeconds waitTime;
    private ScrollRect scrolRect;

    private void Start()
    {
        clientNameField.onValueChanged.AddListener(FilterClients);
        waitTime = new WaitForSeconds(searchDelay);
        addClientButton.SetActive(false);
        scrolRectGameObj.SetActive(false);
        noClientsFoundText.SetActive(false);
        searchLoadImage.gameObject.SetActive(false);
        scrolRect = scrolRectGameObj.GetComponentInChildren<ScrollRect>();
    }

    private void OnDestroy()
    {
        clientNameField.onValueChanged.RemoveAllListeners();
    }

    //Add client function attached to the add new client button in the client picker scrollrect
    public void AddNewClient()
    {
        ///add client screen - save client + go to navscreen on button + setClientCallback

        //clientEdit.SaveAddedClient(string clientNameField.text, UnityAction<IClient> callback);


        Debug.Log("Should be to new client screen");

        if(onClickAddNewClient != null)
        {
            onClickAddNewClient.Invoke();
        }

        scrolRectGameObj.SetActive(false);
    }

    //triggered when the user taps outside the scrolview rect containing the client names
    public void CancelSearch()
    {
        SelectAction(null);
    }

    //callback assigned to the client picker objects (sets the active client in the edit reservation screen and concludes search)
    internal void SelectAction(IClient client)
    {
        reservationEdit.SetClient(client);
        scrolRectGameObj.SetActive(false);
        addClientButton.SetActive(false);
    }

    //Filters the input field value on change if allowed to search, and starts load coroutine before populating scrolrect with matching clients
    private void FilterClients(string value)
    {
        if(reservationEdit.allowEdit)
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
        comparison = comparison.ToLower();
        return comparison.StartsWith(querry);
    }
}
