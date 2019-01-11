using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PropertyDataManager
{
    private const string FILE_NAME = "propertyData.json";

    public static PropertyData GetPropertyData()
    {
        PropertyData data;

        string filePath = Path.Combine(Application.dataPath, FILE_NAME);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<PropertyData>(dataAsJson);
        }
        else
        {
            data = new PropertyData();
        }

        return data;
    }

    public static void SetPropertyData(PropertyData data)
    {
        string dataAsJson = JsonUtility.ToJson(data);

        string filePath = Path.Combine(Application.dataPath, FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }
}

[Serializable]
public class PropertyData
{
    public List<Property> properties;

    public PropertyData()
    {
        this.properties = new List<Property>();
    }

    public PropertyData(List<Property> properties)
    {
        this.properties = properties;
    }
}

[Serializable]
public class Property
{
    public string name;
    public string address;
    public List<Room> rooms = new List<Room>();
}

[Serializable]
public class Room
{
    public string name;
    public int singleBeds;
    public int doubleBeds;
    public int Persons => singleBeds + 2 * doubleBeds;
}
