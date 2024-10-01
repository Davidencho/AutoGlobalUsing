namespace AutoGlobalUsing.Extensions
{
    internal static class StreamWriterExtensions
    {
        internal static async Task WriteLinesAsync(this StreamWriter streamWriter, IEnumerable<string> lines, bool closeStream = true)
        {
            foreach (string line in lines)
                await streamWriter.WriteLineAsync(line);
            
            if (closeStream)
                streamWriter.Close();
        }
    }
}
