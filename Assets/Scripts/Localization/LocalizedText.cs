using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public UnityEvent OnLanguageChanged;
    //public List<LanguageScript> csvData; 
    [SerializeField]
    private GameObject[] parents;
    [SerializeField]
    private List<Text> texts;
    [SerializeField]
    private List<Text> textList;
    [SerializeField]
    private Image ddImage;
    [SerializeField]
    private LocalizationManager myManager;
    [SerializeField]
    private Dropdown languageDropdown;
    [SerializeField]
    private List<Sprite> spriteList;
    private static LocalizedText instance;
    private SettingsManager languageSet;
    private string option;

    public static LocalizedText Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LocalizedText>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        myManager = new LocalizationManager();
        myManager.CustomStart();
        languageSet = new SettingsManager();
        languageSet.ReadData();
        option = languageSet.DataElements.settings.language;
        DropdownValue();
        SetLanguage(option);
        ChangeOptionsDropdown();
    }
   
    private void DropdownValue()
    {
        if (option == "Ro")
        {
            languageDropdown.value = 0;
        }
        else
        {
            languageDropdown.value = 1;
        }
    }
    public void ChangeLanguage()
    {
        if (languageDropdown.value == 0)
        {
            SetFunctionsOnChange(0, "Ro");
        }
        else if (languageDropdown.value == 1)
        {
            SetFunctionsOnChange(1, "En");
        }
        OnLanguageChanged?.Invoke();
    }

    private void SetFunctionsOnChange(int value, string myOption)
    {
        ddImage.sprite = spriteList[value];
        option = myOption;
        languageSet.DataElements.settings.language = option;
        SetLanguage(option);
        ChangeOptionsDropdown();
        languageSet.WriteData();
    }
    private void ChangeOptionsDropdown()
    {
        languageDropdown.options[0].text = Languages[0];
        languageDropdown.options[1].text = Languages[1];

    }


    public string[] ReservationHeader
    {
        get
        {
            return SetOptionsValues(option, "ReservationEditHeaderText");
        }
    }
    public string[] DaysLong
    {
        get
        {
            return SetOptionsValues(option, "Days");
        }
    }
    public string[] DaysShort
    {
        get
        {
            return SetOptionsValues(option, "Days_Short");
        }
    }
    public string[] Months
    {
        get
        {
            return SetOptionsValues(option, "Months");
        }
    }
    public string[] MonthsShort
    {
        get
        {
            return SetOptionsValues(option, "Months_Short");
        }
    }
    public string[] DropdownProperty
    {
        get
        {
            return SetOptionsValues(option, "property_dropdown");
        }
    }
    public string[] Notifications
    {
        get
        {
            return SetOptionsValues(option, "Notifications");
        }
    }
    
         public string[] NotificationDropdown
    {
        get
        {
            return SetOptionsValues(option, "NotificationDropdown");
        }
    }
    public Dictionary<int, string> PreAlertDictFunction
    {
        get
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            for (int i = 0; i < Notifications.Length; i++)
            {
                result.Add(i, Notifications[i]);
            }
            return result;
        }
    }

    public string[] Prepositions
    {
        get
        {
            return SetOptionsValues(option, "Prepositions");
        }
    }
    public string[] ConfirmAction
    {
        get
        {
            return SetOptionsValues(option, "ConfirmAction");
        }
    }

    public string[] ConfirmDelete
    {
        get
        {
            return SetOptionsValues(option, "ConfirmDelete");
        }
    }
    public string[] Languages
    {
        get
        {
            return SetOptionsValues(option, "Language");
        }
    }
    public string[] HeaderClients
    {
        get
        {
            return SetOptionsValues(option, "TextAddClient");
        }
    }
    public string MailRequired
    {
        get
        {
            return SetTextValue(option, "MessageEmail");
        }
    }
    public string PhoneRequired
    {
        get
        {
            return SetTextValue(option, "MessagePhone");
        }
    }
    public string NameRequired
    {
        get
        {
            return SetTextValue(option, "NameRequired");
        }
    }
    public string[] HelpTextMainPage
    {
        get
        {
            string textMain = SetTextValue(option, "main_text_help");
            string textTitle = SetTextValue(option, "TextMainPage");
            textMain = textMain.Replace('~', '\n');
            string[] content = new string[2];
            content[0] = textTitle;
            content[1] = textMain;
            return content;
        }
    }

    public string[] HelpTextPropertyPage
    {
        get
        {
            string textMain = SetTextValue(option, "propr_text_help");
            string textTitle = SetTextValue(option, "TextProperties");
            textMain = textMain.Replace('~', '\n');
            string[] content = new string[2];
            content[0] = textTitle;
            content[1] = textMain;
            return content;
        }
    }

    public string[] HelpTextRoomPage
    {
        get
        {
            string textMain = SetTextValue(option, "room_text_help");
            string textTitle = SetTextValue(option, "TextRooms");
            textMain = textMain.Replace('~', '\n');
            string[] content = new string[2];
            content[0] = textTitle;
            content[1] = textMain;
            return content;
        }
    }
    public string[] HelpTextClientPage
    {
        get
        {
            string textClient = SetTextValue(option, "client_text_help");
            string textTitle = SetTextValue(option, "TextClients");
            textClient = textClient.Replace('~', '\n');
            string[] content = new string[2];
            content[0] = textTitle;
            content[1] = textClient;
            return content;
        }
    }
    public string[] HelpTextFilterPage
    {
        get
        {
            string textMain = SetTextValue(option, "filter_text_help");
            string textTitle = SetTextValue(option, "TextFilters");
            textMain = textMain.Replace('~', '\n');
            string[] content = new string[2];
            content[0] = textTitle;
            content[1] = textMain;
            return content;
        }
    }
    public string[] HelpTextReservationPage
    {
        get
        {
            string textMain = SetTextValue(option, "rez_text_help");
            string textTitle = SetTextValue(option, "TextReservations");
            textMain = textMain.Replace('~', '\n');
            string[] content = new string[2];
            content[0] = textTitle;
            content[1] = textMain;
            return content;
        }
    }
    public string[] SetOptionsValues(string lang, string myKey)
    {
        Dictionary<string, string> language = myManager.Languages.Where(x => x.Name.Trim() == lang).First().Texts;
        string[] result = new string[20];
        foreach (var item in language)
        {
            if (item.Key == myKey)
            {
                result = item.Value.Split(',');
            }
        }
        return result;
    }

    public string SetTextValue(string lang, string myKey)
    {
        Dictionary<string, string> language = myManager.Languages.Where(x => x.Name.Trim() == lang).First().Texts;
        string result = string.Empty;
        foreach (var item in language)
        {
            if (item.Key == myKey)
            {
                result = item.Value;
            }
        }
        return result;
    }

    public void GetTexts()
    {
        textList = new List<Text>();
        Text[] elements = new Text[250];
        Dictionary<string, string> language = myManager.Languages.Where(x => x.Name.Trim() == "Ro").First().Texts;
        Debug.Log("clicked");
        for (int i = 0; i < parents.Length; i++)
        {
            elements = parents[i].GetComponentsInChildren<Text>(true);
         
            for (int j = 0; j < elements.Length; j++)
            {
                if (language.ContainsKey(elements[j].name))
                {
                    textList.Add(elements[j]);
                }
            }
        }
    }

    private void SetLanguage(string language)
    {  // myManager = LocalizationManager.Instance;
        // var lang = csvData.Where(x => x.Name == language).First();
        var lang = myManager.Languages.Where(x => x.Name.Trim() == language).First();
        for (int i = 0; i < textList.Count; i++)
        {
            textList[i].text = lang.Texts[textList[i].name];
        }
    }

    private void CheckSystemLanguage()
    {
        if (Application.systemLanguage == SystemLanguage.English)
        {
            SetLanguage(myManager.Languages.Last().Name);
        }
        else if (Application.systemLanguage == SystemLanguage.Romanian)
        {
            SetLanguage(myManager.Languages.First().Name);
        }
    }
}