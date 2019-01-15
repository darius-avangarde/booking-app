using System.Collections.Generic;

public interface IProperty
{
    string ID { get; }
    string Name { get; set; }
    IEnumerable<IRoom> Rooms { get; }

    IRoom AddRoom();
    IRoom GetRoom(string ID);
    void DeleteRoom(string ID);
}

public interface IRoom
{
    string ID { get; }
    string Name { get; set; }
    int SingleBeds { get; set; }
    int DoubleBeds { get; set; }
    int Persons { get; }
}
