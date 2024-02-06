namespace PrettyConsole;

/// <summary>
/// Used as input parameter in an overload of the ProgressBar method.
/// </summary>
public readonly record struct ProgressBarDisplay {
    /// <summary>
    /// Header text -> this is where you could write an updatable status message.
    /// </summary>
    public string Header { get; init; }

    /// <summary>
    /// The current progress value.
    /// </summary>
    /// <remarks>Recommended to round up to 2 digits</remarks>
    public double Percentage { get; init; }

    /// <summary>
    /// The character used to represent the filled progress bar.
    /// </summary>
    public char ProgressChar { get; init; }

    /// <summary>
    /// The color of the header, progress bar bounds and percentage
    /// </summary>
    public ConsoleColor Foreground { get; init; }

    /// <summary>
    /// The color of the filled progress bar
    /// </summary>
    public ConsoleColor Progress { get; init; }

    /// <summary>
    /// Constructor for the ProgressBarDisplay struct.
    /// </summary>
    /// <param name="header">Header text -> this is where you could write an updatable status message.</param>
    /// <param name="percentage">The current progress value. Recommended to round up to 2 digits.</param>
    /// <param name="progressChar">The character used to represent the filled progress bar.</param>
    /// <param name="foreground">The color of the header, progress bar bounds and percentage.</param>
    /// <param name="progress">The color of the filled progress bar.</param>
    public ProgressBarDisplay(string header, double percentage, char progressChar, ConsoleColor foreground, ConsoleColor progress) {
        Header = header;
        Percentage = percentage;
        ProgressChar = progressChar;
        Foreground = foreground;
        Progress = progress;
        if (percentage is < 0D or > 100D) {
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100");
        }
        if (progressChar is ' ') {
            throw new ArgumentException("Progress char cannot be a space", nameof(progressChar));
        }
    }

    /// <summary>
    /// Constructor for the ProgressBarDisplay struct.
    /// </summary>
    /// <param name="percentage">The current progress value. Recommended to round up to 2 digits.</param>
    /// <param name="foreground">The color of the header, progress bar bounds and percentage.</param>
    /// <param name="progress">The color of the filled progress bar.</param>
    public ProgressBarDisplay(double percentage, ConsoleColor foreground, ConsoleColor progress) : this("", percentage, '■', foreground, progress) { }

    /// <summary>
    /// Default constructor for the ProgressBarDisplay struct. Do not use as it will throw an exception to enforce proper initialization.
    /// </summary>
    public ProgressBarDisplay() : this("", -1D, '■', default, default) { }
}
