using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using UINavigation;

public class PropertyButton : MonoBehaviour
{
    public List<GameObject> RoomButtons { get; set; } = new List<GameObject>();
    public Transform RoomsContentScrollView = null;

    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Text nrOfRooms = null;
    [SerializeField]
    private Button propertyButtonItem = null;
    [SerializeField]
    private Button editPropertyButton = null;
    [SerializeField]
    private Button deletePropertyButton = null;
    [SerializeField]
    private Button addRoomButton = null;
    [SerializeField]
    private GameObject roomArrowLeft = null;
    [SerializeField]
    private GameObject roomArrowDown = null;
    [SerializeField]
    private GameObject addRoomsButton = null;
    [SerializeField]
    private Image disponibilityMarker = null;

    public void Initialize(IProperty property, RectTransform layoutContent, bool disponibility, string nrRooms, Action<IProperty> addRoomCallback, Action<IRoom> PropertyRoomCallback, Action<IProperty> editCallback, Action<IProperty> deleteCallback)
    {
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.NEW_PROPERTY : property.Name;
        editPropertyButton.onClick.AddListener(() => editCallback(property));
        deletePropertyButton.onClick.AddListener(() => deleteCallback(property));
        if (!property.HasRooms)
        {
            propertyButtonItem.onClick.AddListener(() => PropertyRoomCallback(property.GetRoom(property.GetPropertyRoomID)));
            IEnumerable<IReservation> reservations = ReservationDataManager.GetActiveRoomReservations(property.GetRoom(property.GetPropertyRoomID).ID)
                    .Where(r => r.Period.Includes(DateTime.Today));
            if (reservations.Count() == 0)
            {
                disponibilityMarker.color = Constants.availableItemColor;
            }
            else
            {
                disponibilityMarker.color = Constants.reservedUnavailableItemColor;
            }
            if (disponibility)
            {
                disponibilityMarker.color = Constants.availableItemColor;
            }
        }
        else
        {
            nrOfRooms.text = string.IsNullOrEmpty(nrRooms) ? Constants.ROOMS_NUMBER : nrRooms;
            if (disponibility)
            {
                addRoomsButton.SetActive(false);
            }
            else
            {
                addRoomsButton.SetActive(true);
            }
            addRoomButton.onClick.AddListener(() => addRoomCallback(property));
            propertyButtonItem.onClick.AddListener(() =>
            {
                RoomsContentScrollView.gameObject.SetActive(RoomsContentScrollView.gameObject.activeInHierarchy ? false : true);
                if (RoomsContentScrollView.gameObject.activeInHierarchy)
                {
                    roomArrowLeft.SetActive(false);
                    roomArrowDown.SetActive(true);
                }
                else
                {
                    roomArrowLeft.SetActive(true);
                    roomArrowDown.SetActive(false);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutContent);
                Canvas.ForceUpdateCanvases();
            });
        }
    }
}
