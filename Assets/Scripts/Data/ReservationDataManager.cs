using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ReservationDataManager
{
    private const string FILE_NAME = "reservationData.json";

    private static ReservationData cache;

    private static ReservationData GetReservationData()
    {
        if (cache == null)
        {
            string filePath = Path.Combine(Application.dataPath, FILE_NAME);
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

    private static void WriteReservationData()
    {
        ReservationData data = GetReservationData();
        string dataAsJson = JsonUtility.ToJson(data);

        string filePath = Path.Combine(Application.dataPath, FILE_NAME);
        File.WriteAllText(filePath, dataAsJson);
    }

    // we're using interfaces to restrict data mutation to methods and properties that we control
    // this allows us to make sure that the data file is updated automatically with each change
    public static IEnumerable<IReservation> GetReservations()
    {
        ReservationData data = GetReservationData();
        return data.reservations.FindAll(r => !r.deleted);
    }

    public static IEnumerable<IReservation> GetDeletedReservations()
    {
        ReservationData data = GetReservationData();
        return data.reservations.FindAll(r => r.deleted);
    }

    public static IReservation GetReservation(string ID)
    {
        ReservationData data = GetReservationData();
        return data.reservations.Find(r => r.ID.Equals(ID));
    }

    public static IReservation AddReservation(IRoom room)
    {
        Reservation newReservation = new Reservation(room);
        GetReservationData().reservations.Add(newReservation);
        WriteReservationData();

        return newReservation;
    }

    public static void DeleteReservation(string ID)
    {
        ReservationData data = GetReservationData();
        Reservation reservation = data.reservations.Find(r => r.id.Equals(ID));
        if (reservation != null)
        {
            reservation.deleted = true;
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
        public string id;
        public string ID => id;

        public bool deleted = false;

        public string propertyID;
        public string PropertyID => propertyID;

        public string roomID;
        public string RoomID => roomID;

        public string customerName;
        public string CustomerName
        {
            get => customerName;
            set
            {
                customerName = value;
                WriteReservationData();
            }
        }

        public DateTimePeriod period;
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
        public long startTicks;
        public DateTime Start
        {
            get => new DateTime(startTicks, DateTimeKind.Local);
            set
            {
                startTicks = value.Ticks;
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
            get => new DateTime(endTicks, DateTimeKind.Local);
            set
            {
                endTicks = value.Ticks;
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
    }
}
