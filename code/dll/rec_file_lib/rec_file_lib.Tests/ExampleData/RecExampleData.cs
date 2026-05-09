namespace rec_file_lib.Tests.ExampleData;

internal enum RecExampleScenario
{
    SimpleSingleFile,
    SimpleRecutilsBookExample,
    WrongMissingMandatoryField,
    WrongDuplicateKeyValue,
    WrongInvalidFieldType,
    WrongBadMultilineContinuation,
    WrongMissingFieldSeparator,
    MultipleRecordTypesSingleFile,
    IntFieldType,
    WrongInvalidIntFieldType,
    RepeatedFieldsBacktracking,
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
            RecExampleScenario.WrongMissingMandatoryField => Path.Combine("docs", "examples", "3_wrong_missing_mandatory_field", "missing_name.rec"),
            RecExampleScenario.WrongDuplicateKeyValue => Path.Combine("docs", "examples", "4_wrong_duplicate_key_value", "duplicate_id.rec"),
            RecExampleScenario.WrongInvalidFieldType => Path.Combine("docs", "examples", "5_wrong_invalid_field_type", "invalid_status.rec"),
            RecExampleScenario.WrongBadMultilineContinuation => Path.Combine("docs", "examples", "6_wrong_bad_multiline_continuation", "orphan_continuation.rec"),
            RecExampleScenario.WrongMissingFieldSeparator => Path.Combine("docs", "examples", "7_wrong_missing_field_separator", "missing_colon.rec"),
            RecExampleScenario.MultipleRecordTypesSingleFile => Path.Combine("docs", "examples", "8_multiple_record_types_single_file", "people_and_residences.rec"),
            RecExampleScenario.IntFieldType => Path.Combine("docs", "examples", "9_int_field_type", "tasks.rec"),
            RecExampleScenario.WrongInvalidIntFieldType => Path.Combine("docs", "examples", "10_wrong_invalid_int_field_type", "invalid_priority.rec"),
            RecExampleScenario.RepeatedFieldsBacktracking => Path.Combine("docs", "examples", "11_repeated_fields_backtracking", "repeated_tags.rec"),
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
