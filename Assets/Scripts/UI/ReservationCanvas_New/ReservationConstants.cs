//TODO: Move to constants
public class ReservationConstants
{
    public const string CLIENTS_TITLE = "Clienți";
    public const string DELETE_DIALOG = "Ștergeți rezervarea?";
    public const string EDIT_DIALOG = "Moficați rezervarea?";
    public const string EDIT_TITLE = "Modifică rezervarea";
    public const string NEW_TITLE = "Rezervare nouă";
    public const string NEWLINE = "\n";

    #region Reservation edit error messages
        public const string ERR_PROP = "Specificați proprietatea pentru această rezervare";
        public const string ERR_ROOM = "Specificați camera pentru această rezervare";
        public const string ERR_PERIOD = "Există deja o rezervare pe această cameră care se suprapune cu perioada selectată";
        public const string ERR_CLIENT = "Este necesară selectarea unui client pentru a crea sau modifica rezervarea";
        public const string ERR_DATES = "Data de sfârșit a rezervarii trebuie să fie cu cel puțin o zi după de data de început a acesteia";
    #endregion
}
