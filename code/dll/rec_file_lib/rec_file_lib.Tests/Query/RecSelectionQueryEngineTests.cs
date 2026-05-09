using rec_file_lib.Model;
using rec_file_lib.Query;

namespace rec_file_lib.Tests.Query;

public sealed class RecSelectionQueryEngineTests
{
    [Fact]
    public void Select_WithIndexes_OnlyReturnsRequestedRecordPositions()
    {
        var engine = new RecSelectionQueryEngine();
        var recordSet = CreatePersonRecordSet();

        var result = engine.Select(
            recordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: null,
                SelectedIndexes: new HashSet<int> { 1, 2 },
                QuickFilter: null));

        Assert.NotNull(result);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("Mandy Nebel", GetFieldValue(result.Records[0], "Name"));
        Assert.Equal("Ernest Wright", GetFieldValue(result.Records[1], "Name"));
    }

    [Fact]
    public void Select_WithProjectedFields_OnlyKeepsRequestedFieldsPerRecord()
    {
        var engine = new RecSelectionQueryEngine();
        var recordSet = CreatePersonRecordSet();

        var result = engine.Select(
            recordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Name", "Email" },
                SelectedIndexes: null,
                QuickFilter: null));

        Assert.NotNull(result);
        Assert.Equal(3, result.Records.Count);
        Assert.Equal(2, result.Records[0].Fields.Count);
        Assert.Equal(2, result.Records[1].Fields.Count);
        Assert.Single(result.Records[2].Fields);
    }

    [Fact]
    public void Select_WithQuickFilter_OnlyReturnsRecordsWithMatchingFieldValue()
    {
        var engine = new RecSelectionQueryEngine();
        var recordSet = CreatePersonRecordSet();

        var result = engine.Select(
            recordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: null,
                SelectedIndexes: null,
                QuickFilter: "Chez"));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Equal("Ernest Wright", GetFieldValue(result.Records[0], "Name"));
    }

    [Fact]
    public void Select_WithNullRecordSet_ReturnsNull()
    {
        var engine = new RecSelectionQueryEngine();

        var result = engine.Select(
            recordSet: null,
            new RecSelectionQueryOptions(
                ProjectedFields: null,
                SelectedIndexes: null,
                QuickFilter: null));

        Assert.Null(result);
    }

    [Fact]
    public void Select_WithEmptyRecordSet_ReturnsEmptyRecordSet()
    {
        var engine = new RecSelectionQueryEngine();
        var emptyRecordSet = new RecRecordSet(
            TypeName: "Person",
            Descriptor: new RecDescriptor(
                Fields: [],
                KeyFieldName: null,
                FieldTypes: new Dictionary<string, string>(StringComparer.Ordinal),
                MandatoryFieldNames: [],
                Documentation: null),
            Records: []);

        var result = engine.Select(
            emptyRecordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: null,
                SelectedIndexes: new HashSet<int> { 0, 1 },
                QuickFilter: "anything"));

        Assert.NotNull(result);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void Select_WithOutOfRangeIndexes_ReturnsEmptyRecordSet()
    {
        var engine = new RecSelectionQueryEngine();
        var recordSet = CreatePersonRecordSet();

        var result = engine.Select(
            recordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: null,
                SelectedIndexes: new HashSet<int> { 99, 100 },
                QuickFilter: null));

        Assert.NotNull(result);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void Select_WithUnknownProjectedFields_ReturnsRecordsWithNoFields()
    {
        var engine = new RecSelectionQueryEngine();
        var recordSet = CreatePersonRecordSet();

        var result = engine.Select(
            recordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "UnknownField" },
                SelectedIndexes: null,
                QuickFilter: null));

        Assert.NotNull(result);
        Assert.Equal(3, result.Records.Count);
        Assert.All(result.Records, record => Assert.Empty(record.Fields));
    }

    [Fact]
    public void Select_WithWhitespaceQuickFilter_TreatedAsLiteralAndReturnsNoMatches()
    {
        var engine = new RecSelectionQueryEngine();
        var recordSet = CreatePersonRecordSet();

        var result = engine.Select(
            recordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: null,
                SelectedIndexes: null,
                QuickFilter: "   "));

        Assert.NotNull(result);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void Select_WithCaseMismatchedQuickFilter_DoesNotMatch()
    {
        var engine = new RecSelectionQueryEngine();
        var recordSet = CreatePersonRecordSet();

        var result = engine.Select(
            recordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: null,
                SelectedIndexes: null,
                QuickFilter: "chez"));

        Assert.NotNull(result);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void Select_WithIndexesQuickAndProjection_AppliesCombinedSemantics()
    {
        var engine = new RecSelectionQueryEngine();
        var recordSet = CreatePersonRecordSet();

        var result = engine.Select(
            recordSet,
            new RecSelectionQueryOptions(
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Name" },
                SelectedIndexes: new HashSet<int> { 1, 2 },
                QuickFilter: "Chez"));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Single(result.Records[0].Fields);
        Assert.Equal("Name", result.Records[0].Fields[0].Name);
        Assert.Equal("Ernest Wright", result.Records[0].Fields[0].Value);
    }

    private static RecRecordSet CreatePersonRecordSet()
    {
        return new RecRecordSet(
            TypeName: "Person",
            Descriptor: new RecDescriptor(
                Fields: [],
                KeyFieldName: null,
                FieldTypes: new Dictionary<string, string>(StringComparer.Ordinal),
                MandatoryFieldNames: [],
                Documentation: null),
            Records:
            [
                new RecRecord([
                    new RecField("Name", "Alfred Nebel"),
                    new RecField("Email", "alf@example.com"),
                    new RecField("Abode", "42AbbeterWay")]),
                new RecRecord([
                    new RecField("Name", "Mandy Nebel"),
                    new RecField("Email", "mandy@example.com"),
                    new RecField("Abode", "42AbbeterWay")]),
                new RecRecord([
                    new RecField("Name", "Ernest Wright"),
                    new RecField("Abode", "ChezGrampa")])
            ]);
    }

    private static string? GetFieldValue(RecRecord record, string fieldName)
    {
        return record.Fields.FirstOrDefault(field => field.Name == fieldName)?.Value;
    }
}
