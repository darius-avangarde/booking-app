using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UINavigation;

public class PropertyButton : MonoBehaviour
{
    [SerializeField]
    private Text propertyName = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private Image overlayLarge = null;
    [SerializeField]
    private Image overlaySmall = null;
    [SerializeField]
    private Button propertyButtonItem = null;
    [SerializeField]
    private Image disponibilityMarker = null;

    private IProperty currentProperty;
    private RectTransform propertyItemTransform;
    private DateTime dateTimeStart = DateTime.Today.Date;
    private DateTime dateTimeEnd = DateTime.Today.AddDays(1).Date;
    private float currentTime;
    private float maxHeight;

    private void Awake()
    {
        propertyItemTransform = GetComponent<RectTransform>();
    }

    public void InitializeDateTime(DateTime dateTimeStart, DateTime dateTimeEnd)
    {
        this.dateTimeStart = dateTimeStart;
        this.dateTimeEnd = dateTimeEnd;
    }

    public void Initialize(IProperty property, Action<IRoom> PropertyRoomCallback, Action<IProperty> PropertyCallback, Action<DateTime, DateTime, List<IRoom>> reservationCallback)
    {
        currentProperty = property;
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.NEW_PROPERTY : property.Name;
        if (ImageDataManager.PropertyPhotos.ContainsKey(property.ID))
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[property.ID];
        }
        else
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[Constants.defaultPropertyPicture];
        }
        if (!property.HasRooms)
        {
            if (reservationCallback != null)
            {
                propertyButtonItem.onClick.AddListener(() => reservationCallback(dateTimeStart, dateTimeEnd, SendCurrentRoom()));
            }
            else
            {
                propertyButtonItem.onClick.AddListener(() => PropertyRoomCallback(property.GetPropertyRoom()));
            }
            disponibilityMarker.gameObject.SetActive(true);
            bool reservations = ReservationDataManager.GetReservationsBetween(dateTimeStart, dateTimeEnd)
                .Any(r => r.ContainsRoom(property.GetRoom(property.GetPropertyRoomID).ID));
            if (reservations)
            {
                disponibilityMarker.color = Constants.reservedUnavailableItemColor;
            }
            else
            {
                disponibilityMarker.color = Constants.availableItemColor;
            }
        }
        else
        {
            propertyButtonItem.onClick.AddListener(() => PropertyCallback(property));
            disponibilityMarker.gameObject.SetActive(false);
        }
    }

    private List<IRoom> SendCurrentRoom()
    {
        List<IRoom> currentRoomList = new List<IRoom>();
        currentRoomList.Add(currentProperty.GetPropertyRoom());
        return currentRoomList;
    }
}
