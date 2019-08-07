using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class ReservationObject : MonoBehaviour
{

    public IReservation ObjReservation => objReservation;
    public string ObjRoomID => objRoomID;
    public RectTransform ObjRectTransform => objectRectTransform;

    [SerializeField]
    private RectTransform objectRectTransform;
    [SerializeField]
    private RectTransform textRectTransform;
    [SerializeField]
    private TextMeshProUGUI clientNameText;
    // [SerializeField]
    // private Button reservationButton;
    [SerializeField]
    private Image itemImage;

    private CalendarDayColumnObject co;

    private IReservation objReservation;
    private string objRoomID;

    private UnityAction<IReservation> currentTapAction;
    private bool doGuiPlace = false;


    public void PlaceUpdateObject(ReservationObjectManager.PointSize pointSize, CalendarDayColumnObject c, IReservation reservation, UnityAction<IReservation> tapAction, bool forceReposition = false)
    {
        currentTapAction = tapAction;

        itemImage.color = (reservation.Period.End.Date > DateTime.Today.Date) ? ThemeManager.Instance.ThemeColor.CurrentReservationColor : ThemeManager.Instance.ThemeColor.PastReservationColor;

        co = c;

        gameObject.SetActive(true);
        objectRectTransform.localPosition = pointSize.minPos;
        objectRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,pointSize.size.x);
        objectRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,pointSize.size.y);
        objectRectTransform.pivot = pointSize.pivot;

        objReservation = reservation;
        objRoomID = c.ObjectRoom.ID;

        clientNameText.text = reservation.CustomerName;

        if(forceReposition)
        {
            doGuiPlace = true;
        }
    }

    public void ButtonAction()
    {
        currentTapAction?.Invoke(objReservation);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        if(doGuiPlace)
        {
            DoGuiPlace();
        }
    }

    private void DoGuiPlace()
    {
        if(co != null)
        {
            doGuiPlace = true;
            if(transform.position != co.DayRectTransform.position)
            {
                transform.position = co.DayRectTransform.position;
            }
            else
            {
                co = null;
            }
        }
        doGuiPlace = false;
    }
}
