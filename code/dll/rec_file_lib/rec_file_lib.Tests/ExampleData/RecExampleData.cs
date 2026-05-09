namespace rec_file_lib.Tests.ExampleData;

internal enum RecExampleScenario
{
    SimpleSingleFile,
    SimpleRecutilsBookExample,
}

internal static class RecExampleData
{
    public static string GetExampleFilePath(RecExampleScenario scenario)
    {
        var repositoryRoot = FindRepositoryRoot();
        var relativePath = scenario switch
        {
            RecExampleScenario.SimpleSingleFile => Path.Combine("docs", "examples", "1_simple_singlefile", "user.rec"),
            RecExampleScenario.SimpleRecutilsBookExample => Path.Combine("docs", "examples", "2_simple_recutils_book_example", "books.rec"),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null),
        };

        return Path.Combine(repositoryRoot, relativePath);
    }

    public static string ReadAllText(RecExampleScenario scenario)
    {
        return File.ReadAllText(GetExampleFilePath(scenario));
    }

    public static TextReader OpenText(RecExampleScenario scenario)
    {
        return File.OpenText(GetExampleFilePath(scenario));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "docs", "examples"))
                && File.Exists(Path.Combine(directory.FullName, "AGENTS.md")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root containing docs/examples.");
    }
}
