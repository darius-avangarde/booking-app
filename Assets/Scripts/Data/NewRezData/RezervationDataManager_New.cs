using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ReservationDataManager_New
{
    public const string DATA_FILE_NAME = "reservationData_new.json";

    private static ReservationData_New cache;
    private static ReservationData_New Data
    {
        get
        {
            if (cache == null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
                if (File.Exists(filePath))
                {
                    string dataAsJson = File.ReadAllText(filePath);
                    cache = JsonUtility.FromJson<ReservationData_New>(dataAsJson);
                }
                else
                {
                    cache = new ReservationData_New();
                }
            }

            return cache;
        }
    }

    private static void WriteReservationData()
    {
        string dataAsJson = JsonUtility.ToJson(Data);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    // we're using interfaces to restrict data mutation to methods and properties that we control
    // this allows us to make sure that the data file is updated automatically with each change

    ///<summary>
    ///Returns an IEnumerable of all undeleted reservations.
    ///</summary>
    public static IEnumerable<IReservation_New> GetReservations()
    {
        return Data.reservations.FindAll(r => !r.Deleted);
    }

    public static IEnumerable<IReservation_New> GetReservations(Predicate<IReservation_New> match)
    {
        return Data.reservations.FindAll(match);
    }

    public static IEnumerable<IReservation_New> GetDeletedReservations()
    {
        return Data.reservations.FindAll(r => r.Deleted);
    }

    public static IReservation_New GetReservation(string ID)
    {
        return Data.reservations.Find(r => r.ID.Equals(ID));
    }

    public static IReservation_New AddReservation(IRoom room)
    {
        Reservation_New newReservation = new Reservation_New(room);
        Data.reservations.Add(newReservation);
        WriteReservationData();

        return newReservation;
    }

    public static IReservation_New AddReservation(IRoom room, string customerID, DateTime start, DateTime end)
    {
        Reservation_New newReservation = new Reservation_New(room,customerID,start,end);
        Data.reservations.Add(newReservation);
        WriteReservationData();

        return newReservation;
    }

    public static void EditReservation(IReservation_New reservation, IRoom room, string customerID, DateTime start, DateTime end)
    {
        reservation.EditReservation(room,customerID,start,end);
        WriteReservationData();
    }

    public static void DeleteReservation(string ID)
    {
        Reservation_New reservation = Data.reservations.Find(r => r.ID.Equals(ID));
        if (reservation != null)
        {
            reservation.Deleted = true;
            WriteReservationData();
        }
    }

    public static void DeleteReservationsForRoom(string roomID)
    {
        List<Reservation_New> reservations = Data.reservations.FindAll(r => r.RoomID.Equals(roomID));
        foreach (var reservation in reservations)
        {
            reservation.Deleted = true;
        }
        WriteReservationData();
    }

    public static void DeleteReservationsForProperty(string propertyID)
    {
        List<Reservation_New> reservations = Data.reservations.FindAll(r => r.PropertyID.Equals(propertyID));
        foreach (var reservation in reservations)
        {
            reservation.Deleted = true;
        }
        WriteReservationData();
    }

    public static void DeleteReservationsForCustomer(string customerID)
    {
        List<Reservation_New> reservations = Data.reservations.FindAll(r => r.CustomerID.Equals(customerID));
        foreach (var reservation in reservations)
        {
            reservation.Deleted = true;
        }
        WriteReservationData();
    }

    [Serializable]
    private class ReservationData_New
    {
        public List<Reservation_New> reservations;

        public ReservationData_New() => this.reservations = new List<Reservation_New>();
    }

    [Serializable]
    private class Reservation_New : IReservation_New
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
        public string PropertyID
        {
            get => roomID;
            set
            {
                roomID = value;
                WriteReservationData();
            }
        }

        [SerializeField]
        private string roomID;
        public string RoomID
        {
            get => roomID;
            set
            {
                roomID = value;
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

        public Reservation_New(IRoom room)
        {
            this.id = Guid.NewGuid().ToString();
            this.propertyID = room.PropertyID;
            this.roomID = room.ID;
            this.period = new DateTimePeriod(DateTime.Today, DateTime.Today.AddDays(1f));
            WriteReservationData();
        }

        public Reservation_New(IRoom room, string customerID, DateTime start, DateTime end)
        {
            this.id = Guid.NewGuid().ToString();
            this.propertyID = room.PropertyID;
            this.roomID = room.ID;
            this.customerID = customerID;
            this.period = new DateTimePeriod(start, end);
            WriteReservationData();
        }

        public void EditReservation(IRoom room, string _customerID, DateTime start, DateTime end)
        {
            id = Guid.NewGuid().ToString();
            propertyID = room.PropertyID;
            roomID = room.ID;
            customerID = _customerID;
            period = new DateTimePeriod(start, end);
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
    }
}
