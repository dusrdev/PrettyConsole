namespace PrettyConsole.UnitTests;

public class PrettyConsoleExtensionsTests {
    [Test]
    public async Task WriteWhiteSpaces_WritesRequestedLength() {
        var writer = new StringWriter();

        writer.WriteWhiteSpaces(10);
        await Assert.That(writer.ToString()).IsEqualTo(new string(' ', 10));

        writer.GetStringBuilder().Clear();
        writer.WriteWhiteSpaces(300);
        await Assert.That(writer.ToString().Length).IsEqualTo(300);
        await Assert.That(writer.ToString().All(c => c == ' ')).IsTrue();
    }

    [Test]
    public async Task Console_WriteWhiteSpaces_RoutesToCorrectPipe() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            Console.WriteWhiteSpaces(5);

            await Assert.That(writer.ToString()).IsEqualTo("     ");
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ConsoleContext_GetPipeTargetAndState_ReportsCustomOutAsRedirected() {
        var originalOut = Out;
        try {
            Out = Utilities.GetWriter(out var writer);

            var (pipeWriter, redirected) = ConsoleContext.GetPipeTargetAndState(OutputPipe.Out);

            await Assert.That(pipeWriter).IsSameReferenceAs(writer);
            await Assert.That(redirected).IsTrue();
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ConsoleContext_GetPipeTargetAndState_TracksConsoleStreams() {
        var originalOut = Out;
        try {
            Out = Console.Out;

            var (pipeWriter, redirected) = ConsoleContext.GetPipeTargetAndState(OutputPipe.Out);

            await Assert.That(pipeWriter).IsSameReferenceAs(Console.Out);
            await Assert.That(redirected).IsEqualTo(Console.IsOutputRedirected);
        } finally {
            Out = originalOut;
        }
    }

    [Test]
    public async Task ConsoleContext_GetPipeTargetAndState_ForErrorPipe() {
        var originalErr = Error;
        try {
            Error = Utilities.GetWriter(out var writer);

            var (pipeWriter, redirected) = ConsoleContext.GetPipeTargetAndState(OutputPipe.Error);

            await Assert.That(pipeWriter).IsSameReferenceAs(writer);
            await Assert.That(redirected).IsTrue();
        } finally {
            Error = originalErr;
        }
    }

    [Test]
    public async Task ConsoleContext_GetWidthOrDefault_WhenRedirected() {
        var originalOut = Console.Out;
        Console.SetOut(new StringWriter());

        int width = ConsoleContext.GetWidthOrDefault(77);
        Console.SetOut(originalOut);

        await Assert.That(width).IsEqualTo(77);
    }

    [Test]
    public async Task RenderingExtensions_DefaultCursorAccessor_IsInvoked() {
        // use default cursor accessors (no override)
        int line = Console.GetCurrentLine();
        await Assert.That(line).IsGreaterThanOrEqualTo(0);
    }
}
