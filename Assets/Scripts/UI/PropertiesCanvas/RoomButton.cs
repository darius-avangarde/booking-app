using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public DisponibilityScreen DisponibilityScreenComponent { get; set; }
    public Image DisponibilityMarker;
    public bool Selected { get; set; } = false;

    [SerializeField]
    private Text roomName = null;
    //[SerializeField]
    //private Text roomPrice = null;
    //[SerializeField]
    //private Text roomBeds = null;
    //[SerializeField]
    //private Text personsNumber = null;
    [SerializeField]
    private Button roomButton = null;
    [SerializeField]
    private ScrollButton roomScrollButton = null;
    //[SerializeField]
    //private Image disponibilityMarker = null;
    [SerializeField]
    private Image selectMarker = null;

    Action<DateTime, DateTime, List<IRoom>> reservationCallback;
    Action<IRoom> roomCallback;
    private IRoom currentRoom;
    private DateTime dateTimeStart = DateTime.Today.Date;
    private DateTime dateTimeEnd = DateTime.Today.AddDays(1).Date;
    private bool pressAndHold = false;
    private bool reservations = false;
    private bool firstSelection = false;

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
    /// set current room name
    /// set disponibility marker color
    /// set callbacks according to different situations
    /// </summary>
    /// <param name="room">current room</param>
    /// <param name="disponibilityScript">refference to disponibility screen</param>
    /// <param name="roomCallback">callback to open room screen</param>
    /// <param name="reservationCallback">calback to open reservation screen</param>
    public void Initialize(IRoom room, DisponibilityScreen disponibilityScript, Action<IRoom> roomCallback, Action<DateTime, DateTime, List<IRoom>> reservationCallback)
    {
        selectMarker.gameObject.SetActive(false);
        roomName.text = string.IsNullOrEmpty(room.Name) ? Constants.NEW_ROOM : $"Camera {room.Name}";
        DisponibilityScreenComponent = disponibilityScript;
        //roomPrice.text = string.IsNullOrEmpty(room.Price) ? Constants.PRICE : ("Pret: " + room.Price + " ron");
        //roomBeds.text = null;
        //if (room.SingleBeds != 0)
        //{
        //    roomBeds.text += Constants.SingleBed + room.SingleBeds;
        //}
        //if (room.SingleBeds != 0 && room.DoubleBeds != 0)
        //{
        //    roomBeds.text += " & ";
        //}
        //if (room.DoubleBeds != 0)
        //{
        //    roomBeds.text += Constants.DoubleBed + room.DoubleBeds;
        //}
        reservations = ReservationDataManager.GetReservationsBetween(dateTimeStart, dateTimeEnd)
                .Any(r => r.ContainsRoom(room.ID));
        if (reservations)
        {
            DisponibilityMarker.color = Constants.reservedUnavailableItemColor;
        }
        else
        {
            DisponibilityMarker.color = Constants.availableItemColor;
        }
        currentRoom = room;
        if (disponibilityScript != null)
        {
            roomButton.enabled = false;
            roomScrollButton.enabled = true;
            if (reservationCallback != null)
            {
                this.reservationCallback = reservationCallback;
            }
            else
            {
                this.roomCallback = roomCallback;
            }
            roomScrollButton.OnPointerDownEvent.AddListener(() => StartCoroutine(SelectionMode()));
            roomScrollButton.OnDragEvent.AddListener(() => StartCoroutine(StopCoroutineDelay()));
            roomScrollButton.OnClick.AddListener(() => ClickOrSelect());
            if (disponibilityScript.selectedRooms.Any(r => r.ID == currentRoom.ID))
            {
                SelectToggleMark();
            }
        }
        else
        {
            roomScrollButton.enabled = false;
            roomButton.enabled = true;
            roomButton.onClick.AddListener(() => roomCallback(room));
        }
    }

    /// <summary>
    /// convert the current room to a list of rooms
    /// </summary>
    /// <returns>a list of rooms, only current room is included in list</returns>
    private List<IRoom> SendCurrentRoom()
    {
        List<IRoom> currentRoomList = new List<IRoom>();
        currentRoomList.Add(currentRoom);
        return currentRoomList;
    }

    /// <summary>
    /// if in selection mode, select or deselect current room item
    /// </summary>
    public void SelectToggleMark()
    {
        if (Selected)
        {
            Selected = false;
            selectMarker.gameObject.SetActive(false);
            if (DisponibilityScreenComponent.selectedRooms.Any(r => r.ID == currentRoom.ID))
            {
                DisponibilityScreenComponent.selectedRooms.Remove(currentRoom);
            }
        }
        else
        {
            Selected = true;
            selectMarker.gameObject.SetActive(true);
            if (!DisponibilityScreenComponent.selectedRooms.Any(r => r.ID == currentRoom.ID))
            {
                DisponibilityScreenComponent.selectedRooms.Add(currentRoom);
            }
            DisponibilityScreenComponent.CheckRoomsSelection();
        }
    }

    /// <summary>
    /// check if the button should act as select or invoke a different callback
    /// </summary>
    private void ClickOrSelect()
    {
        StopAllCoroutines();
        if (!pressAndHold)
        {
            if (reservationCallback != null)
            {
                reservationCallback.Invoke(dateTimeStart, dateTimeEnd, SendCurrentRoom());
            }
            else
            {
                roomCallback.Invoke(currentRoom);
            }
        }
        else
        {
            if (!firstSelection)
            {
                SelectToggleMark();
            }
            firstSelection = false;
        }
        DisponibilityScreenComponent.CheckRoomsSelection();
    }

    /// <summary>
    /// check if the user is holding on the current room item
    /// </summary>
    /// <returns>selection mode after a period of time</returns>
    private IEnumerator SelectionMode()
    {
        pressAndHold = false;
        if (!DisponibilityScreenComponent.roomSelection)
        {
            double timer = 0;
            while (timer < 0.6f)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            SelectToggleMark();
            firstSelection = true;
        }
        pressAndHold = true;
    }

    /// <summary>
    /// delay to stop all coroutines, to avoid selecting items by mistake
    /// </summary>
    /// <returns>wait for 0.2 seconds</returns>
    private IEnumerator StopCoroutineDelay()
    {
        yield return new WaitForSeconds(0.2f);
        StopAllCoroutines();
    }
}
