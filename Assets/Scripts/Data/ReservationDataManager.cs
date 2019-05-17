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
        string dataAsJson = JsonUtility.ToJson(Data);

        string filePath = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    // we're using interfaces to restrict data mutation to methods and properties that we control
    // this allows us to make sure that the data file is updated automatically with each change
    public static IEnumerable<IReservation> GetReservations()
    {
        return Data.reservations.FindAll(r => !r.Deleted);
    }

    public static IEnumerable<IReservation> GetReservations(Predicate<IReservation> match)
    {
        return Data.reservations.FindAll(match);
    }

    public static IEnumerable<IReservation> GetDeletedReservations()
    {
        return Data.reservations.FindAll(r => r.Deleted);
    }

    public static IReservation GetReservation(string ID)
    {
        return Data.reservations.Find(r => r.ID.Equals(ID));
    }

    public static IReservation AddReservation(IRoom room)
    {
        Reservation newReservation = new Reservation(room);
        Data.reservations.Add(newReservation);
        WriteReservationData();

        return newReservation;
    }

    public static IReservation AddReservation(IRoom room, string customerID, DateTime start, DateTime end)
    {
        Reservation newReservation = new Reservation(room,customerID,start,end);
        Data.reservations.Add(newReservation);
        WriteReservationData();

        return newReservation;
    }

    public static void EditReservation(IReservation reservation, IRoom room, string customerID, DateTime start, DateTime end)
    {
        reservation.EditReservation(room, customerID, start, end);
        WriteReservationData();
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
