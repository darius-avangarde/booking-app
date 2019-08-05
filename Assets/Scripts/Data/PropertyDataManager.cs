using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class PropertyDataManager
{
    public const string DATA_FILE_NAME = "propertyData.json";

    private static PropertyData cache;
    public enum PropertyType { hotel, guesthouse, villa, cabin}
    public enum RoomType { room, apartment, cabin, villa, bed}

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

    /// <summary>
    /// save property data to json file
    /// </summary>
    private static void WritePropertyData()
    {
        string dataAsJson = JsonUtility.ToJson(Data, true);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    // we're using interfaces to restrict data mutation to methods and properties that we control
    // this allows us to make sure that the data file is updated automatically with each change
    public static IEnumerable<IProperty> GetProperties()
    {
        return Data.properties.FindAll(p => !p.Deleted);
    }

    /// <summary>
    /// get all deleted properties
    /// </summary>
    /// <returns>list of deleted properties</returns>
    public static IEnumerable<IProperty> GetDeletedProperties()
    {
        return Data.properties.FindAll(p => p.Deleted);
    }

    /// <summary>
    /// get property with the given property id
    /// </summary>
    /// <param name="ID">property id</param>
    /// <returns>Property object</returns>
    public static IProperty GetProperty(string ID)
    {
        return Data.properties.Find(p => p.ID.Equals(ID) && !p.Deleted);
    }

    /// <summary>
    /// add a new property
    /// this function does not save the new property to json
    /// </summary>
    /// <returns>Property object</returns>
    public static IProperty AddProperty()
    {
        Property newProperty = new Property();
        return newProperty;
    }

    /// <summary>
    /// create a default room for a property without rooms
    /// </summary>
    /// <param name="property">current property</param>
    public static void CreatePropertyRoom(IProperty property)
    {
        IRoom newRoom = property.AddRoom();
        newRoom.Name = property.Name;
        if (property.PropertyType == PropertyType.cabin)
        {
            newRoom.RoomType = RoomType.cabin;
        }
        else if (property.PropertyType == PropertyType.villa)
        {
            newRoom.RoomType = RoomType.villa;
        }
        property.SaveRoom(newRoom);
        property.GetPropertyRoomID = newRoom.ID;
    }

    /// <summary>
    /// save the current property to json file
    /// </summary>
    /// <param name="property">selected property</param>
    public static void SaveProperty(IProperty property)
    { 
        Data.properties.Add((Property)property);
        WritePropertyData();
    }

    /// <summary>
    /// delete selected property
    /// </summary>
    /// <param name="ID">selected property id</param>
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
        private bool hasRooms = true;
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
        private int floors = 0;
        public int Floors
        {
            get => floors;
            set
            {
                floors = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private int[] floorRooms;
        public int[] FloorRooms
        {
            get => floorRooms;
            set
            {
                floorRooms = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private int propertyType = (int)PropertyType.hotel;
        public PropertyType PropertyType
        {
            get => (PropertyType)propertyType;
            set
            {
                propertyType = (int)value;
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

        public Property()
        {
            this.id = Guid.NewGuid().ToString();
        }

        [SerializeField]
        private List<Room> rooms = new List<Room>();
        public IEnumerable<IRoom> Rooms => rooms.FindAll(r => !r.Deleted);
        public IEnumerable<IRoom> DeletedRooms => rooms.FindAll(r => r.Deleted);

        [SerializeField]
        private string getPropertyRoomID;
        public string GetPropertyRoomID
        {
            get => getPropertyRoomID;
            set
            {
                getPropertyRoomID = value;
                WritePropertyData();
            }
        }

        public IRoom GetRoom(string ID) => rooms.Find(r => r.ID.Equals(ID));

        /// <summary>
        /// return default room for a property without rooms
        /// </summary>
        /// <returns>room object</returns>
        public IRoom GetPropertyRoom()
        {
            return GetRoom(getPropertyRoomID);
        }

        /// <summary>
        /// add a new room to current property
        /// this function does not save the room to json
        /// </summary>
        /// <returns>Room object</returns>
        public IRoom AddRoom()
        {
            Room newRoom = new Room(id);
            return newRoom;
        }

        /// <summary>
        /// save current room to json file
        /// </summary>
        /// <param name="room">current room</param>
        public void SaveRoom(IRoom room)
        {
            rooms.Add((Room)room);
            WritePropertyData();
        }

        /// <summary>
        /// save a list of rooms to json
        /// </summary>
        /// <param name="roomsList">list of room objects</param>
        public void SaveMultipleRooms(List<IRoom> roomsList)
        {
            List<Room> multipleRooms = new List<Room>();
            foreach (var room in roomsList)
            {
                multipleRooms.Add((Room)room);
            }
            rooms.AddRange(multipleRooms);
            WritePropertyData();
        }

        /// <summary>
        /// delete selected room from json
        /// </summary>
        /// <param name="ID">selected room id</param>
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
        private int roomType = (int)RoomType.room;
        public RoomType RoomType
        {
            get => (RoomType)roomType;
            set
            {
                roomType = (int)value;
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
        private bool multiple = false;
        public bool Multiple
        {
            get => multiple;
            set
            {
                multiple = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private int floor = 0;
        public int Floor
        {
            get => floor;
            set
            {
                floor = value;
                WritePropertyData();
            }
        }

        [SerializeField]
        private int roomNumber = 0;
        public int RoomNumber
        {
            get => roomNumber;
            set
            {
                roomNumber = value;
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
