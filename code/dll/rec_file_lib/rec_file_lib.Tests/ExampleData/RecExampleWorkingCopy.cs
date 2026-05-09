namespace rec_file_lib.Tests.ExampleData;

internal sealed class RecExampleWorkingCopy : IDisposable
{
    private readonly DirectoryInfo _workingDirectory;
    private bool _disposed;

    private RecExampleWorkingCopy(DirectoryInfo workingDirectory)
    {
        _workingDirectory = workingDirectory;
    }

    public static RecExampleWorkingCopy Create(RecExampleScenario scenario)
    {
        var sourceFilePath = RecExampleData.GetExampleFilePath(scenario);
        var tempRoot = GetTempRootDirectory();
        tempRoot.Create();

        var workingDirectory = tempRoot.CreateSubdirectory($"work_{Guid.NewGuid():N}");
        var workingFilePath = Path.Combine(workingDirectory.FullName, Path.GetFileName(sourceFilePath));
        File.Copy(sourceFilePath, workingFilePath, overwrite: false);

        return new RecExampleWorkingCopy(workingDirectory)
        {
            FilePath = workingFilePath,
        };
    }

    public string FilePath { get; private init; } = string.Empty;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            if (_workingDirectory.Exists)
            {
                _workingDirectory.Delete(recursive: true);
            }
        }
        catch (DirectoryNotFoundException)
        {
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    ~RecExampleWorkingCopy()
    {
        Dispose();
    }

    private static DirectoryInfo GetTempRootDirectory()
    {
        return new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "TestWorkingCopies"));
    }
}
