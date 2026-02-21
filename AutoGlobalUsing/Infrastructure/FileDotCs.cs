namespace AutoGlobalUsing.Infrastructure
{
    internal class FileDotCs
    {
        private readonly FileInfo _fileInfo;

        internal FileDotCs(string path)
        {
            _fileInfo = new FileInfo(path);
        }

        internal async Task<IEnumerable<string>> CollectUsingAsync(bool globalizeAliasDirective, bool trailingNewline)
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
            await WriteLinesAsync(linesWithoutUsing, trailingNewline);
            return usingLines;
        }

        internal async IAsyncEnumerable<string> ReadLinesAsync()
        {
            StreamReader streamReader = new(_fileInfo.FullName);
            await foreach (string line in streamReader.ReadLinesAsync())
                yield return line;
        }

        internal async Task WriteLinesAsync(IEnumerable<string> lines, bool trailingNewline)
        {
            StreamWriter streamWriter = new(_fileInfo.FullName, false, new UTF8Encoding(true));

            string[] arrayLines = lines.ToArray();

            int lenght = arrayLines.Length;

            if (lenght > 0)
            {
                int i;

                for (i = 0; i< lenght -1; i++)
                {
                    string line = arrayLines[i];
                    await streamWriter.WriteLineAsync(line);
                }

                string lastLine = arrayLines[i];

                if (trailingNewline)
                    await streamWriter.WriteLineAsync(lastLine);
                else
                    await streamWriter.WriteAsync(lastLine);                
            }

            streamWriter.Close();
        }

        internal void Delete()
        {
            _fileInfo.Delete();
        }
    }
}