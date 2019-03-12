public interface IRoom
{
    string PropertyID { get; }
    string ID { get; }
    string Name { get; set; }
    int SingleBeds { get; set; }
    int DoubleBeds { get; set; }
    int Persons { get; }
}
