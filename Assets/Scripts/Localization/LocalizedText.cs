using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public List<Text> texts;
    public List<Text> textList;
    public LocalizationManager myManager;
    public Dropdown mydd;

    public void ChangeLanguage()
    {
        if (mydd.value == 0)
        {
            Debug.Log("ro");
            SetLanguage("Ro");
        }
        else if(mydd.value ==1)
        {
            Debug.Log("en");
            SetLanguage("En");
        }
    }

   
    public void GetTexts()
    {
        foreach(var item in textList)
            {

            Debug.Log(item.text);
        }
        Debug.Log("clicked");
        texts = FindObjectsOfType<Text>().ToList();
        foreach (var item in texts)
        {
            if (item.name == "TextNameq" || item.name == "TextEmailw")
            {
                textList.Add(item);
                Debug.Log(item.name);
            }

            /* var lang = myManager.Languages.Where(x => x.Name == "Ro").First().Texts;
             if (lang.ContainsKey(item.name))
             {
                 Debug.Log(item.name);
             }*/
        }

    }
    private void SetLanguage(string language)
    {
        var lang = myManager.Languages.Where(x => x.Name == language).First();
        foreach (var item in textList)
        {
            item.text = lang.Texts[item.name];
            Debug.Log(item.text);
        }
    }
}
