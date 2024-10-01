namespace AutoGlobalUsing.Errors
{
    internal static class AutoGlobalUsingErrors
    {
        internal static string InvalidDirectoryPath(string directoryPath) => $"{directoryPath} is not a folder.";
        internal static string MissedFileNameResult => "Missed \"FileNameResult\" in appsetting.json";
        internal static string MissedFileExistsRegex => "Missed \"FileExistsRegex\" in appsetting.json";
    }
}
