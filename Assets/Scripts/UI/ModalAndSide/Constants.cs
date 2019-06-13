using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    //TODO: Remove bool when no longer used
    public const bool PRETTY_PRINT = true;

    #region Property specific strings
    public const string PROPERTIES_TITLE = "Proprietăți";
    public const string DELETE_PROPERTY = "Ștergeți proprietatea?";
    public const string DELETE_ROOM = "Ștergeți camera?";
    public const string DELETE_ROOM_RESERVATIONS = "Exista rezervari pentru camera selectata, acestea vor fi sterse.";
    public const string DELETE_CLIENT = "Ștergeți clientul?";
    public const string DELETE_CONFIRM = "Ștergeți";
    public const string DELETE_CANCEL = "Anulați";
    public const string NEW_PROPERTY = "Proprietate nouă";
    public const string EDIT_PROPERTY = "Modifică proprietatea";
    public const string NEW_ROOM = "Cameră nouă";
    public const string MULTIPLE_ROOMS = "Camere noi";
    public const string EDIT_ROOM = "Modifică camera";
    public const string ROOMS_NUMBER = "Număr de camere: ";
    public const string AVAILABLE_ROOMS = "Camere disponibile: ";
    public const string PROPERTY = "Proprietate: ";
    public const string PERSONS = "Persoane: ";
    public const string BEDS = "Paturi: ";
    public const string PRICE = "Pret: ";
    public const string SingleBed = "Paturi single: ";
    public const string DoubleBed = "Paturi duble: ";
    public const string OPEN_CAMERA_GALLERY = "Deschideți galeria sau camera foto?";
    public const string OPEN_CAMERA = "Camera";
    public const string OPEN_GALLERY = "Galeria";

    //error messages
    public const string ERR_PROPERTY = "Nu există proprietăți, apăsați pe butonul adaugă proprietate";
    public const string ERR_DISPONIBILITY = "Nu există camere disponibile în intervalul selectat";
    #endregion

    public const string DateTimePrintFormat = "dd/MM/yy";
    public const string AndDelimiter = "  -  ";
    public const string NEW_CLIENT = "Client nou";
    public const string EDIT_CLIENT = "Editează client";
    public const string NameRequired = " Te rog adaugă un nume!";
    public const string PhoneRequired = "Te rog adaugă un număr de telefon!";
    public const string Name_Phone_Required = "Numele și telefonul sunt necesare!";
    public const string MessageEmail = "Nu există email înregistrat!";
    public const string Valid_Email = "Inserează o adresă de email validă!";
    public const string defaultCustomerName = "NumeClient";
    public const string defaultPropertyPicture = "DefaultPicture";
    public const string SPACE = " ";

    #region Reservation specific strings
        public const string CLIENTS_TITLE = "Clienți";
        public const string DELETE_DIALOG = "Ștergeți rezervarea?";
        public const string EDIT_DIALOG = "Moficați rezervarea?";
        public const string EDIT_TITLE = "Modifică rezervarea";
        public const string NEW_TITLE = "Rezervare nouă";
        public const string NEWLINE = "\n";
        public const string DAY_COUNT_PREF = "Ați selectat";
        public const string DAY_COUNT_SUFF_PL = "nopți.";
        public const string DAY_COUNT_SUFF_SN = "noapte.";
        public const string CHOOSE = "Alege";
        public const string ROOMS_SELECTED = "camere selectate";
        public const string RESERVATION_PERIOD = "perioadă";

        //error messages
        public const string ERR_PROP = "Specificați proprietatea pentru această rezervare";
        public const string ERR_ROOM = "Specificați camera pentru această rezervare";
        public const string ERR_PERIOD = "Există deja o rezervare pentru această cameră care se suprapune cu perioada selectată";
        public const string ERR_CLIENT = "Este necesară selectarea unui client pentru a crea sau modifica rezervarea";
        public const string ERR_DATES = "Specificați perioada rezervării";
    #endregion

    // COLORS
    public readonly static Color darkTextColor = new Color(0.2313726f, 0.2196078f, 0.2078431f);
    public readonly static Color lightTextColor = new Color(0.7019608f, 0.6745098f, 0.6470588f);
    public readonly static Color lightGreenColor = new Color(0.3686275f, 0.772549f, 0.7960784f);
    public readonly static Color mediumGreenColor = new Color(0.09019608f, 0.6039216f, 0.6352941f);
    public readonly static Color darkGreenColor = new Color(0.06666667f, 0.4352942f, 0.4784314f);
    public readonly static Color lightBackgroundColor = new Color(0.9882354f, 0.9686275f, 0.9725491f);
    public readonly static Color availableItemColor = new Color(0, 0.6941176f, 0);
    public readonly static Color reservedUnavailableItemColor = new Color(0.8078431f, 0.0117647f, 0.0117647f);
    public readonly static Color reservedAvailableItemColor = new Color(0.909804f, 0.6705883f, 0.1882353f);
    public readonly static Color graphBarColor1 = new Color(0.909804f, 0.6705883f, 0.1882353f);
    public readonly static Color graphBarColor2 = new Color(0.909804f, 0.6705883f, 0.1882353f);
    public readonly static Color unavailableItemColor = new Color(0.8941177f, 0.8745098f, 0.854902f);
    public readonly static Color selectedItemColor = new Color(0.05490196f, 0.5294118f, 0.5568628f);
    public readonly static Color defaultTextCollor = new Color(0.74901960f, 0.74901960f, 0.74901960f);

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
