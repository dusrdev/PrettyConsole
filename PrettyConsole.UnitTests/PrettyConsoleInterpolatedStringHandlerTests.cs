using System.Globalization;

namespace PrettyConsole.UnitTests;

public class PrettyConsoleInterpolatedStringHandlerTests {
    private readonly StringWriter _writer;

    public PrettyConsoleInterpolatedStringHandlerTests() {
        Out = Utilities.GetWriter(out _writer);
    }

    [Test]
    public async Task ReadOnlySpanAlignment_PadsCorrectly() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            ReadOnlySpan<char> span = "Hi".AsSpan();
            Console.WriteInterpolated($"{span,5}");

            await Assert.That(writer.ToString()).IsEqualTo("   Hi");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task GenericFormatsAndAlignment_WorkForISpanFormattable() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            decimal amount = 12.34m;
            Console.WriteInterpolated($"{amount:F1} {(decimal)5.5,6:F2}");

            await Assert.That(writer.ToString()).IsEqualTo("12.3   5.50");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ColorTokens_CountWhenNotRedirected() {
        var originalOut = Out;
        try {
            Out = Console.Out;

            int chars = Console.WriteInterpolated($"{ConsoleColor.Red}X{ConsoleColor.Default}");

            await Assert.That(chars).IsEqualTo(1);
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task GrowAndEnsureCapacity_HandleLargePayloads() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);
            var longText = new string('x', 5000);

            Console.WriteInterpolated($"{longText}");

            await Assert.That(writer.ToString().Length).IsEqualTo(5000);
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ThrowsAfterFlush_WhenAppendingFurther() {
        var handler = new PrettyConsoleInterpolatedStringHandler(0, 0, OutputPipe.Out, provider: null, out var shouldAppend);
        await Assert.That(shouldAppend).IsTrue();

        handler.AppendLiteral("ok");
        handler.Flush();

        await Assert.That(() => handler.AppendLiteral("fail")).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AlignmentWithPaddingRight_AddsSpaces() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteInterpolated($"{123,-5}");

            await Assert.That(writer.ToString()).IsEqualTo("123  ");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    [Arguments(0, 0, 0, "0h 0m 0s")]
    [Arguments(0, 1, 2, "0h 1m 2s")]
    [Arguments(5, 59, 59, "5h 59m 59s")]
    [Arguments(48, 30, 5, "48h 30m 5s")]
    [Arguments(1234, 0, 1, "1234h 0m 1s")]
    [Arguments(256204778, 48, 5, "256204778h 48m 5s")]
    public async Task AppendFormatted_TimeSpanDuration_WritesExpected(int hours, int minutes, int seconds, string expected) {
        var timeSpan = TimeSpan.FromHours(hours)
            .Add(TimeSpan.FromMinutes(minutes))
            .Add(TimeSpan.FromSeconds(seconds));

        Console.WriteInterpolated($"Elapsed {timeSpan:duration}");

        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo($"Elapsed {expected}");
    }

    [Test]
    [Arguments(0d)]
    [Arguments(512d)]
    [Arguments(1024d)]
    [Arguments(15360d)]
    [Arguments(42_949_672_960d)]
    [Arguments(1.1258999068426228e105)]
    [Arguments(1.1258999068426251e105)]
    [Arguments(double.MaxValue)]
    public async Task AppendFormatted_DoubleBytes_WritesExpected(double value) {
        Console.WriteInterpolated($"Size {value:bytes}");

        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo($"Size {FormatBytes(value)}");
    }

    [Test]
    public async Task CharsWritten_IgnoresAnsiColorAndMarkupSequences() {
        int chars = Console.WriteInterpolated($"{ConsoleColor.Red}{Markup.Bold}Hi{Markup.Reset}");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        await Assert.That(written).IsEqualTo("Hi");
        await Assert.That(chars).IsEqualTo(2);
    }

    [Test]
    [Arguments(5, "   OK")]
    [Arguments(-5, "OK   ")]
    public async Task Alignment_UsesVisibleLengthWhenMarkupPresent(int alignment, string expected) {
        int chars = alignment > 0
            ? Console.WriteInterpolated($"{Markup.Bold}{"OK",5}{Markup.Reset}")
            : Console.WriteInterpolated($"{Markup.Bold}{"OK",-5}{Markup.Reset}");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        await Assert.That(written).IsEqualTo(expected);
        await Assert.That(chars).IsEqualTo(expected.Length);
    }

    [Test]
    public async Task WriteLineInterpolated_ReturnsCharsWithoutNewline() {
        int chars = Console.WriteLineInterpolated($"Hi");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        await Assert.That(written).IsEqualTo("Hi" + Environment.NewLine);
        await Assert.That(chars).IsEqualTo(2);
    }

    [Test]
    public async Task WriteInterpolated_MixedPrimitivesAndFormats_ReturnsVisibleCount() {
        var duration = TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(7);

        int chars = Console.WriteInterpolated($"Id:{123} Ok:{true} Pi:{3.14159:F2} Char:{'X'} Elapsed:{duration:duration}");

        var expected = $"Id:123 Ok:True Pi:3.14 Char:X Elapsed:0h 5m 7s";
        await Assert.That(_writer.ToStringAndFlush()).IsEqualTo(expected);
        await Assert.That(chars).IsEqualTo(expected.Length);
    }

    [Test]
    public async Task WriteInterpolated_ColorTuple_DoesNotAffectVisibleCount() {
        int chars = Console.WriteInterpolated($"{(ConsoleColor.Red, ConsoleColor.White)}ERR{ConsoleColor.Default} done");

        var written = Utilities.StripAnsiSequences(_writer.ToStringAndFlush());

        await Assert.That(written).IsEqualTo("ERR done");
        await Assert.That(chars).IsEqualTo("ERR done".Length);
    }

    [Test]
    public async Task WriteLineInterpolated_WithColorsAndPrimitives_CountExcludesNewline() {
        var duration = TimeSpan.FromSeconds(42);
        var writer = new StringWriter();
        Error = writer;

        int chars = Console.WriteLineInterpolated(OutputPipe.Error, $"{ConsoleColor.Yellow}[{duration:duration}] {Markup.Bold}done{Markup.Reset}");

        var stripped = Utilities.StripAnsiSequences(writer.ToString());
        var expected = $"[0h 0m 42s] done{Environment.NewLine}";

        await Assert.That(stripped).IsEqualTo(expected);
        await Assert.That(chars).IsEqualTo("[0h 0m 42s] done".Length);
    }

    [Test]
    public async Task AppendFormattedBackground_ChangesBackgroundWhenNotRedirected() {
        var originalOut = Out;
        try {
            Out = Console.Out;
            int chars = Console.WriteInterpolated($"{ConsoleColor.Red}{ConsoleColor.Black / ConsoleColor.White}X");
            await Assert.That(chars).IsEqualTo(1);
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task AppendFormattedObject_WithAlignment_UsesObjectToString() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);
            var obj = new object();

            Console.WriteInterpolated($"{obj,6}");

            await Assert.That(writer.ToString()).IsEqualTo($"{obj,6}");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task AppendFormattedTimeSpan_WithAlignmentAndDurationFormat() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);
            var ts = TimeSpan.FromSeconds(5);

            Console.WriteInterpolated($"{ts,10:duration}");

            await Assert.That(writer.ToString()).IsEqualTo("  0h 0m 5s");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task AppendFormattedDouble_WithAlignmentAndFormat() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteInterpolated($"{12.345,8:F2}");

            await Assert.That(writer.ToString()).IsEqualTo("   12.35");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task AppendSpanFormattable_WithAlignmentAndFormat() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteInterpolated($"{1234,6:D}");

            await Assert.That(writer.ToString()).IsEqualTo("  1234");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task AppendSpan_LeftAndRightAlignment() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteInterpolated($"{"Hi",4}{"Bye",-5}");

            await Assert.That(writer.ToString()).IsEqualTo("  HiBye  ");
        } finally {
            Out = originalOut;
        }
    }

