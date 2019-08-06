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
    }

    public IEnumerable<LanguageScript> ReadFromCSV()
    {
        List<LanguageScript> result = new List<LanguageScript>();
        string[] fileContent = Read("TextFileCsv");
        List<string> tableHeader = fileContent.First().Split(fieldSeperator).Skip(1).ToList();
        List<string> tableContent = fileContent.Skip(1).ToList();
        
        for (int i = 0; i < tableHeader.Count; i++)
        {
            LanguageScript tempLanguage = new LanguageScript();
            tempLanguage.Name = tableHeader[i];
            Dictionary<string, string> tempTexts = new Dictionary<string, string>();
           
            for (int j = 0; j < tableContent.Count; j++)
            {

                List<string> columns = tableContent[j].Split(fieldSeperator).ToList();
                string key = columns.First();
                columns = columns.Skip(1).ToList();
                string value = columns.ElementAt(tableHeader.IndexOf(tableHeader[i]));
                tempTexts.Add(key, value);
            }
            tempLanguage.Texts = tempTexts;
            result.Add(tempLanguage);
        }

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
