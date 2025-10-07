using System.Globalization;

namespace PrettyConsole.Tests.Unit;

public class UtilityTests {
    [Fact]
    public void FormatPercentage_PadsAndRoundsToFiveCharacters() {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        try {
            Span<char> buffer = stackalloc char[8];

            var span = Utils.FormatPercentage(3, buffer);
            Assert.Equal("    3", span.ToString());

            span = Utils.FormatPercentage(12.345, buffer);
            Assert.Equal("12.35", span.ToString());
        } finally {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void FormatPercentage_ClampsWithinBounds() {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        try {
            Span<char> buffer = stackalloc char[8];

            var span = Utils.FormatPercentage(150, buffer);
            Assert.Equal("  100", span.ToString());

            span = Utils.FormatPercentage(-10, buffer);
            Assert.Equal("    0", span.ToString());
        } finally {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void FormatTimeSpan_AdaptsToDuration() {
        Span<char> buffer = stackalloc char[32];

        var written = Utils.FormatTimeSpan(TimeSpan.FromMilliseconds(500), buffer);
        Assert.Equal("500ms", new string(buffer[..written]));

        written = Utils.FormatTimeSpan(TimeSpan.FromMilliseconds(12_345), buffer);
        Assert.Equal("12:345s", new string(buffer[..written]));

        written = Utils.FormatTimeSpan(TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(30), buffer);
        Assert.Equal("05:30m", new string(buffer[..written]));

        written = Utils.FormatTimeSpan(TimeSpan.FromHours(2) + TimeSpan.FromMinutes(15), buffer);
        Assert.Equal("02:15hr", new string(buffer[..written]));

        written = Utils.FormatTimeSpan(TimeSpan.FromDays(1) + TimeSpan.FromHours(3), buffer);
        Assert.Equal("01:03d", new string(buffer[..written]));
    }

    [Fact]
    public void WriteWhiteSpaces_WritesRequestedLength() {
        var writer = new StringWriter();

        writer.WriteWhiteSpaces(10);
        Assert.Equal(new string(' ', 10), writer.ToString());

        writer.GetStringBuilder().Clear();
        writer.WriteWhiteSpaces(300);
        Assert.Equal(300, writer.ToString().Length);
        Assert.True(writer.ToString().All(c => c == ' '));
    }

    [Fact]
    public void InColor_SetsForegroundWithDefaultBackground() {
        var colored = "hello".InColor(ConsoleColor.Cyan);

        Assert.Equal("hello", colored.Value);
        Assert.Equal(ConsoleColor.Cyan, colored.ForegroundColor);
        Assert.Equal(Color.DefaultBackgroundColor, colored.BackgroundColor);
    }

    [Fact]
    public void InColor_WithBackground_SetsBothColors() {
        var colored = "world".InColor(ConsoleColor.Green, ConsoleColor.Black);

        Assert.Equal("world", colored.Value);
        Assert.Equal(ConsoleColor.Green, colored.ForegroundColor);
        Assert.Equal(ConsoleColor.Black, colored.BackgroundColor);
    }
}
