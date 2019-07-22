using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;
    //public static readonly string csvFile="Texts - Sheet1.csv";
    public IEnumerable<LanguageScript> Languages { get; set; }
    private static char fieldSeperator = '&';
    public static LocalizationManager Instance { get
        {
            if (instance == null )
            {
                instance = FindObjectOfType<LocalizationManager>();
            }
            return instance;
        }
    }
   /* void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }*/
    private void CheckSystemLanguage()
    {
        if (Application.systemLanguage == SystemLanguage.English)
        {
            Debug.Log("This system is in English. ");
        }
        else if (Application.systemLanguage == SystemLanguage.Romanian)
        { 
           Debug.Log("This system is in Romanian.");
        }
    }
    private void Start()
    {
        CheckSystemLanguage();
        Languages = ReadFromCSV(/*@"Texts - Sheet1.csv */"D:\\Booking\\Assets\\Resources\\TextsFileEx.csv"); 
        foreach (var item in Languages)
        {
           // Debug.Log(item.Name);
            foreach (var col in item.Texts)
            {
                //Debug.Log(col.Key + " ----- " + col.Value);
            }

          // Debug.Log("-----------------------------------");
        }
    }

    public  IEnumerable<LanguageScript> ReadFromCSV(string csvFilePath)
    {
        //string filePath = Path.Combine(Application.persistentDataPath, csvFilePath);
        var result = new List<LanguageScript>();
        if (File.Exists(csvFilePath))
        {
            var fileContent = File.ReadAllLines(csvFilePath);
            var tableHeader = fileContent.First().Split(fieldSeperator).Skip(1).ToList();
            var tableContent = fileContent.Skip(1).ToList();
            foreach (var language in tableHeader)
            {
                var tempLanguage = new LanguageScript();
                tempLanguage.Name = language;
                var tempTexts = new Dictionary<string, string>();
                foreach (var line in tableContent)
                {
                  
                    var columns = line.Split(fieldSeperator).ToList();
                    var key = columns.First();
                    columns = columns.Skip(1).ToList();
                    var value = columns.ElementAt(tableHeader.IndexOf(language));
                    tempTexts.Add(key, value);
                }
                tempLanguage.Texts = tempTexts;
                result.Add(tempLanguage);
            }
        }
        Debug.Log(result.Count());
        Languages = result;
        return result;
    }


   
}
