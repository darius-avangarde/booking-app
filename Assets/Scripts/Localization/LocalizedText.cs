using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private SettingsManager languageSet;
    private string[] dropOptions = { "Română", "Engleză" };
    public List<Text> texts;
    public List<Text> textList;
    public LocalizationManager myManager = new LocalizationManager();
    public Dropdown mydd;
    [SerializeField]
    private List<Sprite> spriteList;
    private string romanian;
    private string english;
    private string currentLanguage;
    //public List<LanguageScript> csvData;
    public string option;
    UnityEvent m_MyEvent = new UnityEvent();
    private void Start()
    {
        languageSet = new SettingsManager();
        var x = languageSet.ReadData().settings.language;
        romanian = myManager.Languages.First().Name;
        english = myManager.Languages.Last().Name;
        Debug.Log(romanian + "first-----" + "Last:---" + english);
        currentLanguage = (mydd.value == 0) ? romanian : english;
       // Debug.Log(currentLanguage);
        PopulateDropdown();
        m_MyEvent.AddListener(ChangeLanguage);
        //myManager = LocalizationManager.instance;
    }
    public void SetCurrentLanguage()
    {
        romanian = myManager.Languages.First().Name;
        english = myManager.Languages.Last().Name;
        currentLanguage = (mydd.value == 0) ? romanian : english;
        Debug.Log(currentLanguage+ "-----------");
    }
    public void ChangeLanguage()
    {
        m_MyEvent.Invoke();
        // csvData = LocalizationManager.Instance.ReadFromCSV(@"Assets/Resources/TextsFileEx.csv").ToList();
        if (mydd.value == 0)
        {
            SetLanguage(currentLanguage);
            option = "Ro";
            PopulateDropdown();
        }
        else if (mydd.value == 1)
        {
            SetLanguage(currentLanguage);
            option = "En";
            PopulateDropdown();
        }
    }

    private void PopulateDropdown()
    {
        dropOptions = SetLanguageValues(currentLanguage, "Language");
        mydd.ClearOptions();
        for (int i = 0; i < spriteList.Count; i++)
        {
            mydd.options.Add(new Dropdown.OptionData(dropOptions[i], spriteList[i]));
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
                Debug.Log(item.name);
            }
        }

    }
    public string[] DaysLong {
        get
        { 
            return SetLanguageValues(currentLanguage, "Days");
        }
    }
    public string[] DaysShort
    {
        get
        {
            return SetLanguageValues(currentLanguage, "DaysShort");
        }
    }
    public string[] Months
    {
        get
        {
            return SetLanguageValues(currentLanguage, "Months");
        }
    }
    public string[] MonthsShort
    {
        get
        {
            return SetLanguageValues(currentLanguage, "MonthsShort");
        }
    }
    public string[] DropdownProperty
    {
        get
        {
            return SetLanguageValues(currentLanguage, "Rooms");
        }
    }
    public   string[] SetLanguageValues(string lang, string myKey)
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
        //  myManager = LocalizationManager.Instance;
        //var lang = csvData.Where(x => x.Name == language).First();

        var lang = myManager.Languages.Where(x => x.Name == language).First();
        foreach (var item in textList)
        {
            item.text = lang.Texts[item.name];
            Debug.Log(item.text);
        }
    }
}
