namespace AutoGlobalUsing.Infrastructure
{
    internal class FileDotCs
    {
        private readonly FileInfo _fileInfo;

        internal FileDotCs(string path)
        {
            _fileInfo = new FileInfo(path);
        }

        internal async Task<IEnumerable<string>> CollectUsingAsync(bool globalizeAliasDirective)
        {
            List<string> usingLines = [];
            List<string> linesWithoutUsing = [];

            bool lastLineWasAnUsingLine = false;
            await foreach (string line in ReadLinesAsync())
            {
                if (!string.IsNullOrWhiteSpace(line)
                    && (line.StartsWith("using") || line.StartsWith("global using"))
                    && line.EndsWith(';')
                    && !line.Contains(" static ")
                    && !line.Contains('(')
                    && !line.Contains(')')
                    && (globalizeAliasDirective || !line.Contains('='))
                )
                {
                    usingLines.Add(line);
                    lastLineWasAnUsingLine = true;
                }
                else if (lastLineWasAnUsingLine && string.IsNullOrWhiteSpace(line)) // Remove empty lines beewteen using and namespace
                    lastLineWasAnUsingLine = false;
                else
                    linesWithoutUsing.Add(line);
            }
            await WriteLinesAsync(linesWithoutUsing);
            return usingLines;
        }

        internal async IAsyncEnumerable<string> ReadLinesAsync()
        {
            StreamReader streamReader = new(_fileInfo.FullName);
            await foreach (string line in streamReader.ReadLinesAsync())            
                yield return line;            
        }

        internal async Task WriteLinesAsync(IEnumerable<string> lines)
        {
            StreamWriter streamWriter = new(_fileInfo.FullName, false, Encoding.UTF8);
            await streamWriter.WriteLinesAsync(lines);
        }

        internal void Delete()
        {
            _fileInfo.Delete();
        }
    }
}