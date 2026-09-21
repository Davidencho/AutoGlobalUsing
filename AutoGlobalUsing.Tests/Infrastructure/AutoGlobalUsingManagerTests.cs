namespace AutoGlobalUsing.Tests.Infrastructure;

public class AutoGlobalUsingManagerTests : IDisposable
{
    private readonly string _tempDirectory;

    public AutoGlobalUsingManagerTests()
    {
        _tempDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), $"AutoGlobalUsingManagerTests_{Guid.NewGuid()}")).FullName;
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);

        GC.SuppressFinalize(this);
    }

    private void CreateFile(string relativePath, string content)
    {
        string path = Path.Combine(_tempDirectory, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content, new UTF8Encoding(true));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenDirectoryDoesNotExist()
    {
        string invalidPath = Path.Combine(_tempDirectory, "does-not-exist");

        ArgumentException exception = Assert.Throws<ArgumentException>(() => new AutoGlobalUsingManager(invalidPath));
        Assert.Equal(AutoGlobalUsingErrors.InvalidDirectoryPath(invalidPath), exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesGlobalUsingsFile_WithCollectedUsingsFromProject()
    {
        CreateFile("MyProject.csproj", "<Project />");
        CreateFile("Foo.cs", "using System;\nusing System.Linq;\n\nnamespace Foo;\n\nclass Foo { }\n");
        CreateFile("Bar.cs", "using System;\nusing System.Collections.Generic;\n\nnamespace Foo;\n\nclass Bar { }\n");

        AutoGlobalUsingManager manager = new(_tempDirectory, verbose: false);
        await manager.ExecuteAsync();

        string globalUsingsPath = Path.Combine(_tempDirectory, "GlobalUsings.cs");
        Assert.True(File.Exists(globalUsingsPath));

        string content = await File.ReadAllTextAsync(globalUsingsPath);
        Assert.Contains("global using System;", content);
        Assert.Contains("global using System.Linq;", content);
        Assert.Contains("global using System.Collections.Generic;", content);

        string fooContent = await File.ReadAllTextAsync(Path.Combine(_tempDirectory, "Foo.cs"));
        Assert.DoesNotContain("using System;", fooContent);
        Assert.Contains("class Foo { }", fooContent);
    }

    [Fact]
    public async Task ExecuteAsync_RecursesIntoSubProjects_WhenNoCsprojInRoot()
    {
        CreateFile("SubA/ProjectA.csproj", "<Project />");
        CreateFile("SubA/A.cs", "using System;\n\nnamespace SubA;\n\nclass A { }\n");
        CreateFile("SubB/ProjectB.csproj", "<Project />");
        CreateFile("SubB/B.cs", "using System.Text;\n\nnamespace SubB;\n\nclass B { }\n");

        AutoGlobalUsingManager manager = new(_tempDirectory, verbose: false);
        await manager.ExecuteAsync();

        Assert.True(File.Exists(Path.Combine(_tempDirectory, "SubA", "GlobalUsings.cs")));
        Assert.True(File.Exists(Path.Combine(_tempDirectory, "SubB", "GlobalUsings.cs")));
        Assert.False(File.Exists(Path.Combine(_tempDirectory, "GlobalUsings.cs")));
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotCreateGlobalUsingsFile_WhenNoUsingsFound()
    {
        CreateFile("MyProject.csproj", "<Project />");
        CreateFile("Foo.cs", "namespace Foo;\n\nclass Foo { }\n");

        AutoGlobalUsingManager manager = new(_tempDirectory, verbose: false);
        await manager.ExecuteAsync();

        Assert.False(File.Exists(Path.Combine(_tempDirectory, "GlobalUsings.cs")));
    }

    [Fact]
    public async Task ExecuteAsync_MergesWithExistingGlobalUsingsFile()
    {
        CreateFile("MyProject.csproj", "<Project />");
        CreateFile("GlobalUsings.cs", "global using System.Text;\n");
        CreateFile("Foo.cs", "using System;\n\nnamespace Foo;\n\nclass Foo { }\n");

        AutoGlobalUsingManager manager = new(_tempDirectory, verbose: false);
        await manager.ExecuteAsync();

        string content = await File.ReadAllTextAsync(Path.Combine(_tempDirectory, "GlobalUsings.cs"));
        Assert.Contains("global using System.Text;", content);
        Assert.Contains("global using System;", content);
    }
}
