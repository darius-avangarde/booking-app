using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    //public List<LanguageScript> csvData; 
    [SerializeField]
    private List<Text> texts;
    [SerializeField]
    private List<Text> textList;
    [SerializeField]
    private LocalizationManager myManager = new LocalizationManager();
    [SerializeField]
    private Dropdown languageDropdown;
    [SerializeField]
    private List<Sprite> spriteList;
    public  string option;
    private SettingsManager languageSet;
    private string[] dropOptions = { "Română", "Engleză" };
    UnityEvent m_MyEvent = new UnityEvent();

    private void Start()
    {
        languageSet = new SettingsManager();
        languageSet.ReadData();
        option = languageSet.DataElements.settings.language;
        SetLanguage(option);
        Debug.Log(option + "----limba este");
        ChangeOptionsDropdown();
       // m_MyEvent.AddListener(ChangeLanguage);
       //myManager = LocalizationManager.instance;
    }
  
    public void ChangeLanguage()
    {
       // m_MyEvent.Invoke();
       //csvData = LocalizationManager.Instance.ReadFromCSV(@"Assets/Resources/TextsFileEx.csv").ToList();
        if (languageDropdown.value == 0)
        {
            option = "Ro";
            SetLanguage(option);
            languageSet.DataElements.settings.language = option;
            languageSet.WriteData();
            ChangeOptionsDropdown();

        }
        else if (languageDropdown.value == 1)
        {
            option = "En";
            languageSet.DataElements.settings.language = option;
            SetLanguage(option);  
            ChangeOptionsDropdown();  
            languageSet.WriteData();
          
           
        }
    }
    private void ChangeOptionsDropdown()
    {
        languageDropdown.options[0].text =Languages[0];
        languageDropdown.options[1].text = Languages[1];

    }
    private void PopulateDropdown(string optionL)
    {
        Debug.Log(optionL + "----limba este");
        dropOptions = SetOptionsValues(optionL, "Language");
        languageDropdown.ClearOptions();
        for (int i = 0; i < spriteList.Count; i++)
        {
            languageDropdown.options.Add(new Dropdown.OptionData(dropOptions[i], spriteList[i]));
        }
    }

    public void GetTexts()
    {
        //  myManager = LocalizationManager.Instance;
        textList = new List<Text>();
        var language = myManager.Languages.Where(x => x.Name == "Ro").First().Texts;

        Debug.Log("clicked");
        texts = FindObjectsOfType<Text>().ToList();
        foreach (var item in texts)
        {
            if (language.ContainsKey(item.name))
            {
                textList.Add(item);
            }
        }

    }
    public string[] DaysLong {
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
    public string[] Languages
    {
        get
        {
            return SetOptionsValues(option, "Language");
        }
    }

    public   string[] SetOptionsValues(string lang, string myKey)
    {
        var language = myManager.Languages.Where(x => x.Name == lang).First().Texts;
        var result = new string[12];
        foreach (var item in language)
        {
            if (item.Key == myKey)
            {
                result = item.Value.Split(',');
            }
        }
        return result;
    }

    private void SetLanguage(string language)
    {
        var lang = myManager.Languages.Where(x => x.Name == language).First();
        foreach (var item in textList)
        {
            item.text = lang.Texts[item.name];
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
