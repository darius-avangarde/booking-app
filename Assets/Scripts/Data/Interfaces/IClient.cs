
using System.Collections.Generic;

public interface IClient
{
    string ID { get; }
    string Name { get; set; }
    string Number { get; set; }
    string Email { get; set; }
    string Adress { get; set; }
}
