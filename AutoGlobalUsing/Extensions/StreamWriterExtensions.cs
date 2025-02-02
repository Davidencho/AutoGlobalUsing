namespace AutoGlobalUsing.Extensions
{
    internal static class StreamWriterExtensions
    {
        internal static async Task WriteLinesAsync(this StreamWriter streamWriter, IEnumerable<string> lines, bool closeStream = true)
        {
            if (lines.Any())
            {
                foreach (string line in lines.SkipLast(1))
                    await streamWriter.WriteLineAsync(line);

                await streamWriter.WriteAsync(lines.Last());
            }
            
            if (closeStream)
                streamWriter.Close();
        }
    }
}
