using System;

[Serializable]
public class BackupData
{
    public long ticks;
    public DateTime CreationDate
    {
        get => new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
        set => ticks = value.ToUniversalTime().Ticks;
    }

    public string propertyData;
    public string reservationData;
}
