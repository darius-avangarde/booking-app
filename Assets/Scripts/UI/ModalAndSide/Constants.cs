using System.Collections;
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

    // COLORS
    public readonly static Color darkTextColor = new Color(0.2313726f, 0.2196078f, 0.2078431f);
    public readonly static Color lightTextColor = new Color(0.7019608f, 0.6745098f, 0.6470588f);
    public readonly static Color lightGreenColor = new Color(0.3686275f, 0.772549f, 0.7960784f);
    public readonly static Color mediumGreenColor = new Color(0.09019608f, 0.6039216f, 0.6352941f);
    public readonly static Color darkGreenColor = new Color(0.06666667f, 0.4352942f, 0.4784314f);
    public readonly static Color lightBackgroundColor = new Color(0.9882354f, 0.9686275f, 0.9725491f);
    public readonly static Color availableItemColor = new Color(0.2980392f, 0.7254902f, 0.2666667f);
    public readonly static Color reservedUnavailableItemColor = new Color(0.8705883f, 0.2117647f, 0.1372549f);
    public readonly static Color reservedAvailableItemColor = new Color(0.909804f, 0.6705883f, 0.1882353f);
    public readonly static Color graphBarColor1 = new Color(0.909804f, 0.6705883f, 0.1882353f);
    public readonly static Color graphBarColor2 = new Color(0.909804f, 0.6705883f, 0.1882353f);
    public readonly static Color unavailableItemColor = new Color(0.8941177f, 0.8745098f, 0.854902f);
    public readonly static Color selectedItemColor = new Color(0.05490196f, 0.5294118f, 0.5568628f);

    public static Dictionary<int, string> MonthNamesDict = new Dictionary<int, string>()
    {
        {1, "Ianuarie"},
        {2, "Februarie"},
        {3, "Martie"},
        {4, "Aprilie"},
        {5, "Mai"},
        {6, "Iunie"},
        {7, "Iulie"},
        {8, "August"},
        {9, "Septembrie"},
        {10, "Octombrie"},
        {11, "Noiembrie"},
        {12, "Decembrie"}
    };

    public static List<string> MonthNames = new List<string>()
    {
        "Ian",
        "Feb",
        "Mar",
        "Apr",
        "Mai",
        "Iun",
        "Iul",
        "Aug",
        "Sep",
        "Oct",
        "Noi",
        "Dec"
    };

    public static List<string> DayOfWeekNames = new List<string>()
    {
        "Lun",
        "Mar",
        "Mie",
        "Joi",
        "Vin",
        "Sam",
        "Dum"
    };

    public static Dictionary<int, string> XAxisDict = new Dictionary<int, string>()
    {
        {0, "Timp"},
        {1, "Locație"},
        {2, "Tip cameră"},
        {3, "Cameră"}
    };

    public static Dictionary<int, string> AggregateXAxisDict = new Dictionary<int, string>()
    {
        {0, "Saptămană"},
        {1, "Lună"},
        {2, "An"}
    };
}
