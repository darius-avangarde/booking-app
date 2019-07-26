using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class ReservationObject : MonoBehaviour
{

    public IReservation ObjReservation => objReservation;

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
    UnityAction<IReservation> currentTapAction;

    // private void OnDestroy()
    // {
    //     reservationButton.onClick.RemoveAllListeners();
    // }

    public void PlaceUpdateObject(ReservationObjectManager.PointSize pointSize, CalendarDayColumnObject c, IReservation reservation, UnityAction<IReservation> tapAction, bool forceReposition = false)
    {
        currentTapAction = tapAction;

        //reservationButton.onClick.RemoveAllListeners();
        //reservationButton.targetGraphic.color = (reservation.Period.End.Date > DateTime.Today.Date) ? Placeholder_ThemeManager.Instance.currentReservationColor : Placeholder_ThemeManager.Instance.pastReservationColor;
        itemImage.color = (reservation.Period.End.Date > DateTime.Today.Date) ? CalendarThemeManager.Instance.currentReservationColor : CalendarThemeManager.Instance.pastReservationColor;

        co = c;

        gameObject.SetActive(true);
        objectRectTransform.position = pointSize.minPos;
        objectRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,pointSize.size.x);
        objectRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,pointSize.size.y);
        objectRectTransform.pivot = pointSize.pivot;

        objReservation = reservation;
        //reservationButton.onClick.AddListener(() => tapAction(objReservation));

        clientNameText.text = reservation.CustomerName;

        if(forceReposition)
        {
            StartCoroutine(ForceReposition());
        }
    }

    private IEnumerator ForceReposition()
    {
        yield return new WaitForEndOfFrame();
        if(co != null)
        {
            if(transform.position != co.DayRectTransform.position)
            {
                transform.position = co.DayRectTransform.position;
            }
            else
            {
                co = null;
            }
        }
    }

    public void ButtonAction()
    {
        currentTapAction?.Invoke(objReservation);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        //reservationButton.onClick.RemoveAllListeners();
    }
}
