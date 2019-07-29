using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instance;
    public const string csvFile = "TextsFileEx.csv";
    public IEnumerable<LanguageScript> Languages { get; set; }
    private static char fieldSeperator = '&';
    public static LocalizationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LocalizationManager>();
            }
            return instance;
        }
    }

    public void CustomStart()
    {
        Languages = ReadFromCSV();
        foreach (var item in Languages)
        {
            foreach (var col in item.Texts)
            {

                Debug.Log(col.Key + " ----- " + col.Value);
            }

            Debug.Log("-----------------------------------");
        }
    }

    public IEnumerable<LanguageScript> ReadFromCSV()
    {
        var result = new List<LanguageScript>();
        var fileContent = Read("TextFileCsv");
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

        Debug.Log(result.Count);
        Languages = result;
        return result;
    }

    public static string[] Read(string filename)
    {
      
        TextAsset theTextFile = Resources.Load<TextAsset>(filename);

      
        if (theTextFile != null)
            return theTextFile.text.Split('\n');

        return new string[] { "Empty" };
    }

}
