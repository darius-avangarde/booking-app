using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class PropertyDataManager
{
    public const string DATA_FILE_NAME = "propertyData.json";

    private static PropertyData cache;

    private static PropertyData Data
    {
        get
        {
            if (cache == null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
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
    }

    private static void WritePropertyData()
    {
        string dataAsJson = JsonUtility.ToJson(Data);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    // we're using interfaces to restrict data mutation to methods and properties that we control
    // this allows us to make sure that the data file is updated automatically with each change
    public static IEnumerable<IProperty> GetProperties()
    {
        return Data.properties.FindAll(p => !p.Deleted);
    }

    public static IEnumerable<IProperty> GetDeletedProperties()
    {
        return Data.properties.FindAll(p => p.Deleted);
    }

    public static IProperty GetProperty(string ID)
    {
        return Data.properties.Find(p => p.ID.Equals(ID));
    }

    public static IProperty AddProperty()
    {
        Property newProperty = new Property();
        //Data.properties.Add(newProperty);
        //WritePropertyData();
        return newProperty;
    }

    public static void SaveProperty(IProperty property)
    {
        Data.properties.Add((Property)property);
        WritePropertyData();
    }

    public static void DeleteProperty(string ID)
    {
        Property property = Data.properties.Find(p => p.ID.Equals(ID));
        if (property != null)
        {
            property.Deleted = true;
            WritePropertyData();
        }
    }

    [Serializable]
    private class PropertyData
    {
        public List<Property> properties;

        public PropertyData()
        {
            this.properties = new List<Property>();
        }
    }

    [Serializable]
    private class Property : IProperty
    {
        [SerializeField]
        private string id;
        public string ID => id;

        [SerializeField]
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private bool hasRooms = false;
        public bool HasRooms
        {
            get => hasRooms;
            set
            {
                hasRooms = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private bool deleted = false;
        public bool Deleted
        {
            get => deleted;
            set
            {
                deleted = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private List<Room> rooms = new List<Room>();
        public IEnumerable<IRoom> Rooms => rooms.FindAll(r => !r.Deleted);
        public IEnumerable<IRoom> DeletedRooms => rooms.FindAll(r => r.Deleted);
        public string NrRooms => Rooms.Count().ToString();

        [SerializeField]
        private IRoom getPropertyRoom = null;
        public IRoom GetPropertyRoom => getPropertyRoom;

        public Property()
        {
            this.id = Guid.NewGuid().ToString();
        }

        public IRoom GetRoom(string ID) => rooms.Find(r => r.ID.Equals(ID));

        public IRoom AddRoom()
        {
            Room newRoom = new Room(id);
            if (!HasRooms)
            {
                newRoom.Name = name;
                getPropertyRoom = newRoom;
            }
            rooms.Add(newRoom);
            WritePropertyData();

            return newRoom;
        }

        public void DeleteRoom(string ID)
        {
            Room room = rooms.Find(r => r.ID.Equals(ID));
            if (room != null)
            {
                room.Deleted = true;
                WritePropertyData();
            }
        }
    }

    [Serializable]
    private class Room : IRoom
    {
        [SerializeField]
        private string propertyID;
        public string PropertyID => propertyID;

        [SerializeField]
        private string id;
        public string ID => id;

        [SerializeField]
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private string price;
        public string Price
        {
            get => price;
            set
            {
                price = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private bool deleted = false;
        public bool Deleted
        {
            get => deleted;
            set
            {
                deleted = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private int singleBeds;
        public int SingleBeds
        {
            get => singleBeds;
            set
            {
                singleBeds = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private int doubleBeds;
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
