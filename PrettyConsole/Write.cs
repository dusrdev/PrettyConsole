using PrettyConsole.Models;

using ogConsole = System.Console;

namespace PrettyConsole;

public static partial class Console {
    /// <summary>
    /// Write a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To end line, use <see cref="WriteLine(ColoredOutput)"/>
    /// </remarks>
    public static void Write(ColoredOutput output) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = output.ForegroundColor;
            ogConsole.BackgroundColor = output.BackgroundColor;
            ogConsole.Out.Write(output.Value);
        } finally {
            ogConsole.ResetColor();
        }
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2) {
        Write(output1);
        Write(output2);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3) {
        Write(output1);
        Write(output2);
        Write(output3);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3, ColoredOutput output4) {
        Write(output1);
        Write(output2);
        Write(output3);
        Write(output4);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3, ColoredOutput output4, ColoredOutput output5) {
        Write(output1);
        Write(output2);
        Write(output3);
        Write(output4);
        Write(output5);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ColoredOutput output1, ColoredOutput output2, ColoredOutput output3, ColoredOutput output4, ColoredOutput output5, ColoredOutput output6) {
        Write(output1);
        Write(output2);
        Write(output3);
        Write(output4);
        Write(output5);
        Write(output6);
    }

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(params ColoredOutput[] outputs) => Write(new ReadOnlySpan<ColoredOutput>(outputs));

    /// <summary>
    /// Write a number of <see cref="ColoredOutput"/> to the console
    /// </summary>
    public static void Write(ReadOnlySpan<ColoredOutput> outputs) {
        foreach (var output in outputs) {
            Write(output);
        }
    }

    /// <summary>
    /// Write a <see cref="ColoredOutput"/> to the error console
    /// </summary>
    /// <param name="output"/>
    /// <remarks>
    /// To end line, use <see cref="WriteLineError(ColoredOutput)"/>
    /// </remarks>
    public static void WriteError(ColoredOutput output) {
        try {
            ogConsole.ResetColor();
            ogConsole.ForegroundColor = output.ForegroundColor;
            ogConsole.BackgroundColor = output.BackgroundColor;
            ogConsole.Error.Write(output.Value);
        } finally {
            ogConsole.ResetColor();
        }
    }
}