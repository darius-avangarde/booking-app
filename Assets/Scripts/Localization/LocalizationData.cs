using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LocalizationData : MonoBehaviour
{
    //private Dictionary<string, string> localizedText;
    //public static readonly string csvFile="Texts - Sheet1.csv";
    public IEnumerable<LanguageScript> TextsList { get; set; }
    public List<DataScript> Values = new List<DataScript>();
    private char lineSeperator = '\n'; 
    private char fieldSeperator = ',';
    public string[] data;
    private void Start()
    {
       TextsList = ReadFromCSV(/*@"Texts - Sheet1.csv */"D:\\Booking\\Assets\\Resources\\TextFile.csv");
        foreach (var item in TextsList)
        {
            Debug.Log(item.Name);
            foreach (var col in item.Texts)
            {
                Debug.Log(col.Key + " ----- " + col.Value);
            }
            
            Debug.Log("-----------------------------------");
        }
    }

    private void ReadData()
    {
      //TextAsset  csvFile = Resources.Load<TextAsset>("Texts-Sheet1");
     
       /*  data = csvFile.text.Split(lineSeperator);
        for (int i = 0; i < data.Length-1; i++)
        {
          
            string[] row = data[i].Split(fieldSeperator);
           
                DataScript texts = new DataScript();
                //texts.key = row[0];
               // texts.valueRo = row[1];
                //texts.valueEn = row[2];
               
                Values.Add(texts);
          
        }*/
    }

    public static IEnumerable<LanguageScript> ReadFromCSV(string csvFilePath)
    {
        //string filePath = Path.Combine(Application.persistentDataPath, csvFilePath);
        var result = new List<LanguageScript>();
        if (File.Exists(csvFilePath))
        {
            var fileContent = File.ReadAllLines(csvFilePath);
            var tableHeader = fileContent.First().Split('&').Skip(1).ToList();
            var tableContent = fileContent.Skip(1).ToList();
            foreach (var language in tableHeader)
            {
                var tempLanguage = new LanguageScript();
                tempLanguage.Name = language;
                var tempTexts = new List<DataScript>();
                foreach (var line in tableContent)
                {
                    var tempText = new DataScript();
                    var columns = line.Split('&').ToList();
                    tempText.Key = columns.First();
                    columns = columns.Skip(1).ToList(); 
                    tempText.Value = columns.ElementAt(tableHeader.IndexOf(language));
                    tempTexts.Add(tempText);
                }
                tempLanguage.Texts = tempTexts;
                result.Add(tempLanguage);
            }
        }
        return result;
    }

    public void SetLocalize()
    {

    }
}
