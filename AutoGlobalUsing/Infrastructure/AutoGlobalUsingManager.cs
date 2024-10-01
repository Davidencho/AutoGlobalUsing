namespace AutoGlobalUsing.Infrastructure
{
    internal class AutoGlobalUsingManager
    {
        private readonly string _fileNameResult;
        private readonly string _fileExistsRegex;
        private readonly bool _globalizeAliasDirective;
        private readonly DirectoryInfo _directoryInfo;
        private static readonly char directorySeparator = Path.DirectorySeparatorChar;
        private readonly bool _verbose;

        internal AutoGlobalUsingManager(string directoryPath, bool verbose = true)
        {
            if (!Directory.Exists(directoryPath))
                throw new ArgumentException(AutoGlobalUsingErrors.InvalidDirectoryPath(directoryPath));

            IConfiguration configuration = GetConfiguration();

            _fileNameResult = configuration["FileNameResult"] ?? throw new InvalidDataException(AutoGlobalUsingErrors.MissedFileNameResult);
            if (!_fileNameResult.EndsWith(".cs"))
                _fileNameResult += ".cs";

            _fileExistsRegex = configuration["FileExistsRegex"] ?? throw new InvalidDataException(AutoGlobalUsingErrors.MissedFileExistsRegex);
            _ = bool.TryParse(configuration["GlobalizeAliasDirective"], out _globalizeAliasDirective);
            _directoryInfo = new(directoryPath);
            _verbose = verbose;
        }

        internal async Task ExecuteAsync()
        {
            await ExecuteMultipleProjectsAsync(_directoryInfo);                        
        }

        private async Task ExecuteMultipleProjectsAsync(DirectoryInfo directoryInfo)
        {
            if (Directory.EnumerateFiles(directoryInfo.FullName, "*.csproj", SearchOption.TopDirectoryOnly).Any())
                await ExecuteSingleProjectAsync(directoryInfo);
            else
                foreach (DirectoryInfo dir in SearchSubFolder(directoryInfo))
                    await ExecuteMultipleProjectsAsync(dir);                
        }

        private async Task ExecuteSingleProjectAsync(DirectoryInfo directoryInfo)
        {
            SortedSet<string> usingSet = new(StringComparer.OrdinalIgnoreCase);

            IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryInfo.FullName, "*.cs", SearchOption.AllDirectories)
                .Where(it =>
                !it.Contains($"{directorySeparator}bin{directorySeparator}")
                &&
                !it.Contains($"{directorySeparator}obj{directorySeparator}"));

            foreach (string filePath in filePaths)
            {
                FileDotCs file = new(filePath);
                usingSet.UnionWith(await file.CollectUsingAsync(_globalizeAliasDirective));
                if (Regex.IsMatch(new FileInfo(filePath).Name, _fileExistsRegex))
                    file.Delete();
            }

            if (usingSet.Count != 0)
            {
                string globalUsingPath = Path.Combine(directoryInfo.FullName, _fileNameResult);
                FileDotCs globalUsingFile = new(globalUsingPath);
                if (File.Exists(globalUsingPath))
                    usingSet.UnionWith(await globalUsingFile.CollectUsingAsync(_globalizeAliasDirective));

                IEnumerable<string> globalLines = usingSet.Select(x => x.StartsWith("global") ? x : $"global {x}");

                await globalUsingFile.WriteLinesAsync(globalLines);

                if (_verbose)
                    foreach (string line in usingSet)
                        if (!line.StartsWith("global"))
                            Console.WriteLine($"{directoryInfo.FullName} -> global {line}");
            }
        }

        private static IConfiguration GetConfiguration()
        {
            ConfigurationBuilder builder = new();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json", false, true);
            return builder.Build();
        }

        private static IEnumerable<DirectoryInfo> SearchSubFolder(DirectoryInfo topDirectory) => 
            topDirectory
                .GetDirectories()
                .Where(x => x.Attributes != FileAttributes.Hidden && x.Attributes != FileAttributes.System && !x.Name.StartsWith('.'));
    }
}
