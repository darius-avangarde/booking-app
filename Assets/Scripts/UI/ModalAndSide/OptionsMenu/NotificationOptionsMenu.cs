using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class NotificationOptionsMenu : MonoBehaviour
{
    [SerializeField]
    private ConfirmationDialog confirmationDialog = null;
    [SerializeField]
    private ReservationEditScreen reservationScreenComponent = null;
    [SerializeField]
    private ClientsScreen clientsScreen = null;
    [SerializeField]
    private OptionsMenuDropdown notificationItemMenu = null;
    [SerializeField]
    private List<Sprite> dropdownIcons;

    private enum DropdownOptions { call_icon, msg, email, edit, cancel, delete}
    private string[] menuOptions = { "Apel", "Mesaj", "E-Mail", "Editează", "Anulează", "Șterge" };

    private Dictionary<string, Sprite> optionsPictures = new Dictionary<string, Sprite>();
    private Dictionary<string, DropdownOptions> optionsDictionary = new Dictionary<string, DropdownOptions>();

    private IReservation currentReservation;

    private void Start()
    {
        for (int i = 0; i <= (int)DropdownOptions.delete; i++)
        {
            optionsDictionary.Add(menuOptions[i], (DropdownOptions)i);
        }
        foreach (var image in dropdownIcons)
        {
            optionsPictures.Add(image.name.ToLower(), image);
        }
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            foreach (var item in optionsPictures)
            {
                if (optionsDictionary[menuOptions[i]].ToString() == item.Key)
                {
                    switch ((DropdownOptions)i)
                    {
                        case DropdownOptions.call_icon:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => CallClient());
                            break;
                        case DropdownOptions.msg:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => SendSMS());
                            break;
                        case DropdownOptions.email:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => SendEmail());
                            break;
                        case DropdownOptions.edit:
                            notificationItemMenu.AddOption("", null, null, 20);
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => EditReservation());
                            break;
                        case DropdownOptions.cancel:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => CancelReservation());
                            break;
                        case DropdownOptions.delete:
                            notificationItemMenu.AddOption(menuOptions[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => DeleteProperty());
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void OpenMenu(IReservation reservation)
    {
        currentReservation = reservation;
        notificationItemMenu.ShowMenu();
    }

    private void CallClient()
    {
        clientsScreen.phoneUS(ClientDataManager.GetClient(currentReservation.CustomerID));
    }

    private void SendSMS()
    {
        clientsScreen.SmsUs(ClientDataManager.GetClient(currentReservation.CustomerID));
    }

    private void SendEmail()
    {
        clientsScreen.EmailUs(ClientDataManager.GetClient(currentReservation.CustomerID));
    }

    private void EditReservation()
    {
        reservationScreenComponent.OpenEditReservation(currentReservation, null);
    }

    private void CancelReservation()
    {
        currentReservation.CancelReservation();
    }

    private void DeleteProperty()
    {
        confirmationDialog.Show(new ConfirmationDialogOptions
        {
            Message = Constants.DELETE_ROOM_RESERVATIONS,
            ConfirmText = Constants.DELETE_CONFIRM,
            CancelText = Constants.DELETE_CANCEL,
            ConfirmCallback = () =>
            {
                ReservationDataManager.DeleteReservation(currentReservation.ID);
            },
            CancelCallback = null
        });
    }
}
