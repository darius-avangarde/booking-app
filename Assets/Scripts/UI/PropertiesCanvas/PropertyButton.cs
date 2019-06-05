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
    private Image PropertyPicture = null;
    [SerializeField]
    private Button propertyButtonItem = null;
    [SerializeField]
    private Image disponibilityMarker = null;

    private DateTime dateTimeStart = DateTime.Today.Date;
    private DateTime dateTimeEnd = DateTime.Today.AddDays(1).Date;
    private float currentTime;
    private float maxHeight;

    public void InitializeDateTime(DateTime dateTimeStart, DateTime dateTimeEnd)
    {
        this.dateTimeStart = dateTimeStart;
        this.dateTimeEnd = dateTimeEnd;
    }

    public void Initialize(IProperty property, RectTransform layoutContent, Action<IRoom> PropertyRoomCallback, Action<IProperty> PropertyCallback)
    {
        propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.NEW_PROPERTY : property.Name;
        //PropertyPicture.sprite
        if (!property.HasRooms)
        {
            propertyButtonItem.onClick.AddListener(() => PropertyRoomCallback(property.GetRoom(property.GetPropertyRoomID)));
            disponibilityMarker.gameObject.SetActive(true);
            bool reservations = ReservationDataManager.GetReservationsBetween(dateTimeStart, dateTimeEnd)
                .Any(r => r.RoomID == property.GetRoom(property.GetPropertyRoomID).ID);
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
}
