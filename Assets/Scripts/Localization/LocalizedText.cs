using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public List<Text> texts;
    public List<Text> textList;
    public LocalizationManager myManager = new LocalizationManager();
    public Dropdown mydd;
    //public List<LanguageScript> csvData;
    public void ChangeLanguage()
    {
       // csvData = LocalizationManager.Instance.ReadFromCSV(@"Assets/Resources/TextsFileEx.csv").ToList();
        if (mydd.value == 0)
        {
            SetLanguage("Ro");
        }
        else if (mydd.value == 1)
        {
            SetLanguage("En");
        }
    }

    private void Start()
    {
        //myManager = LocalizationManager.instance;
       // GetTexts();
    }

    public void GetTexts()
    {
        //  myManager = LocalizationManager.Instance;
        textList = new List<Text>();
       var language = myManager.Languages.Where(x => x.Name == "Ro").First().Texts;
        
        foreach (var item in textList)
        {
            Debug.Log(item.text);
        }
        Debug.Log("clicked");
        texts = FindObjectsOfType<Text>().ToList();
        foreach (var item in texts)
        {
            /*if (item.name == "TextNameq" || item.name == "TextEmailw")
            {
                textList.Add(item);
                Debug.Log(item.name);
            }*/


             if (language.ContainsKey(item.name))
             {
                textList.Add(item);
                Debug.Log(item.name);
             }
        }

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
