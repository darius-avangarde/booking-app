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
    private  string option;
    private SettingsManager languageSet;
    private string[] dropOptions = { "Română", "Engleză" };
   
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
            ddImage.sprite = spriteList[0];
            option = "Ro";
            SetLanguage(option);
            languageSet.DataElements.settings.language = option;
            languageSet.WriteData();
            ChangeOptionsDropdown();
        }
        else if (languageDropdown.value == 1)
        {
            ddImage.sprite = spriteList[1];
            option = "En";
            languageSet.DataElements.settings.language = option;
            SetLanguage(option);  
            ChangeOptionsDropdown();  
            languageSet.WriteData();

        }
        OnLanguageChanged?.Invoke();
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
        //myManager = LocalizationManager.Instance;
        textList = new List<Text>();
        Text[] elements = new Text[250];
        var language = myManager.Languages.Where(x => x.Name.Trim() == "Ro").First().Texts;

        Debug.Log("clicked");
        foreach (var item in parents)
        {
            elements = item.GetComponentsInChildren<Text>(true);
            foreach (var items in elements)
            {
                if (language.ContainsKey(items.name))
                {
                    textList.Add(items);
                }
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
        var language = myManager.Languages.Where(x => x.Name.Trim() == lang).First().Texts;
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
       // myManager = LocalizationManager.Instance;
       // var lang = csvData.Where(x => x.Name == language).First();
        var lang = myManager.Languages.Where(x => x.Name.Trim() == language).First();
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
