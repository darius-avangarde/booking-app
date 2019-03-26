using System;

public interface IDateTimePeriod
{
    DateTime Start { get; set; }
    DateTime End { get; set; }
    bool Includes(DateTime dateTime);
}