    private static string FormatBytes(double value) {
        const double formatBytesKb = 1024d;
        var suffix = 0;
        var num = value;

        while (suffix < FileSizeSuffix.Length - 1 && num >= formatBytesKb) {
            num /= formatBytesKb;
            suffix++;
        }

        return string.Format(CultureInfo.CurrentCulture, "{0:#,##0.##} {1}", num, FileSizeSuffix[suffix]);
    }

    private static ReadOnlySpan<string> FileSizeSuffix => new[] { "B", "KB", "MB", "GB", "TB", "PB" };

    [Test]
    public async Task AppendWhiteSpace() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteInterpolated($"{new WhiteSpace(5)}");

            await Assert.That(writer.ToString()).IsEqualTo(new string(' ', 5));
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ManualCtor() {
        (_, var isRedirected) = GetPipeTargetAndState(OutputPipe.Out);

        var handler = new PrettyConsoleInterpolatedStringHandler(OutputPipe.Out);
        handler.AppendFormatted(Green);
        handler.AppendSpan("Hello");
        handler.ResetColors();

        if (isRedirected) {
            await Assert.That(new string(handler.WrittenSpan)).IsEqualTo("Hello");
        } else {
            await Assert.That(new string(handler.WrittenSpan)).IsEqualTo($"{AnsiColors.Foreground(Green)}Hello{AnsiColors.ForegroundResetSequence}");
        }

        handler.FlushWithoutWrite();
    }

    [Test]
    public async Task NestedHandler() {
        (_, var isRedirected) = GetPipeTargetAndState(OutputPipe.Out);

        var handler = new PrettyConsoleInterpolatedStringHandler(OutputPipe.Out);
        handler.AppendHandlerContent(OutputPipe.Out, $"{Green}Hello");

        if (isRedirected) {
            await Assert.That(new string(handler.WrittenSpan)).IsEqualTo("Hello");
        } else {
            await Assert.That(new string(handler.WrittenSpan)).IsEqualTo($"{AnsiColors.Foreground(Green)}Hello{AnsiColors.ForegroundResetSequence}");
        }

        handler.FlushWithoutWrite();
    }
}