using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PropertyDataManager
{
    private const string FILE_NAME = "propertyData.json";

    private static PropertyData cache;

    private static PropertyData GetPropertyData()
    {
        if (cache == null)
        {
            string filePath = Path.Combine(Application.dataPath, FILE_NAME);
            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                cache = JsonUtility.FromJson<PropertyData>(dataAsJson);
            }
            else
            {
                cache = new PropertyData();
            }
        }

        return cache;
    }

    private static void WritePropertyData()
    {
        PropertyData data = GetPropertyData();
        string dataAsJson = JsonUtility.ToJson(data);

        string filePath = Path.Combine(Application.dataPath, FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    // we're using interfaces to restrict data mutation to methods and properties that we control
    // this allows us to make sure that the data file is updated automatically with each change
    public static IEnumerable<IProperty> GetProperties()
    {
        PropertyData data = GetPropertyData();
        return data.properties;
    }

    public static IProperty GetProperty(string ID)
    {
        PropertyData data = GetPropertyData();
        return data.properties.Find(p => p.ID.Equals(ID));
    }

    public static IProperty AddProperty()
    {
        Property newProperty = new Property();
        GetPropertyData().properties.Add(newProperty);
        WritePropertyData();

        return newProperty;
    }

    public static void DeleteProperty(string ID)
    {
        PropertyData data = GetPropertyData();
        GetPropertyData().properties.RemoveAll(p => p.ID.Equals(ID));
        WritePropertyData();
    }

    [Serializable]
    private class PropertyData
    {
        public List<Property> properties;

        public PropertyData() => this.properties = new List<Property>();
    }

    [Serializable]
    private class Property : IProperty
    {
        public string id;
        public string name;
        public List<Room> rooms = new List<Room>();

        public string ID => id;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                WritePropertyData();
            }
        }
        public IEnumerable<IRoom> Rooms => rooms;

        public Property() => this.id = Guid.NewGuid().ToString();

        public IRoom GetRoom(string ID) => rooms.Find(r => r.ID.Equals(ID));

        public IRoom AddRoom()
        {
            Room newRoom = new Room(id);
            rooms.Add(newRoom);
            WritePropertyData();

            return newRoom;
        }

        public void DeleteRoom(string ID)
        {
            rooms.RemoveAll(r => r.ID.Equals(ID));
            WritePropertyData();
        }
    }

    [Serializable]
    private class Room : IRoom
    {
        public string propertyID;
        public string PropertyID => propertyID;

        public string id;
        public string ID => id;

        public string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                WritePropertyData();
            }
        }

        public int singleBeds;
        public int SingleBeds
        {
            get => singleBeds;
            set
            {
                singleBeds = value;
                WritePropertyData();
            }
        }

        public int doubleBeds;
        public int DoubleBeds
        {
            get => doubleBeds;
            set
            {
                doubleBeds = value;
                WritePropertyData();
            }
        }

        public int Persons => singleBeds + 2 * doubleBeds;

        public Room(string propertyID)
        {
            this.id = Guid.NewGuid().ToString();
            this.propertyID = propertyID;
        }
    }
}
