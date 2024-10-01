namespace AutoGlobalUsing.Extensions
{
    internal static class StreamReaderExtensions
    {
        internal static async IAsyncEnumerable<string> ReadLinesAsync(this StreamReader streamReader, bool closeStream = true)
        {
            string? line;

            while ((line = await streamReader.ReadLineAsync()) != null)
            {
                yield return line;
            }
            if (closeStream)
                streamReader.Close();
        }
    }
}
