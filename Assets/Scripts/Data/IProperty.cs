using System.Collections.Generic;

public interface IProperty
{
    string ID { get; }
    string Name { get; set; }
    string NrRooms { get; }
    string GetPropertyRoomID { get; set; }
    bool HasRooms { get; set; }
    IEnumerable<IRoom> Rooms { get; }
    IEnumerable<IRoom> DeletedRooms { get; }

    IRoom AddRoom();
    IRoom GetRoom(string ID);
    IRoom GetPropertyRoom();
    void SaveRoom(IRoom room);
    void DeleteRoom(string ID);
}
