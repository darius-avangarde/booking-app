using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instance;
    public const string csvFile= "TextFile.csv";
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

    private void Start()
    {
      
        Languages = ReadFromCSV();//ReadFromCSV("Assets\\Resources\\TextsFileEx.csv"); 
       
    }

    public  IEnumerable<LanguageScript> ReadFromCSV()//string csvFilePath)
    {
        string csvFilePath = Path.Combine(Application.persistentDataPath, csvFile);
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
