using Resto.Domain.Platform.Abstractions;

namespace Resto.Domain.Platform;

public sealed class BusinessClock(TimeProvider timeProvider, TimeZoneInfo timeZone, TimeOnly cutoff) : IClock
{
    public DateTimeOffset UtcNow => timeProvider.GetUtcNow();

    public DateOnly BusinessDay(DateTimeOffset utc)
    {
        var localTime = TimeZoneInfo.ConvertTime(utc, timeZone);
        var localDate = DateOnly.FromDateTime(localTime.DateTime);
        var localCutoff = localDate.ToDateTime(cutoff, DateTimeKind.Unspecified);

        // A repeated cutoff starts the day at its first occurrence.
        // A skipped cutoff takes effect at the first valid local time after the gap.
        var beforeCutoff = timeZone.IsAmbiguousTime(localCutoff)
            ? utc < new DateTimeOffset(localCutoff, timeZone.GetAmbiguousTimeOffsets(localCutoff).Max())
            : localTime.DateTime < localCutoff;

        return beforeCutoff ? localDate.AddDays(-1) : localDate;
    }
}
