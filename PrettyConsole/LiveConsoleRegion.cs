using System.Runtime.InteropServices;

namespace PrettyConsole;

/// <summary>
/// Manages a retained transient console region on a single output pipe while coordinating durable writes above it.
/// Disposing the region clears the retained snapshot and permanently closes the instance.
/// </summary>
public sealed class LiveConsoleRegion : IDisposable {
    private const int InitialSnapshotCapacity = 256;

    private readonly Lock _lock = new();
    private volatile bool _disposed;
    private bool _isVisible;
    private readonly List<char> _snapshot = new(InitialSnapshotCapacity);

    private readonly OutputPipe _pipe;

    /// <summary>
    /// The output pipe used for both transient and durable output.
    /// </summary>
    public OutputPipe Pipe => _pipe;

    /// <summary>
    /// Whether this region currently has retained transient content.
    /// </summary>
    public bool IsActive => _snapshot.Count != 0;

    /// <summary>
    /// The number of lines occupied by the retained transient content.
    /// </summary>
    public int OccupiedLines { get; private set; }

    /// <summary>
    /// Creates a new live region bound to the specified output pipe.
    /// </summary>
    /// <param name="pipe">The output pipe used for both retained transient content and durable writes.</param>
    public LiveConsoleRegion(OutputPipe pipe = OutputPipe.Error) {
        _pipe = pipe;
    }

    /// <summary>
    /// Writes a durable line above the retained region and then restores the region beneath it.
    /// </summary>
    /// <param name="handler">The interpolated content to write as a durable line.</param>
    public void WriteLine([InterpolatedStringHandlerArgument("")] ref PrettyConsoleInterpolatedStringHandler handler) {
        lock (_lock) {
            ObjectDisposedException.ThrowIf(_disposed, this);
            bool shouldRestore = _snapshot.Count != 0;

            if (_isVisible) {
                ClearVisibleOnly();
            }

            handler.AppendNewLine();
            handler.Flush();

            if (shouldRestore) {
                WriteSnapshot();
            }
        }
    }

    /// <summary>
    /// Replaces the current retained region contents with the rendered handler output.
    /// </summary>
    /// <param name="handler">The interpolated content to retain as the region snapshot.</param>
    public void Render([InterpolatedStringHandlerArgument("")] ref PrettyConsoleInterpolatedStringHandler handler) {
        lock (_lock) {
            ObjectDisposedException.ThrowIf(_disposed, this);
            handler.ResetColors();
            var written = handler.WrittenSpan;

            if (_isVisible && CollectionsMarshal.AsSpan(_snapshot).SequenceEqual(written)) {
                handler.FlushWithoutWrite();
                return;
            }

            if (_isVisible) {
                ClearVisibleOnly();
            }

            if (written.Length == 0) {
                _snapshot.Clear();
                OccupiedLines = 0;
                handler.FlushWithoutWrite();
                return;
            }

            _snapshot.EnsureCapacity(written.Length);
            CollectionsMarshal.SetCount(_snapshot, written.Length);
            var snapshot = CollectionsMarshal.AsSpan(_snapshot);
            written.CopyTo(snapshot);
            OccupiedLines = snapshot.Count(Environment.NewLine) + 1;
            handler.FlushWithoutWrite();
            WriteSnapshot();
        }
    }

    /// <summary>
    /// Renders a retained progress-bar snapshot into the live region, optionally prefixed with header content.
    /// </summary>
    /// <param name="percentage">The progress percentage to render.</param>
    /// <param name="factory">Optional header factory that renders content before the progress bar.</param>
    /// <param name="sameLine">Whether the optional header should share the same line as the progress bar.</param>
    /// <param name="progressColor">Optional color token for the filled progress segment.</param>
    /// <param name="progressChar">The character used for the filled portion of the progress bar.</param>
    /// <param name="maxLineWidth">Optional total width constraint for the rendered progress line.</param>
    public void RenderProgress(
        double percentage,
        PrettyConsoleInterpolatedStringHandlerFactory? factory = null,
        bool sameLine = true,
        AnsiToken? progressColor = null,
        char progressChar = ProgressBar.DefaultProgressChar,
        int? maxLineWidth = null) {
        var handler = new PrettyConsoleInterpolatedStringHandler(_pipe);
        int cursorLeft = 0;

        if (factory is not null) {
            factory(PrettyConsoleInterpolatedStringHandlerBuilder.Singleton, out var headerHandler);
            handler.AppendInline(_pipe, ref headerHandler);

            if (sameLine) {
                handler.AppendFormatted(new WhiteSpace(1));
                cursorLeft = handler.CharsWritten;
            } else {
                handler.AppendNewLine();
            }
        }

        ProgressBar.AppendTo(ref handler, (int)percentage, progressColor ?? Color.DefaultForeground, cursorLeft, progressChar, maxLineWidth);
        Render(ref handler);
    }

    /// <summary>
    /// Clears the retained region from the console and discards the current snapshot without disposing the region.
    /// Call this when the pinned region should disappear before the region instance itself goes out of scope and you still intend to reuse it later.
    /// </summary>
    public void Clear() {
        lock (_lock) {
            if (_disposed) {
                return;
            }

            if (_isVisible) {
                ClearVisibleOnly();
            }

            _snapshot.Clear();
            OccupiedLines = 0;
        }
    }

    /// <summary>
    /// Clears any retained snapshot and permanently closes the region.
    /// After disposal, the region can no longer be rendered to or written through.
    /// </summary>
    public void Dispose() {
        Clear();
        _disposed = true;
    }

    private void ClearVisibleOnly() {
        Console.ClearNextLines(OccupiedLines, _pipe);
        _isVisible = false;
    }

    private void WriteSnapshot() {
        if (_snapshot.Count == 0) {
            return;
        }

        int currentLine = Console.GetCurrentLine();
        ConsoleContext.GetPipeTarget(_pipe).Write(CollectionsMarshal.AsSpan(_snapshot));
        Console.GoToLine(currentLine);
        _isVisible = true;
    }
}
