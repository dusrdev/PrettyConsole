using PrettyConsole.Models;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Write a read-only span of characters to the console in the default color as set by <see cref="Color.Default"/>, followed by the current line terminator.
    /// </summary>
    /// <param name="buffer">The read-only span of characters to write to the console.</param>
    /// <remarks>
    /// To stay on the same line, use <see cref="Write(ReadOnlySpan{char})"/> with the same parameters.
    /// </remarks>
    public static void WriteLine(ReadOnlySpan<char> buffer) {
        WriteLine(buffer, Color.Default);
    }

    /// <summary>
    /// Write a read-only span of characters to the console in the specified color, followed by the current line terminator.
    /// </summary>
    /// <param name="buffer">The read-only span of characters to write to the console.</param>
    /// <param name="color">The color in which the output will be displayed.</param>
    /// <remarks>
    /// To stay on the same line, use <see cref="Write(ReadOnlySpan{char}, ConsoleColor)"/> with the same parameters.
    /// </remarks>
    public static void WriteLine(ReadOnlySpan<char> buffer, ConsoleColor color) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = color;
            ogConsole.Out.WriteLine(buffer);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Write a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To not end line, use <see cref="Write(ColoredOutput)"/>
    /// </remarks>
    public static void WriteLine(ColoredOutput output) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = output.ForegroundColor;
            ogConsole.BackgroundColor = output.BackgroundColor;
            ogConsole.Out.WriteLine(output.Value);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// WriteLine a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLine(ColoredOutput output1, ColoredOutput output2) {
        Write(output1);
        WriteLine(output2);
    }

    /// <summary>
    /// WriteLine a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLine(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3) {
        Write(output1);
        Write(output2);
        WriteLine(output3);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLine(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3, ColoredOutput output4) {
        Write(output1);
        Write(output2);
        Write(output3);
        WriteLine(output4);
    }

    /// <summary>
    /// WriteLine a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLine(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3, ColoredOutput output4, ColoredOutput output5) {
        Write(output1);
        Write(output2);
        Write(output3);
        Write(output4);
        WriteLine(output5);
    }

    /// <summary>
    /// WriteLine a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    /// <remarks>
    /// In overloads of WriteLine with multiple <see cref="ColoredOutput"/> parameters, only the last <see cref="ColoredOutput"/> will end the line.
    /// </remarks>
    public static void WriteLine(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3, ColoredOutput output4, ColoredOutput output5, ColoredOutput output6) {
        Write(output1);
        Write(output2);
        Write(output3);
        Write(output4);
        Write(output5);
        WriteLine(output6);
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
            return;
        }
        for (int i = 0; i < outputs.Length - 1; i++) {
            Write(outputs[i]);
        }
        WriteLine(outputs[^1]);
    }

    /// <summary>
    /// WriteLine a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To end line, use <see cref="WriteError(ColoredOutput)"/>
    /// </remarks>
    public static void WriteLineError(ColoredOutput output) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = output.ForegroundColor;
            ogConsole.BackgroundColor = output.BackgroundColor;
            ogConsole.Error.WriteLine(output.Value);
        } finally {
            ogConsole.ResetColor();
        }
    }
}