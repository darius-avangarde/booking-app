using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;

public class NotificationOptionsMenu : MonoBehaviour
{
    [SerializeField]
    private ClientsScreen clientsScreen = null;
    [SerializeField]
    private OptionsMenuDropdown notificationItemMenu = null;
    [SerializeField]
    private List<Sprite> dropdownIcons;

    private enum DropdownOptions { call_icon, msg, email }
    private string[] menuOptions = { "Apel", "Mesaj", "E-Mail" };

    private Dictionary<string, Sprite> optionsPictures = new Dictionary<string, Sprite>();
    private Dictionary<string, DropdownOptions> optionsDictionary = new Dictionary<string, DropdownOptions>();

    private IReservation currentReservation;

    private void Start()
    {
        LocalizedText.Instance.OnLanguageChanged.AddListener(() => SetDropdownOptions());
        SetDropdownOptions();
    }

    private void SetDropdownOptions()
    {
        for (int i = 0; i <= (int)DropdownOptions.email; i++)
        {
            optionsDictionary.Add(LocalizedText.Instance.NotificationDropdown[i], (DropdownOptions)i);
        }
        for (int i = 0; i < dropdownIcons.Count; i++)
        {
            optionsPictures.Add(dropdownIcons[i].name.ToLower(), dropdownIcons[i]);
        }
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < LocalizedText.Instance.NotificationDropdown.Length; i++)
        {
            foreach (var item in optionsPictures)
            {
                if (optionsDictionary[LocalizedText.Instance.NotificationDropdown[i]].ToString() == item.Key)
                {
                    switch ((DropdownOptions)i)
                    {
                        case DropdownOptions.call_icon:
                            notificationItemMenu.AddOption(LocalizedText.Instance.NotificationDropdown[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => CallClient());
                            break;
                        case DropdownOptions.msg:
                            notificationItemMenu.AddOption(LocalizedText.Instance.NotificationDropdown[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => SendSMS());
                            break;
                        case DropdownOptions.email:
                            notificationItemMenu.AddOption(LocalizedText.Instance.NotificationDropdown[i], optionsPictures[((DropdownOptions)i).ToString().ToLower()], () => SendEmail());
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
}
