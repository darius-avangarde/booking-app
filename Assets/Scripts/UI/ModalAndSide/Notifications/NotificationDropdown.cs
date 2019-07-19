using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDropdown : MonoBehaviour
{
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private NotificationItem notificationItem = null;
    [SerializeField]
    private NotificationsItemMenu notificationItemMenu = null;
    [SerializeField]
    private List<Sprite> dropdownIcons;


    private enum DropdownOptions { call_icon, msg, email, edit, cancel, delete, LAST }
    private string[] menuOptions = { "Apel", "Mesaj", "E-Mail", "Editează", "Anulează", "Șterge" };

    private Dictionary<string, Sprite> optionsPictures = new Dictionary<string, Sprite>();
    private Dictionary<string, DropdownOptions> optionsDictionary = new Dictionary<string, DropdownOptions>();
    private ReservationEditScreen reservationScreenComponent;

    private IReservation currentReservation;

    private void Awake()
    {
        for (int i = 0; i < (int)DropdownOptions.LAST; i++)
        {
            optionsDictionary.Add(menuOptions[i], (DropdownOptions)i);
        }
        foreach (var image in dropdownIcons)
        {
            optionsPictures.Add(image.name.ToLower(), image);
        }
    }

    public void SetReservationScreen(ReservationEditScreen reservationScreen)
    {
        reservationScreenComponent = reservationScreen;
    }

    public void Initialize(IReservation reservation)
    {
        currentReservation = reservation;
        for (int i = 0; i < menuOptions.Length; i++)
        {
            foreach (var item in optionsPictures)
            {
                if (optionsDictionary[menuOptions[i]].ToString() == item.Key)
                {
                    //notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => MenuOptions((DropdownOptions)i));
                    switch ((DropdownOptions)i)
                    {
                        case DropdownOptions.call_icon:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => Debug.Log("am apasat pe apel"));
                            break;
                        case DropdownOptions.msg:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => Debug.Log("am apasat pe mesaj"));
                            break;
                        case DropdownOptions.email:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => Debug.Log("am apasat pe e-mail"));
                            break;
                        case DropdownOptions.edit:
                            notificationItemMenu.AddOption("", null, null, 10);
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => Debug.Log("am apasat pe edit"));
                            //reservationScreenComponent.OpenEditReservation(currentReservation, Initialize);
                            break;
                        case DropdownOptions.cancel:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => Debug.Log("am apasat pe cancel"));
                            break;
                        case DropdownOptions.delete:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => DeleteProperty());
                            break;
                        default:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], null);
                            break;
                    }
                }
            }
        }
    }

    private void DeleteProperty()
    {
        Debug.Log("am apasat pe delete");
        //confirmationDialog.Show(new ConfirmationDialogOptions
        //{
        //    Message = Constants.DELETE_ROOM_RESERVATIONS,
        //    ConfirmText = Constants.DELETE_CONFIRM,
        //    CancelText = Constants.DELETE_CANCEL,
        //    ConfirmCallback = () =>
        //    {
        //        ReservationDataManager.DeleteReservation(currentReservation.ID);
        //    },
        //    CancelCallback = null
        //});
    }
}
