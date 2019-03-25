﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public const string defaultProperyAdminScreenName = "Proprietate nouă";
    public const string Proprietate = "Proprietate: ";
    public const string defaultRoomAdminScreenName = "Camera nouă";
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
    public static string graphBarsColor = "#143730";
    public static string graphExeptBarsColor = "#1C7966";

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

    public static Dictionary<int, string> DayOfWeekNamesDict = new Dictionary<int, string>()
    {
        {1,"Luni"},
        {2, "Marți"},
        {3,"Miercuri"},
        {4,"Joi"},
        {5,"Vineri"},
        {6,"Sambată"},
        {7,"Duminică"}
    };

    public static Dictionary<int, string> XAxisDict = new Dictionary<int, string>()
    {
        {0,"Timp"},
        {1,"Locație"},
        {2,"Tip cameră"},
        {3,"Rezervări"}
    };

    public static Dictionary<int, string> AggregateXAxisDict = new Dictionary<int, string>()
    {
        {0,"Saptămană"},
        {1,"Lună"},
        {2,"An"}
    };
}
