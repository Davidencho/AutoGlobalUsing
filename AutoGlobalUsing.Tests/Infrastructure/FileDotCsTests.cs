namespace AutoGlobalUsing.Tests.Infrastructure;

public class FileDotCsTests : IDisposable
{
    private readonly string _tempDirectory;

    public FileDotCsTests()
    {
        _tempDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), $"FileDotCsTests_{Guid.NewGuid()}")).FullName;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);

        GC.SuppressFinalize(this);
    }

    private string CreateFile(string fileName, string content)
    {
        string path = Path.Combine(_tempDirectory, fileName);
        File.WriteAllText(path, content, new UTF8Encoding(true));
        return path;
    }

    [Fact]
    public async Task CollectUsingAsync_ExtractsUsingLines_AndRemovesThemFromFile()
    {
        string content = "using System;\nusing System.Linq;\n\nnamespace Foo;\n\nclass Bar { }\n";
        string path = CreateFile("Sample.cs", content);
        FileDotCs file = new(path);

        IEnumerable<string> usingLines = await file.CollectUsingAsync(globalizeAliasDirective: false, trailingNewline: true);

        Assert.Equal(["using System;", "using System.Linq;"], usingLines);

        string remaining = await File.ReadAllTextAsync(path);
        Assert.DoesNotContain("using System;", remaining);
        Assert.Contains("namespace Foo;", remaining);
        Assert.Contains("class Bar { }", remaining);
    }

    [Fact]
    public async Task CollectUsingAsync_IgnoresStaticAndAliasUsings_WhenNotGlobalized()
    {
        string content = "using System;\nusing static System.Console;\nusing Alias = System.String;\n\nnamespace Foo;\n";
        string path = CreateFile("Sample.cs", content);
        FileDotCs file = new(path);

        IEnumerable<string> usingLines = await file.CollectUsingAsync(globalizeAliasDirective: false, trailingNewline: true);

        Assert.Equal(["using System;"], usingLines);
    }

    [Fact]
    public async Task CollectUsingAsync_IncludesAliasUsings_WhenGlobalized()
    {
        string content = "using System;\nusing Alias = System.String;\n\nnamespace Foo;\n";
        string path = CreateFile("Sample.cs", content);
        FileDotCs file = new(path);

        IEnumerable<string> usingLines = await file.CollectUsingAsync(globalizeAliasDirective: true, trailingNewline: true);

        Assert.Equal(["using System;", "using Alias = System.String;"], usingLines);
    }

    [Fact]
    public async Task WriteLinesAsync_WithTrailingNewline_AddsFinalNewline()
    {
        string path = CreateFile("Out.cs", string.Empty);
        FileDotCs file = new(path);

        await file.WriteLinesAsync(["line1", "line2"], trailingNewline: true);

        string result = await File.ReadAllTextAsync(path);
        Assert.Equal($"line1{Environment.NewLine}line2{Environment.NewLine}", result);
    }

    [Fact]
    public async Task WriteLinesAsync_WithoutTrailingNewline_DoesNotAddFinalNewline()
    {
        string path = CreateFile("Out.cs", string.Empty);
        FileDotCs file = new(path);

        await file.WriteLinesAsync(["line1", "line2"], trailingNewline: false);

        string result = await File.ReadAllTextAsync(path);
        Assert.Equal($"line1{Environment.NewLine}line2", result);
    }

    [Fact]
    public void Delete_RemovesFileFromDisk()
    {
        string path = CreateFile("ToDelete.cs", "content");
        FileDotCs file = new(path);

        file.Delete();

        Assert.False(File.Exists(path));
    }
}
