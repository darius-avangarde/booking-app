using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class ReservationObject : MonoBehaviour
{
    [SerializeField]
    private RectTransform objectRectTransform;
    [SerializeField]
    private RectTransform textRectTransform;
    [SerializeField]
    private TextMeshProUGUI clientNameText;
    [SerializeField]
    private Button reservationButton;


    private IReservation objReservation;


    private void OnDestroy()
    {
        reservationButton.onClick.RemoveAllListeners();
    }

    public void PlaceUpdateObject(Vector2 lowerLeft, Vector2 size, IReservation reservation, UnityAction<IReservation> tapAction)
    {
        gameObject.SetActive(true);
        objectRectTransform.position   = lowerLeft;
        objectRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,size.x);
        objectRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,size.y);

        objReservation = reservation;
        reservationButton.onClick.AddListener(() => tapAction(objReservation));

        clientNameText.text = reservation.CustomerName;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        reservationButton.onClick.RemoveAllListeners();
    }
}
