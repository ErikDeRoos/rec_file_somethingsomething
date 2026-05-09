using rec_file_lib;
using rec_file_lib.DirectFileServer;
using rec_file_lib.Tests.ExampleData;

namespace rec_file_lib.Tests.DirectFileServer;

public sealed class DirectFileServerV1TypedTests
{
    [Fact]
    public void RecSelTyped_WithType_ReturnsSuccessAndTypedRecords()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var result = server.RecSel_Typed(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" }
            });

        Assert.Equal(0, result.StatusCode);
        Assert.Equal("OK", result.Message);
        Assert.Single(result.RecordSets);
        Assert.Equal("Person", result.RecordSets[0].RecordType);
        Assert.Equal(3, result.RecordSets[0].Records.Length);
        Assert.Equal("Alfred Nebel", GetFieldValue(result.RecordSets[0].Records[0], "Name"));
    }

    [Fact]
    public void RecSelTyped_WithUnknownType_ReturnsNoMatchStatus()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var result = server.RecSel_Typed(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Unknown" }
            });

        Assert.Equal(1, result.StatusCode);
        Assert.Equal("No matching records.", result.Message);
        Assert.Empty(result.RecordSets);
    }

    [Fact]
    public void RecSelTyped_WithoutTypeAndMultipleRecordSets_ReturnsInvalidRequestStatus()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var result = server.RecSel_Typed(workingCopy.FilePath, options: null);

        Assert.Equal(2, result.StatusCode);
        Assert.Equal("several record types found. Please use -t to specify one.", result.Message);
    }

    [Fact]
    public void RecSelTyped_WithInvalidExpression_ReturnsFormatStatus()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var result = server.RecSel_Typed(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { Expression = "Name ~~ Mandy" }
            });

        Assert.Equal(3, result.StatusCode);
        Assert.Equal("Invalid selection expression: 'Name ~~ Mandy'.", result.Message);
        Assert.Empty(result.RecordSets);
    }

    [Fact]
    public void RecSelTyped_WithJoinAndProjection_ReturnsProjectedTypedFields()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var result = server.RecSel_Typed(
            workingCopy.FilePath,
            new RecSelOptions
            {
                Type = new RecSelTypeOptions { RecordType = "Person" },
                Select = new RecSelSelectOptions { JoinField = "Abode" },
                Project = new RecSelProjectOptions { FieldNames = ["Name", "Residence.Address"] }
            });

        Assert.Equal(0, result.StatusCode);
        Assert.Single(result.RecordSets);
        var records = result.RecordSets[0].Records;
        Assert.Equal(3, records.Length);
        Assert.Equal("42 Abbeter Way, Inprooving, WORCS", GetFieldValue(records[0], "Residence.Address"));
        Assert.Equal("1 Wanter Rise, Greater Inncombe, BUCKS", GetFieldValue(records[2], "Residence.Address"));
    }

    [Fact]
    public void RecInsTyped_WithValidBook_ReturnsSuccessAndTypedRecordSet()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.SimpleRecutilsBookExample);
        var server = new DirectFileServerV1();

        var result = server.RecIns_Typed(
            workingCopy.FilePath,
            new RecInsOptions
            {
                RecordType = "Book",
                RecordText =
                """
                Title: Smalltalk Best Practice Patterns
                Author: Kent Beck
                Location: home
                """
            });

        Assert.Equal(0, result.StatusCode);
        Assert.Equal("OK", result.Message);
        Assert.Single(result.RecordSets);
        Assert.Equal("Book", result.RecordSets[0].RecordType);
        Assert.Equal(6, result.RecordSets[0].Records.Length);
        Assert.Equal("Smalltalk Best Practice Patterns", GetFieldValue(result.RecordSets[0].Records[^1], "Title"));
    }

    [Fact]
    public void RecInsTyped_WithValidationError_ReturnsOperationErrorStatus()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongDuplicateKeyValue);
        var server = new DirectFileServerV1();

        var result = server.RecIns_Typed(
            workingCopy.FilePath,
            new RecInsOptions
            {
                RecordType = "Item",
                RecordText =
                """
                Id: 2
                Title: Added item
                """
            });

        Assert.Equal(4, result.StatusCode);
        Assert.Contains("duplicate key value '1'", result.Message, StringComparison.Ordinal);
        Assert.Empty(result.RecordSets);
    }

    [Fact]
    public void RecInsTyped_WithMalformedSourceFile_ReturnsFormatErrorStatus()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongBadMultilineContinuation);
        var server = new DirectFileServerV1();

        var result = server.RecIns_Typed(
            workingCopy.FilePath,
            new RecInsOptions
            {
                RecordType = "Note",
                RecordText =
                """
                Title: Added note
                """
            });

        Assert.Equal(3, result.StatusCode);
        Assert.Equal("Continuation line found without a current field.", result.Message);
        Assert.Empty(result.RecordSets);
    }

    [Fact]
    public void RecDelTyped_WithPersonType_ReturnsSuccessAndEmptyTypedRecordSet()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var result = server.RecDel_Typed(
            workingCopy.FilePath,
            new RecDelOptions
            {
                RecordType = "Person"
            });

        Assert.Equal(0, result.StatusCode);
        Assert.Equal("OK", result.Message);
        Assert.Single(result.RecordSets);
        Assert.Equal("Person", result.RecordSets[0].RecordType);
        Assert.Empty(result.RecordSets[0].Records);
    }

    [Fact]
    public void RecDelTyped_WithValidationError_ReturnsOperationErrorStatus()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.MultipleRecordTypesSingleFile);
        var server = new DirectFileServerV1();

        var result = server.RecDel_Typed(
            workingCopy.FilePath,
            new RecDelOptions
            {
                RecordType = "Residence"
            });

        Assert.Equal(4, result.StatusCode);
        Assert.Contains("references unknown key '42AbbeterWay'", result.Message, StringComparison.Ordinal);
        Assert.Empty(result.RecordSets);
    }

    [Fact]
    public void RecDelTyped_WithMalformedSourceFile_ReturnsFormatErrorStatus()
    {
        using var workingCopy = RecExampleWorkingCopy.Create(RecExampleScenario.WrongMissingFieldSeparator);
        var server = new DirectFileServerV1();

        var result = server.RecDel_Typed(
            workingCopy.FilePath,
            new RecDelOptions
            {
                RecordType = "Article"
            });

        Assert.Equal(3, result.StatusCode);
        Assert.Equal("Invalid field line: %key Id", result.Message);
        Assert.Empty(result.RecordSets);
    }

    private static string? GetFieldValue(IRecSelTypedRecord record, string fieldName)
    {
        return record.Fields.FirstOrDefault(field => field.Name == fieldName)?.Value;
    }
}
