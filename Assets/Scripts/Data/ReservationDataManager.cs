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

    public static void DeleteReservation(string ID)
    {
        Reservation reservation = Data.reservations.Find(r => r.ID.Equals(ID));
        if (reservation != null)
        {
            reservation.Deleted = true;
            WriteReservationData();
        }
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

        [SerializeField]
        private string customerName;
        public string CustomerName
        {
            get => customerName;
            set
            {
                customerName = value;
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
    }
}
