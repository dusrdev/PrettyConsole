using PrettyConsole.Models;

using ogConsole = System.Console;

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