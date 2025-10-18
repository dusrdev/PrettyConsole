using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace PrettyConsole;

internal sealed class BufferPool {
    internal static readonly BufferPool Shared = new();

    private readonly Channel<List<char>> _channel;
    private List<char>? _fastItem;

    public const int ListStartingSize = 256;

    private BufferPool() {
        _channel = Channel.CreateBounded<List<char>>
                (new BoundedChannelOptions(Environment.ProcessorCount * 2) {
                    SingleWriter = false,
                    SingleReader = false,
                    FullMode = BoundedChannelFullMode.DropWrite
                });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public RentedBufferOwner Rent() {
        var buffer = _fastItem;
        if (buffer is null || Interlocked.CompareExchange(ref _fastItem, null, buffer) != buffer) {
            if (_channel.Reader.TryRead(out buffer)) {
                return new RentedBufferOwner(this, buffer);
            }

            return new RentedBufferOwner(this, new(ListStartingSize));
        }
        return new RentedBufferOwner(this, buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void Return(List<char> buffer) {
        if (buffer.Count > 4096) {
            return;
        }
        if (_fastItem is not null || Interlocked.CompareExchange(ref _fastItem, buffer, null) != null) {
            if (_channel.Writer.TryWrite(buffer)) {
                buffer.Clear();
			}
        }
    }

    internal readonly struct RentedBufferOwner : IDisposable {
        private readonly BufferPool _pool;
        public readonly List<char> Buffer;

        public RentedBufferOwner(BufferPool pool, List<char> buffer) {
            _pool = pool;
            Buffer = buffer;
        }

        public readonly void Dispose() => _pool.Return(Buffer);
    }
}