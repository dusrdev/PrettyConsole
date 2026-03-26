using System.Runtime.InteropServices;

namespace PrettyConsole;

/// <summary>
/// Owns a transient console region on a single output pipe and coordinates it with durable writes.
/// </summary>
public sealed class TransientConsoleRegion : IDisposable {
    private const int InitialSnapshotCapacity = 256;

    private readonly Lock _lock = new();
    private bool _disposed;
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
        ThrowIfDisposed();

        lock (_lock) {
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
        ThrowIfDisposed();

        lock (_lock) {
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
        ThrowIfDisposed();

        lock (_lock) {

            if (_isVisible) {
                ClearVisibleOnly();
            }

            handler.ResetColors();
            var written = handler.WrittenSpan;
            if (written.Length == 0) {
                _snapshot.Clear();
                OccupiedLines = 0;
                handler.FlushWithoutWrite();
                return;
            }

            var lengthDelta = _snapshot.Count - written.Length;
            _snapshot.EnsureCapacity(written.Length);
            if (lengthDelta > 0) CollectionsMarshal.AsSpan(_snapshot).Slice(written.Length, lengthDelta).Clear();
            CollectionsMarshal.SetCount(_snapshot, written.Length);
            written.CopyTo(CollectionsMarshal.AsSpan(_snapshot));
            OccupiedLines = CountLines(CollectionsMarshal.AsSpan(_snapshot));
            handler.FlushWithoutWrite();
            WriteSnapshot();
        }
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
        Clear();
        _disposed = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CountLines(ReadOnlySpan<char> buffer) => buffer.Count(Environment.NewLine) + 1;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
