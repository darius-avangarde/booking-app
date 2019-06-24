using System;

public interface IDateTimePeriod
{
    DateTime Start { get; set; }
    DateTime End { get; set; }
    bool Includes(DateTime dateTime);
    bool Overlaps(DateTime start, DateTime end);
    bool Overlaps(IDateTimePeriod period);
}
