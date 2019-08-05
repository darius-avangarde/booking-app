using System.Collections.Generic;

public interface IProperty
{
    string ID { get; }
    string Name { get; set; }
    string GetPropertyRoomID { get; set; }
    bool HasRooms { get; set; }
    int Floors { get; set; }
    int[] FloorRooms { get; set; }
    PropertyDataManager.PropertyType PropertyType { get; set; }
    IEnumerable<IRoom> Rooms { get; }
    IEnumerable<IRoom> DeletedRooms { get; }

    IRoom AddRoom();
    IRoom GetRoom(string ID);
    IRoom GetPropertyRoom();
    void SaveRoom(IRoom room);
    void SaveMultipleRooms(List<IRoom> roomsList);
    void DeleteRoom(string ID);
}
