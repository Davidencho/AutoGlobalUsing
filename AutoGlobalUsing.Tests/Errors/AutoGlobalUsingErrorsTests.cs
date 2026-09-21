namespace AutoGlobalUsing.Tests.Errors;

public class AutoGlobalUsingErrorsTests
{
    [Fact]
    public void InvalidDirectoryPath_ReturnsExpectedMessage()
    {
        string message = AutoGlobalUsingErrors.InvalidDirectoryPath(@"C:\not\found");

        Assert.Equal(@"C:\not\found is not a folder.", message);
    }

    [Fact]
    public void MissedFileNameResult_ReturnsExpectedMessage()
    {
        Assert.Equal("Missed \"FileNameResult\" in appsetting.json", AutoGlobalUsingErrors.MissedFileNameResult);
    }

    [Fact]
    public void MissedFileExistsRegex_ReturnsExpectedMessage()
    {
        Assert.Equal("Missed \"FileExistsRegex\" in appsetting.json", AutoGlobalUsingErrors.MissedFileExistsRegex);
    }
}
