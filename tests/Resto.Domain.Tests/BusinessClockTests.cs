using System.Globalization;
using Resto.Domain.Platform;
using Xunit;

namespace Resto.Domain.Tests;

public sealed class BusinessClockTests
{
    [Theory]
    [InlineData("America/Tijuana", 4, "2026-01-02T08:00:00Z", "2026-01-01")]
    [InlineData("America/Tijuana", 4, "2026-01-02T09:30:00Z", "2026-01-01")]
    [InlineData("America/Tijuana", 4, "2026-01-02T11:59:59.9999999Z", "2026-01-01")]
    [InlineData("America/Tijuana", 4, "2026-01-02T12:00:00Z", "2026-01-02")]
    [InlineData("America/Tijuana", 6, "2026-01-02T12:00:00Z", "2026-01-01")]
    [InlineData("America/Tijuana", 0, "2026-01-02T08:00:00Z", "2026-01-02")]
    [InlineData("Asia/Tokyo", 4, "2026-01-02T12:00:00Z", "2026-01-02")]
    [InlineData("Asia/Tokyo", 4, "2026-01-01T19:00:00Z", "2026-01-02")]
    [InlineData("America/Tijuana", 4, "2026-07-02T11:00:00Z", "2026-07-02")]
    [InlineData("America/Tijuana", 4, "2026-07-02T04:00:00-07:00", "2026-07-02")]
    [InlineData("America/Tijuana", 2, "2026-03-08T09:59:59Z", "2026-03-07")]
    [InlineData("America/Tijuana", 2, "2026-03-08T10:00:00Z", "2026-03-08")]
    public void BR_PLT_11_AC_PLT_15_UsesRestaurantZoneAndCutoff(
        string zone, int cutoffHour, string instant, string expected)
    {
        var clock = new BusinessClock(TimeProvider.System, TimeZoneInfo.FindSystemTimeZoneById(zone), new TimeOnly(cutoffHour, 0));

        Assert.Equal(DateOnly.Parse(expected, CultureInfo.InvariantCulture), clock.BusinessDay(Parse(instant)));
    }

    [Theory]
    [InlineData("2026-11-01T08:29:59Z", "2026-10-31")]
    [InlineData("2026-11-01T08:30:00Z", "2026-11-01")]
    [InlineData("2026-11-01T09:00:00Z", "2026-11-01")]
    [InlineData("2026-11-01T09:30:00Z", "2026-11-01")]
    public void BR_PLT_11_RepeatedCutoffStartsTheBusinessDayOnce(string instant, string expected)
    {
        var clock = new BusinessClock(TimeProvider.System,
            TimeZoneInfo.FindSystemTimeZoneById("America/Tijuana"), new TimeOnly(1, 30));

        Assert.Equal(DateOnly.Parse(expected, CultureInfo.InvariantCulture), clock.BusinessDay(Parse(instant)));
    }

    [Fact]
    public void BR_PLT_11_ClockReadsUtcFromTheInjectedProvider()
    {
        var provider = new FixedTimeProvider(Parse("2026-01-02T09:30:00Z"));
        var clock = new BusinessClock(provider, TimeZoneInfo.FindSystemTimeZoneById("America/Tijuana"), new TimeOnly(4, 0));

        Assert.Equal(provider.GetUtcNow(), clock.UtcNow);
        Assert.Equal(new DateOnly(2026, 1, 1), clock.BusinessDay(clock.UtcNow));
    }

    private static DateTimeOffset Parse(string instant) => DateTimeOffset.Parse(instant, CultureInfo.InvariantCulture);

    private sealed class FixedTimeProvider(DateTimeOffset instant) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => instant;
    }
}
