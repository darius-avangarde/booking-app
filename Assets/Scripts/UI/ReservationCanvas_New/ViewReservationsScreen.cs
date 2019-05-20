using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class ViewReservationsScreen : MonoBehaviour
{
    [SerializeField]
    private ConfirmationDialog confirmationDialog;

    [SerializeField]
    private Navigator navigator;
    [SerializeField]
    private ReservationEditScreen editScreen;
    [SerializeField]
    private RectTransform reservationViewPanel;

    [SerializeField]
    private Text reservationsTitle;
    [SerializeField]
    private Transform reservationObjectParent;
    [SerializeField]
    private GameObject reservationObjectPrefab;
    [SerializeField]
    private Transform newReservationButton;


    ///Client details/ text (disable parent for room res screen) --- Name/Number/Adress/Email on new line each
    [SerializeField]
    private Text clientDetails;

    private GameObject clientDetailsGameobject;
    private NavScreen editNavScren;
    private ConfirmationDialogOptions deleteConfirmation;

    private IClient viewedClient;
    private IRoom viewedRoom;
    private List<GameObject> createdObjects = new List<GameObject>();


    private void Start()
    {
        deleteConfirmation = new ConfirmationDialogOptions();
        deleteConfirmation.Message = ReservationConstants.DELETE_DIALOG;

        editNavScren = editScreen.GetComponent<NavScreen>();
        clientDetailsGameobject = clientDetails.transform.parent.gameObject;

        #region Testing

            List<IClient> clientlist = ClientDataManager.GetClients().ToList();
            viewedClient = clientlist[0];
            ViewClientReservations(clientlist[0]);

            // List<IReservation> resList = ReservationDataManager.GetReservations().ToList();
            // ViewRoomReservations(PropertyDataManager.GetProperty(resList[0].PropertyID).GetRoom(resList[0].RoomID));

        #endregion
     }

    public void ViewClientReservations(IClient client)
    {
        viewedClient = client;
        reservationsTitle.text = ReservationConstants.CLIENTS_TITLE;
        UpdateClientDetailsText(client);
        clientDetailsGameobject.SetActive(true);
        CreateReservationObjects(
            ReservationDataManager.GetReservations()
            .Where(r => r.CustomerID.Equals(client.ID))
            .OrderBy(r => r.Period.Start)
            .ToList(), true);

        //navigator.GoTo(editNavScren);
    }

    public void ViewRoomReservations(IRoom room)
    {
        viewedRoom = room;
        reservationsTitle.text = room.Name;
        clientDetailsGameobject.SetActive(false);
        CreateReservationObjects(
            ReservationDataManager.GetReservations()
            .Where(r => r.RoomID.Equals(room.ID))
            .OrderBy(r => r.Period.Start)
            .ToList(), false);

        //navigator.GoTo(editNavScren);
    }



    private void CreateReservationObjects(List<IReservation> reservationList, bool forClient)
    {
        foreach(GameObject go in createdObjects)
        {
            Destroy(go);
        }


        for (int r = 0; r < reservationList.Count; r++)
        {
            ReservationObject resObj = Instantiate(reservationObjectPrefab, reservationObjectParent).GetComponent<ReservationObject>();
            createdObjects.Add(resObj.gameObject);
            if(forClient)
            {
                resObj.InitializeReservation(viewedClient, reservationList[r], EditReservation , DeleteReservation);
            }
            else
            {
                resObj.InitializeReservation(viewedRoom, reservationList[r], EditReservation , DeleteReservation);
            }
        }

        newReservationButton.SetAsFirstSibling();
        LayoutRebuilder.ForceRebuildLayoutImmediate(reservationViewPanel);
    }

    private void EditReservation(IReservation reservation)
    {
        Debug.Log("Edit reservation " + reservation.ID);
        editScreen.OpenEditReservation(reservation);
        navigator.GoTo(editNavScren);
        //throw new NotImplementedException();
    }

    private void DeleteReservation(IReservation reservation)
    {
        //open dialog prompt
            //delete reservation
            //repopulate list
        Debug.Log("Delete reservation " + reservation.ID);
        deleteConfirmation.ConfirmCallback = () => ReservationDataManager.DeleteReservation(reservation.ID);
        confirmationDialog.Show(deleteConfirmation);
        throw new NotImplementedException();
    }

    private void UpdateClientDetailsText(IClient client)
    {
        clientDetails.text = string.Empty;
        clientDetails.text += client.Name;
        if(!string.IsNullOrEmpty(client.Number))
        {
            clientDetails.text += ReservationConstants.NEWLINE + client.Number;
        }

        if(!string.IsNullOrEmpty(client.Adress))
        {
            clientDetails.text += ReservationConstants.NEWLINE + client.Adress;
        }

        if(!string.IsNullOrEmpty(client.Email))
        {
            clientDetails.text += ReservationConstants.NEWLINE + client.Email;
        }
    }

}
