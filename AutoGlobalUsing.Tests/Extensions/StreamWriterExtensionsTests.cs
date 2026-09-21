namespace AutoGlobalUsing.Tests.Extensions;

public class StreamWriterExtensionsTests
{
    [Fact]
    public async Task WriteLinesAsync_WritesAllLines_WithoutTrailingNewline()
    {
        using MemoryStream stream = new();
        StreamWriter writer = new(stream, leaveOpen: true) { AutoFlush = true };

        await writer.WriteLinesAsync(["line1", "line2", "line3"], closeStream: false);

        stream.Position = 0;
        string result = new StreamReader(stream).ReadToEnd();

        Assert.Equal($"line1{Environment.NewLine}line2{Environment.NewLine}line3", result);
    }

    [Fact]
    public async Task WriteLinesAsync_EmptyCollection_WritesNothing()
    {
        using MemoryStream stream = new();
        StreamWriter writer = new(stream, leaveOpen: true) { AutoFlush = true };

        await writer.WriteLinesAsync([], closeStream: false);

        stream.Position = 0;
        string result = new StreamReader(stream).ReadToEnd();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task WriteLinesAsync_ClosesStream_WhenRequested()
    {
        MemoryStream stream = new();
        StreamWriter writer = new(stream, leaveOpen: false);

        await writer.WriteLinesAsync(["a"], closeStream: true);

        Assert.Throws<ObjectDisposedException>(() => stream.Position);
    }
}
