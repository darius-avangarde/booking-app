using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientPicker : MonoBehaviour
{

    [SerializeField]
    private ReservationEditScreen editScreen;

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

    [Space]
    [SerializeField]
    private float searchDelay = 1.0f;
    [SerializeField]
    private int maxClientsInPicker = 100;


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
        searchLoadImage.gameObject.SetActive(false);
        scrolRect = scrolRectGameObj.GetComponent<ScrollRect>();
    }

    private void OnDestroy()
    {
        clientNameField.onValueChanged.RemoveAllListeners();
    }

    //Add client function attached to the add new client button in the client picker scrollrect
    public void AddNewClient()
    {
        //pass value of input field//callback for return
        Debug.Log("Going to new client screen");
        //navigator go to new client screen
        //perhaps give back action as well

        // callback is:
        //SelectAction(client);
            // if client is not saved (return with client = null)
            // else client = newly created client
        //(do navigator go back on cancel)

        scrolRectGameObj.SetActive(false);
    }

    //Filters the input field value on change if allowed to search, and starts load coroutine before populating scrolrect with matching clients
    private void FilterClients(string value)
    {
        if(editScreen.AllowSearch)
        {
            if(!scrolRectGameObj.activeSelf)
            {
                scrolRectGameObj.SetActive(true);
            }

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

        //addClientButton.transform.SetAsFirstSibling();
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

    //callback assigned to the client picker objects (sets the active client in the edit reservation screen and concludes search)
    private void SelectAction(IClient client)
    {
        editScreen.SetClient(client);
        scrolRectGameObj.SetActive(false);
        addClientButton.SetActive(false);
    }

    //compares two strings, returns true if the string begins with querry;
    private bool CompareString(string querry, string comparison)
    {
        querry = querry.ToLower();
        comparison = comparison.ToLower();
        return comparison.StartsWith(querry);
    }
}
