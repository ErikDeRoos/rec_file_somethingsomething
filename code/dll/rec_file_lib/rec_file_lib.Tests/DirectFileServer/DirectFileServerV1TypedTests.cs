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

    private static string? GetFieldValue(IRecSelTypedRecord record, string fieldName)
    {
        return record.Fields.FirstOrDefault(field => field.Name == fieldName)?.Value;
    }
}
