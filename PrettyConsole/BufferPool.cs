using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace PrettyConsole;

internal sealed class BufferPool {
	internal static readonly BufferPool Shared = new();

	private readonly ConcurrentQueue<List<char>> _collection;
    private long _poolSize;
    private readonly int _maxCapacity;
    private List<char>? _fastItem;

	public const int ListStartingSize = 256;

    private BufferPool() {
		_collection = new();
		_maxCapacity = Environment.ProcessorCount * 2;
	}

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public RentedBufferOwner Rent() {
        var buffer = _fastItem;
        if (buffer is null || Interlocked.CompareExchange(ref _fastItem, null, buffer) != buffer) {
            if (_collection.TryDequeue(out buffer)) {
                Interlocked.Decrement(ref _poolSize);
                // return item;
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
			if (Interlocked.Increment(ref _poolSize) <= _maxCapacity) {
				buffer.Clear();
				_collection.Enqueue(buffer);
			} else {
				// no room, clean up the count and drop the object on the floor
				Interlocked.Decrement(ref _poolSize);
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