namespace AutoGlobalUsing.Tests.Extensions;

public class StreamReaderExtensionsTests
{
    [Fact]
    public async Task ReadLinesAsync_ReturnsAllLines_InOrder()
    {
        string content = "line1\nline2\nline3";
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
        using StreamReader reader = new(stream);

        List<string> lines = [];
        await foreach (string line in reader.ReadLinesAsync(closeStream: false))
            lines.Add(line);

        Assert.Equal(["line1", "line2", "line3"], lines);
    }

    [Fact]
    public async Task ReadLinesAsync_EmptyStream_ReturnsNoLines()
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(string.Empty));
        using StreamReader reader = new(stream);

        List<string> lines = [];
        await foreach (string line in reader.ReadLinesAsync(closeStream: false))
            lines.Add(line);

        Assert.Empty(lines);
    }

    [Fact]
    public async Task ReadLinesAsync_ClosesStream_WhenRequested()
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes("a\nb"));
        StreamReader reader = new(stream);

        await foreach (string _ in reader.ReadLinesAsync())
        {
            // consume
        }

        Assert.Throws<ObjectDisposedException>(() => reader.Peek());
    }
}
