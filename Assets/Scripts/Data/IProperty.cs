using System.Collections.Generic;

public interface IProperty
{
    string ID { get; }
    string Name { get; set; }
    string NrRooms { get; }
    bool HasRooms { get; set; }
    IEnumerable<IRoom> Rooms { get; }
    IEnumerable<IRoom> DeletedRooms { get; }
    IRoom GetPropertyRoom { get; }

    IRoom AddRoom();
    IRoom GetRoom(string ID);
    void DeleteRoom(string ID);
}
