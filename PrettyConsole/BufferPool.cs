using System.Diagnostics;
using System.Threading.Channels;

namespace PrettyConsole;

internal sealed class BufferPool : IDisposable {
    private bool _disposed;
    private readonly Channel<List<char>> _channel;
    private readonly ThreadLocal<List<char>?> _fastItem;

    internal const int ListStartingSize = 256;
    internal const int ListMaxSize = 4096;

    public static readonly BufferPool Shared = new();

    private BufferPool() {
        _channel = Channel.CreateBounded<List<char>>(new BoundedChannelOptions(Environment.ProcessorCount * 2) {
            SingleWriter = false,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.DropWrite
        });
        _fastItem = new(() => null, trackAllValues: false);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public PooledObjectOwner Rent(out List<char> value) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var item = _fastItem.Value;
        if (item is not null) {
            _fastItem.Value = null;
            value = item;
            return new(this, value);
        }
        if (_channel.Reader.TryRead(out item)) {
            value = item;
            return new(this, value);
        }
        value = new List<char>(ListStartingSize);
        return new(this, value);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Return(List<char> item) {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!AcceptAndClear(item)) {
            return;
        }
        if (_fastItem.Value is null) {
            _fastItem.Value = item;
            return;
        }
        _channel.Writer.TryWrite(item);
    }

    /// <summary>
	/// Checks if <paramref name="item"/> should be accepted back to the pool, and clears it if it should.
	/// </summary>
	/// <param name="item"></param>
	/// <returns></returns>
    private static bool AcceptAndClear(List<char> item) {
        if (item.Count > ListMaxSize) {
            return false;
        }
        item.Clear();
        return true;
    }

    public void Dispose() {
        if (_disposed) {
            return;
        }

        while (_channel.Reader.TryRead(out var it)) {
            (it as IDisposable)?.Dispose();
        }
        _fastItem?.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }

    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    internal struct PooledObjectOwner : IDisposable {
        private BufferPool? _pool;
        private List<char>? _value;
        public readonly List<char> Value
        => _value ?? throw new InvalidOperationException("The buffer was already returned to the pool.");

        internal PooledObjectOwner(BufferPool pool, List<char> value) {
            _pool = pool;
            _value = value;
        }

        public void Dispose() {
            var pool = _pool;
            var value = _value;
            if (pool is null || value is null) return;

            _pool = null;
            _value = null;

            pool.Return(value);
        }

        private readonly string GetDebuggerDisplay() {
            const string type = "List<char>";
            return _pool is not null
                ? $"Owner<{type}>"
                : $"Owner<{type}> [returned]";
        }
    }
}