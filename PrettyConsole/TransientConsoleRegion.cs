using System.Runtime.InteropServices;

namespace PrettyConsole;

/// <summary>
/// Owns a transient console region on a single output pipe and coordinates it with durable writes.
/// </summary>
public sealed class TransientConsoleRegion : IDisposable {
    private const int InitialSnapshotCapacity = 256;

    private readonly Lock _lock = new();
    private volatile bool _disposed;
    private bool _isVisible;
    private readonly List<char> _snapshot = new(InitialSnapshotCapacity);

    /// <summary>
    /// The output pipe used for both transient and durable output.
    /// </summary>
    public OutputPipe Pipe { get; }

    /// <summary>
    /// Whether this region currently has retained transient content.
    /// </summary>
    public bool IsActive => _snapshot.Count > 0;

    /// <summary>
    /// The number of lines occupied by the retained transient content.
    /// </summary>
    public int OccupiedLines { get; private set; }

    /// <summary>
    /// Creates a new region bound to <paramref name="pipe"/>.
    /// </summary>
    public TransientConsoleRegion(OutputPipe pipe = OutputPipe.Error) {
        Pipe = pipe;
    }

    /// <summary>
    /// Writes durable output to the region pipe.
    /// </summary>
    public void Write([InterpolatedStringHandlerArgument("")] ref PrettyConsoleInterpolatedStringHandler handler) {
        lock (_lock) {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_isVisible) {
                ClearVisibleOnly();
            }

            handler.Flush();
        }
    }

    /// <summary>
    /// Writes durable output followed by a newline and restores the transient region afterwards.
    /// </summary>
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
    /// Replaces the transient region contents with the rendered handler output.
    /// </summary>
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
    /// Renders a progress bar snapshot into this region, optionally prefixed with handler-built content.
    /// </summary>
    public void RenderProgress(
        double percentage,
        PrettyConsoleInterpolatedStringHandlerFactory? factory = null,
        bool sameLine = true,
        ConsoleColor? progressColor = null,
        char progressChar = ProgressBar.DefaultProgressChar,
        int? maxLineWidth = null) {
        var handler = new PrettyConsoleInterpolatedStringHandler(Pipe);
        int cursorLeft = 0;

        if (factory is not null) {
            factory(PrettyConsoleInterpolatedStringHandlerBuilder.Singleton, out var headerHandler);
            handler.AppendInline(Pipe, ref headerHandler);

            if (sameLine) {
                handler.AppendFormatted(new WhiteSpace(1));
                cursorLeft = handler.CharsWritten;
            } else {
                handler.AppendNewLine();
            }
        }

        ProgressBar.AppendTo(ref handler, (int)percentage, progressColor ?? ConsoleColor.DefaultForeground, cursorLeft, progressChar, maxLineWidth);
        Render(ref handler);
    }

    /// <summary>
    /// Clears the transient region and forgets any retained snapshot.
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

    /// <inheritdoc />
    public void Dispose() {
        lock (_lock) {
            if (_disposed) {
                return;
            }

            if (_isVisible) {
                ClearVisibleOnly();
            }

            _snapshot.Clear();
            OccupiedLines = 0;
            _disposed = true;
        }
    }

    private void ClearVisibleOnly() {
        Console.ClearNextLines(OccupiedLines, Pipe);
        _isVisible = false;
    }

    private void WriteSnapshot() {
        if (_snapshot.Count == 0) {
            return;
        }

        int currentLine = Console.GetCurrentLine();
        ConsoleContext.GetPipeTarget(Pipe).Write(CollectionsMarshal.AsSpan(_snapshot));
        Console.GoToLine(currentLine);
        _isVisible = true;
    }
}
