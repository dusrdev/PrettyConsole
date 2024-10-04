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
        Out.WriteLine();
    }

    /// <summary>
    /// WriteLine a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLine(params ColoredOutput[] outputs) => WriteLine(new ReadOnlySpan<ColoredOutput>(outputs));

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

        if (outputs.Length is 1) {
            WriteLine(outputs[0]);
        } else {
            for (int i = 0; i < outputs.Length - 1; i++) {
                Write(outputs[i]);
            }

            WriteLine(outputs[outputs.Length - 1]);
        }
    }

    /// <summary>
    /// WriteLine a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To end line, use <see cref="WriteError(ColoredOutput)"/>
    /// </remarks>
    public static void WriteLineError(ColoredOutput output) {
        ResetColors();
        baseConsole.ForegroundColor = output.ForegroundColor;
        baseConsole.BackgroundColor = output.BackgroundColor;
        Error.WriteLine(output.Value);
        ResetColors();
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

        if (outputs.Length is 1) {
            WriteLineError(outputs[0]);
        } else {
            for (int i = 0; i < outputs.Length - 1; i++) {
                WriteError(outputs[i]);
            }

            WriteLineError(outputs[outputs.Length - 1]);
        }
    }
}