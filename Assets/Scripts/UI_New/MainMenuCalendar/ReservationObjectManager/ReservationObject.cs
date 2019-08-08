using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class ReservationObject : MonoBehaviour
{

    public IReservation ObjReservation => objReservation;
    public RectTransform ObjRectTransform => objectRectTransform;
    public string ObjRoomID => objRoomID;

    [SerializeField]
    private RectTransform objectRectTransform;
    [SerializeField]
    private RectTransform textRectTransform;
    [SerializeField]
    private TextMeshProUGUI clientNameText;
    [SerializeField]
    private Image itemImage;

    private UnityAction<IReservation> currentTapAction;
    private IReservation objReservation;
    private string objRoomID;


    public void PlaceUpdateObject(ReservationObjectManager.PointSize pointSize, CalendarDayColumnObject c, IReservation reservation, UnityAction<IReservation> tapAction)
    {
        currentTapAction = tapAction;

        itemImage.color = (reservation.Period.End.Date > DateTime.Today.Date) ? ThemeManager.Instance.ThemeColor.CurrentReservationColor : ThemeManager.Instance.ThemeColor.PastReservationColor;

        gameObject.SetActive(true);
        objectRectTransform.localPosition = pointSize.minPos;
        objectRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,pointSize.size.x);
        objectRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,pointSize.size.y);
        objectRectTransform.pivot = pointSize.pivot;

        objReservation = reservation;
        objRoomID = c.ObjectRoom.ID;

        clientNameText.text = reservation.CustomerName;
    }

    public void ButtonAction()
    {
        currentTapAction?.Invoke(objReservation);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
