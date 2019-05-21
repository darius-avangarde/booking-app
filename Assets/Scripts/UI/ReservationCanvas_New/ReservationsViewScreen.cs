using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UINavigation;

public class ReservationsViewScreen : MonoBehaviour
{
    #region Inspector refrences
        [Header("Navigation")]
        [SerializeField]
        private Navigator navigator;
        [SerializeField]
        private NavScreen navScreen;

        [Space]
        [SerializeField]
        private ConfirmationDialog confirmationDialog;
        [SerializeField]
        private ReservationEditScreen editScreen;

        [Space]
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
        [SerializeField]
        private Text clientDetails;
    #endregion

    #region Private variables
        private GameObject clientDetailsGameobject;
        private NavScreen editNavScren;
        private ConfirmationDialogOptions deleteConfirmation;
        private IClient viewedClient;
        private IRoom viewedRoom;
        private List<GameObject> createdObjects = new List<GameObject>();
        private bool openedFromClient = false;
    #endregion


    private void Start()
    {
        deleteConfirmation = new ConfirmationDialogOptions();
        deleteConfirmation.Message = ReservationConstants.DELETE_DIALOG;

        editNavScren = editScreen.GetComponent<NavScreen>();
        clientDetailsGameobject = clientDetails.transform.parent.gameObject;
     }

    #region Open pannel functions
        ///<summary>
        /// Opens the view reservetions pannel for the specified Client
        /// Use to open the reservations from a client object in the clients screen
        /// This also updates and enables the client detail text
        ///</summary>
        public void ViewClientReservations(IClient client)
        {
            openedFromClient = true;
            viewedClient = client;
            reservationsTitle.text = ReservationConstants.CLIENTS_TITLE;
            UpdateClientDetailsText(client);
            clientDetailsGameobject.SetActive(true);
            CreateReservationObjects(
                ReservationDataManager.GetReservations()
                .Where(r => r.CustomerID.Equals(client.ID))
                .OrderBy(r => r.Period.Start)
                .ToList(), true);

            navigator.GoTo(navScreen);
        }

        ///<summary>
        /// Opens the view reservetions pannel for the specified Room
        /// Use to open the reservations made for a specific room in the properties screen from either a roomless property or a room
        ///</summary>
        public void ViewRoomReservations(IRoom room)
        {
            openedFromClient = false;
            viewedRoom = room;
            reservationsTitle.text = room.Name;
            clientDetailsGameobject.SetActive(false);
            CreateReservationObjects(
                ReservationDataManager.GetReservations()
                .Where(r => r.RoomID.Equals(room.ID))
                .OrderBy(r => r.Period.Start)
                .ToList(), false);

            navigator.GoTo(navScreen);
        }
    #endregion

    ///<summary>
    /// Opens the reservetion edit pannel and fills the apropriate fields depending on where it is opened from (client or room object).
    ///</summary>
    public void CreateNewReservation()
    {
        if(openedFromClient)
        {
            editScreen.OpenEditReservation(viewedClient);
        }
        else
        {
            editScreen.OpenEditReservation(viewedRoom);
        }
    }

    ///<summary>
    /// Reloads reservations objects after a new reservation is created or an existing one is edited.
    ///</summary>
    public void ReloadReservations()
    {
        if(openedFromClient)
        {
            CreateReservationObjects(
                ReservationDataManager.GetReservations()
                .Where(r => r.CustomerID.Equals(viewedClient.ID))
                .OrderBy(r => r.Period.Start)
                .ToList(), true);
        }
        else
        {
            CreateReservationObjects(
                ReservationDataManager.GetReservations()
                .Where(r => r.RoomID.Equals(viewedRoom.ID))
                .OrderBy(r => r.Period.Start)
                .ToList(), false);
        }
    }

    //Populates reservation objects in the client/room reservation object scrollrect
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

    //Callback for a reservation object's edit button
    private void EditReservation(IReservation reservation)
    {
        editScreen.OpenEditReservation(reservation);
    }

    //Callback for a reservation object's edit button
    private void DeleteReservation(IReservation reservation)
    {
        deleteConfirmation.ConfirmCallback = () => ReservationDataManager.DeleteReservation(reservation.ID);
        confirmationDialog.Show(deleteConfirmation);
    }

    //Updates the client detail text
    private void UpdateClientDetailsText(IClient client)
    {
        clientDetails.text = string.Empty;
        clientDetails.text += client.Name;


        clientDetails.text += ReservationConstants.NEWLINE;
        if(!string.IsNullOrEmpty(client.Number))
        {
            clientDetails.text += client.Number;
        }

        clientDetails.text += ReservationConstants.NEWLINE;
        if(!string.IsNullOrEmpty(client.Adress))
        {
            clientDetails.text += client.Adress;
        }

        clientDetails.text += ReservationConstants.NEWLINE;
        if(!string.IsNullOrEmpty(client.Email))
        {
            clientDetails.text += client.Email;
        }
    }
}
