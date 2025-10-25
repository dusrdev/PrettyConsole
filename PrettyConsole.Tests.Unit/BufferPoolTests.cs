namespace PrettyConsole.Tests.Unit;

public class BufferPoolTests {
    [Fact]
    public void Rent_ReturnsFastItemAfterReturn() {
        using var pool = CreatePool();

        var owner = pool.Rent(out var firstBuffer);
        firstBuffer.Add('x');
        owner.Dispose();

        using var secondOwner = pool.Rent(out var reusedBuffer);

        Assert.Same(firstBuffer, reusedBuffer);
        Assert.Empty(reusedBuffer);
    }

    [Fact]
    public void Return_DropsOversizedLists() {
        using var pool = CreatePool();

        List<char> oversized;
        using (var owner = pool.Rent(out var buffer)) {
            oversized = buffer;
            buffer.AddRange(new string('x', BufferPool.ListMaxSize + 1));
        }

        using var nextOwner = pool.Rent(out var nextBuffer);

        Assert.NotSame(oversized, nextBuffer);
        Assert.Equal(BufferPool.ListStartingSize, nextBuffer.Capacity);
    }

    [Fact]
    public void Value_ThrowsAfterDispose() {
        using var pool = CreatePool();

        var owner = pool.Rent(out _);
        owner.Dispose();

        Assert.Throws<InvalidOperationException>(() => _ = owner.Value);
    }

    private static BufferPool CreatePool()
        => (BufferPool)Activator.CreateInstance(typeof(BufferPool), nonPublic: true)!;
}