using System.Buffers;

namespace PrettyConsole;

internal sealed class RentedBuffer<T> : IDisposable {
    public readonly T[] Array;
    private bool _disposed;

    public RentedBuffer(int length) {
        Array = ArrayPool<T>.Shared.Rent(length);
    }

    private const int ShortBufferLength = 20;

    public static RentedBuffer<T> ShortBuffer => new(ShortBufferLength);

    public void Dispose() {
        if (Volatile.Read(ref _disposed)) {
            return;
        }
        ArrayPool<T>.Shared.Return(Array);
        Volatile.Write(ref _disposed, true);
    }
}