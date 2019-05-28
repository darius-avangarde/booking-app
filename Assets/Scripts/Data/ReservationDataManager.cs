using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ReservationDataManager
{
    public const string DATA_FILE_NAME = "reservationData.json";

    private static ReservationData cache;
    private static ReservationData Data
    {
        get
        {
            if (cache == null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
                if (File.Exists(filePath))
                {
                    string dataAsJson = File.ReadAllText(filePath);
                    cache = JsonUtility.FromJson<ReservationData>(dataAsJson);
                }
                else
                {
                    cache = new ReservationData();
                }
            }

            return cache;
        }
    }

    private static void WriteReservationData()
    {
        string dataAsJson = JsonUtility.ToJson(Data, Constants.PRETTY_PRINT);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    // we're using interfaces to restrict data mutation to methods and properties that we control
    // this allows us to make sure that the data file is updated automatically with each change
    public static IEnumerable<IReservation> GetReservations()
    {
        return Data.reservations.FindAll(r => !r.Deleted);
    }

    ///<summary>
    ///Gets all reservations (including deleted that match the given predicate)
    ///</summary>
    public static IEnumerable<IReservation> GetReservations(Predicate<IReservation> match)
    {
        return Data.reservations.FindAll(match);
    }

    ///<summary>
    ///Returns all reservations with the start or end date between(non-equal to) the given from and to DateTimes.
    ///</summary>
    public static IEnumerable<IReservation> GetReservationsBetween(DateTime from, DateTime to)
    {
        return Data.reservations.FindAll(r => !r.Deleted &&
            (r.Period.End.Date > from.Date && r.Period.End.Date < to.Date) ||       //check if interval start is between from/to
            (r.Period.Start.Date > from.Date && r.Period.Start.Date < to.Date) ||   //check if interval end is between from/to
            (from.Date > r.Period.Start.Date && from.Date < r.Period.End.Date) ||   //check if from is in interval
            (to.Date > r.Period.Start.Date && to.Date < r.Period.End.Date) ||       //check if to is in interval
            (from.Date == r.Period.Start || to.Date == r.Period.End.Date)           //chek if either start or end matches any reservation's start or end resepectively
            );
    }

    public static IEnumerable<IReservation> GetDeletedReservations()
    {
        return Data.reservations.FindAll(r => r.Deleted);
    }

    public static IReservation GetReservation(string ID)
    {
        return Data.reservations.Find(r => r.ID.Equals(ID));
    }

    ///<summary>
    ///Returns all active* and undeleted reservations for the given roomID
    ///<para>*with the end date equal to the current day or in the future</para>
    ///</summary>
    public static IEnumerable<IReservation> GetActiveRoomReservations(string roomID)
    {
        return Data.reservations.FindAll(r => !r.Deleted && r.Period.End.Date > DateTime.Today.Date && r.RoomID == roomID);
    }


    ///<summary>
    ///Returns all active* and undeleted reservations for the given clientID
    ///<para>*with the end date equal to the current day or in the future</para>
    ///</summary>
    public static IEnumerable<IReservation> GetActiveClientReservations(string clientID)
    {
        return Data.reservations.FindAll(r => !r.Deleted && r.Period.End.Date > DateTime.Today.Date && r.CustomerID == clientID);
    }

    // TODO: Remove unused AddReservation
    ///<summary>
    ///Do not use this method (both client and reservation objects are required with the new system).
    /// Use the ".AddReservation(IRoom room, IClient client, DateTime start, DateTime end)" overload instead.
    ///</summary>
    public static IReservation AddReservation(IRoom room)
    {
        Reservation newReservation = new Reservation(room);
        Data.reservations.Add(newReservation);
        WriteReservationData();

        return newReservation;
    }

    ///<summary>
    ///Creates a new reservation with the given room, client and period data and returns it, this also saves the new reservation to file
    ///</summary>
    public static IReservation AddReservation(IRoom room, IClient client, DateTime start, DateTime end)
    {
        Reservation newReservation = new Reservation(room, client.ID, start,end);
        Data.reservations.Add(newReservation);
        WriteReservationData();

        return newReservation;
    }

    ///<summary>
    ///Updates the selected reservation's room, client, and period and returns it, this also updates the saved reservation data on file
    ///</summary>
    public static IReservation EditReservation(IReservation reservation, IRoom room, IClient client, DateTime start, DateTime end)
    {
        reservation.EditReservation(room, client, start, end);
        WriteReservationData();
        return reservation;
    }

    public static void DeleteReservation(string ID)
    {
        Reservation reservation = Data.reservations.Find(r => r.ID.Equals(ID));
        if (reservation != null)
        {
            reservation.Deleted = true;
            WriteReservationData();
        }
    }

    public static void DeleteReservationsForRoom(string roomID)
    {
        List<Reservation> reservations = Data.reservations.FindAll(r => r.RoomID.Equals(roomID));
        foreach (var reservation in reservations)
        {
            reservation.Deleted = true;
        }
        WriteReservationData();
    }

    public static void DeleteReservationsForProperty(string propertyID)
    {
        List<Reservation> reservations = Data.reservations.FindAll(r => r.PropertyID.Equals(propertyID));
        foreach (var reservation in reservations)
        {
            reservation.Deleted = true;
        }
        WriteReservationData();
    }

    public static void DeleteReservationsForClient(string clientID)
    {
        List<Reservation> reservations = Data.reservations.FindAll(r => r.CustomerID.Equals(clientID));
        foreach (var reservation in reservations)
        {
            reservation.Deleted = true;
        }
        WriteReservationData();
    }

    public static IDateTimePeriod DefaultPeriod()
    {
        return new DateTimePeriod(DateTime.Today, DateTime.Today);
    }

    [Serializable]
    private class ReservationData
    {
        public List<Reservation> reservations;

        public ReservationData() => this.reservations = new List<Reservation>();
    }

    [Serializable]
    private class Reservation : IReservation
    {
        [SerializeField]
        private string id;
        public string ID => id;

        [SerializeField]
        private bool deleted = false;
        public bool Deleted
        {
            get => deleted;
            set
            {
                deleted = value;
                WriteReservationData();
            }
        }

        [SerializeField]
        private string propertyID;
        public string PropertyID => propertyID;

        [SerializeField]
        private string roomID;
        public string RoomID => roomID;

        public string CustomerName
        {
            get => ClientDataManager.GetClient(customerID).Name;
            set
            {
                ClientDataManager.GetClient(customerID).Name = value;
                WriteReservationData();
            }
        }

        [SerializeField]
        private string customerID;
        public string CustomerID
        {
            get => customerID;
            set
            {
                customerID = value;
                WriteReservationData();
            }
        }

        [SerializeField]
        private DateTimePeriod period;
        public IDateTimePeriod Period
        {
            get => period;
        }

        public Reservation(IRoom room)
        {
            this.id = Guid.NewGuid().ToString();
            this.propertyID = room.PropertyID;
            this.roomID = room.ID;
            this.period = new DateTimePeriod(DateTime.Today, DateTime.Today.AddDays(1f));
            WriteReservationData();
        }

        public Reservation(IRoom room, string customerID, DateTime start, DateTime end)
        {
            this.id = Guid.NewGuid().ToString();
            this.propertyID = room.PropertyID;
            this.roomID = room.ID;
            this.customerID = customerID;
            this.period = new DateTimePeriod(start, end);
            WriteReservationData();
        }

        public void EditReservation(IRoom room, IClient client, DateTime start, DateTime end)
        {
            this.propertyID = room.PropertyID;
            this.roomID = room.ID;
            this.customerID = client.ID;
            this.period = new DateTimePeriod(start, end);
            WriteReservationData();
        }
    }

    [Serializable]
    private class DateTimePeriod : IDateTimePeriod
    {
        // we store the DateTimes as UTC,
        // this ensures that the serialized data is the same regardless of timezone.
        // we simply convert to local time on deserialization
        public long startTicks;
        public DateTime Start
        {
            get => new DateTime(startTicks, DateTimeKind.Utc).ToLocalTime();
            set
            {
                startTicks = value.ToUniversalTime().Ticks;
                if (startTicks > endTicks)
                {
                    endTicks = startTicks;
                }
                WriteReservationData();
            }
        }

        public long endTicks;
        public DateTime End
        {
            get => new DateTime(endTicks, DateTimeKind.Utc).ToLocalTime();
            set
            {
                endTicks = value.ToUniversalTime().Ticks;
                if (endTicks < startTicks)
                {
                    startTicks = endTicks;
                }
                WriteReservationData();
            }
        }

        public DateTimePeriod(DateTime start, DateTime end)
        {
            startTicks = start.Ticks;
            endTicks = end.Ticks;
        }

        public bool Includes(DateTime dateTime)
        {
            long ticks = dateTime.ToUniversalTime().Ticks;
            return (startTicks <= ticks) && (ticks <= endTicks);
        }

        public bool Overlaps(DateTime start, DateTime end)
        {
            long overlapStartTicks = start.ToUniversalTime().Ticks;
            long overlapEndTicks = end.ToUniversalTime().Ticks;
            return startTicks <= overlapEndTicks || endTicks >= overlapStartTicks;
        }

        ///<summary>
        ///Compares the the period to the given one, returns true if they overlap for more then one day (a reservations start date can be the same as another's end date)
        ///<para>Ex(days): true for 22-25 and 24-26, true for 22-23 and 22-23, false for 22-23 and 23-24 etc...</para>
        ///</summary>
        public bool Overlaps(IDateTimePeriod period)
        {
            long checkStart = period.Start.ToUniversalTime().Ticks;
            long checkEnd = period.End.ToUniversalTime().Ticks;

            return
            (checkStart < endTicks && checkStart > startTicks) ||   //returns true if start is between start and end
            (checkEnd > startTicks && checkEnd < endTicks);         //returns ture if end is between start and end
        }
    }
}
