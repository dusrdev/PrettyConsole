namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Write a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To not end line, use <see cref="Write(ColoredOutput)"/>
    /// </remarks>
    public static void WriteLine(ColoredOutput output) {
        Write(output);
        NewLine();
    }

    /// <summary>
    /// WriteLine a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLine(ReadOnlySpan<ColoredOutput> outputs) {
        if (outputs.Length is 0) {
            return;
        }

        Write(outputs);
        NewLine();
    }

    /// <summary>
    /// WriteLine a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To end line, use <see cref="WriteError(ColoredOutput)"/>
    /// </remarks>
    public static void WriteLineError(ColoredOutput output) {
        WriteError(output);
        NewLineError();
    }

    /// <summary>
    /// WriteLine a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLineError(ReadOnlySpan<ColoredOutput> outputs) {
        if (outputs.Length is 0) {
            return;
        }

        WriteError(outputs);
        NewLineError();
    }
}