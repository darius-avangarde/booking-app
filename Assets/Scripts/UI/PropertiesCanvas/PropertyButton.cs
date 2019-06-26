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
    private Text availableRooms = null;
    [SerializeField]
    private Image propertyImage = null;
    [SerializeField]
    private AspectRatioFitter propertyImageAspectFitter = null;
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

    /// <summary>
    /// set new date period
    /// </summary>
    /// <param name="dateTimeStart">start of date period</param>
    /// <param name="dateTimeEnd">end of date period</param>
    public void InitializeDateTime(DateTime dateTimeStart, DateTime dateTimeEnd)
    {
        this.dateTimeStart = dateTimeStart;
        this.dateTimeEnd = dateTimeEnd;
    }

    /// <summary>
    /// set the current property
    /// load property images
    /// set property name
    /// set property size
    /// set button callback according to different situations
    /// </summary>
    /// <param name="property">selected property</param>
    /// <param name="disponibility">if the item is instantiated on disponibility screen mark as true</param>
    /// <param name="PropertyRoomCallback">click callback for properties without rooms</param>
    /// <param name="PropertyCallback">callback for properties with rooms</param>
    /// <param name="reservationCallback">callback to open reservation screen</param>
    public void Initialize(IProperty property, bool disponibility, Action<IRoom> PropertyRoomCallback, Action<IProperty> PropertyCallback, Action<DateTime, DateTime, List<IRoom>> reservationCallback)
    {
        currentProperty = property;
        if (ImageDataManager.PropertyPhotos.ContainsKey(property.ID))
        {
            propertyImage.sprite = (Sprite)ImageDataManager.PropertyPhotos[property.ID];
        }
        
        //set the aspect ratio of the property image
        propertyImageAspectFitter.aspectRatio = (float)propertyImage.sprite.texture.width/propertyImage.sprite.texture.height;

        if (disponibility)
        {
            propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.NEW_PROPERTY : $"Proprietatea {property.Name}";
            propertyItemTransform.sizeDelta = new Vector2(propertyItemTransform.sizeDelta.x, 360f);
        }
        else
        {
            propertyName.text = string.IsNullOrEmpty(property.Name) ? Constants.NEW_PROPERTY : $"Proprietatea{Environment.NewLine}{property.Name}";
            propertyItemTransform.sizeDelta = new Vector2(propertyItemTransform.sizeDelta.x, 750f);
        }

        if (!property.HasRooms)
        {
            availableRooms.gameObject.SetActive(false);
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
            if (disponibility)
            {
                int roomsNumber = 0;
                foreach (var room in currentProperty.Rooms)
                {
                    bool reservations = ReservationDataManager.GetReservationsBetween(dateTimeStart, dateTimeEnd)
                        .Any(r => r.ContainsRoom(room.ID));
                    if (!reservations)
                    {
                        roomsNumber++;
                    }
                }
                availableRooms.gameObject.SetActive(true);
                availableRooms.text = $"{Constants.AVAILABLE_ROOMS} {roomsNumber}";
                if(roomsNumber == 0)
                {
                    availableRooms.color = Constants.reservedUnavailableItemColor;
                }
                else
                {
                    availableRooms.color = Color.white;
                }
            }
            else
            {
                availableRooms.gameObject.SetActive(false);
            }
            propertyButtonItem.onClick.AddListener(() => PropertyCallback(property));
            disponibilityMarker.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// convert the room of the property to a list of rooms
    /// </summary>
    /// <returns>a list of rooms, only current property room is included in list</returns>
    private List<IRoom> SendCurrentRoom()
    {
        List<IRoom> currentRoomList = new List<IRoom>();
        currentRoomList.Add(currentProperty.GetPropertyRoom());
        return currentRoomList;
    }
}
