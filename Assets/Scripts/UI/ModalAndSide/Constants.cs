using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public const string defaultProperyAdminScreenName = "Proprietate noua";
    public const string Proprietate = "Proprietate: ";
    public const string defaultRoomAdminScreenName = "Camera noua";
    public const string defaultCustomerName = "NumeClient";
    public const string Persoane = "Persoane: ";
    public const string SingleBed = "Paturi single: ";
    public const string DoubleBed = "Paturi duble: ";

    public const string DateTimePrintFormat = "dd/MM/yy";
    public const string AndDelimiter = "  -  ";

    public static Color reservedUnavailableItemColor = Color.red;
    public static Color reservedAvailableItemColor = Color.yellow;
    public static Color unavailableItemColor = Color.gray;
    public static Color selectedItemColor = Color.cyan;

    public static Dictionary<int, string> MonthNamesDict = new Dictionary<int, string>()
    {
        {1,"Ianuarie"},
        {2, "Februarie"},
        {3,"Martie"},
        {4,"Aprilie"},
        {5,"Mai"},
        {6,"Iunie"},
        {7,"Iulie"},
        {8,"August"},
        {9,"Septembrie"},
        {10,"Octombrie"},
        {11,"Noiembrie"},
        {12,"Decembrie"}
    };

    public static Dictionary<int, string> XAxisDict = new Dictionary<int, string>()
    {
        {0,"Timp"},
        {1,"Locatie"},
        {2,"Categorie de camera"},
        {3,"Zile Rezervate in Camera"}
    };
}
