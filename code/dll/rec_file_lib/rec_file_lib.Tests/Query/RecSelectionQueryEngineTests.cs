using rec_file_lib.Model;
using rec_file_lib.Query;

namespace rec_file_lib.Tests.Query;

public sealed class RecSelectionQueryEngineTests
{
    [Fact]
    public void Select_WithIndexes_OnlyReturnsRequestedRecordPositions()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(SelectedIndexes: new HashSet<int> { 1, 2 }));

        Assert.NotNull(result);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("Mandy Nebel", GetFieldValue(result.Records[0], "Name"));
        Assert.Equal("Ernest Wright", GetFieldValue(result.Records[1], "Name"));
    }

    [Fact]
    public void Select_WithProjectedFields_OnlyKeepsRequestedFieldsPerRecord()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Name", "Email" }));

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
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(document, recordSet, CreateOptions(QuickFilter: "Chez"));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Equal("Ernest Wright", GetFieldValue(result.Records[0], "Name"));
    }

    [Fact]
    public void Select_WithExpressionEquals_OnlyReturnsMatchingRecords()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(document, recordSet, CreateOptions(Expression: "Name = \"Mandy Nebel\""));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Equal("Mandy Nebel", GetFieldValue(result.Records[0], "Name"));
    }

    [Fact]
    public void Select_WithExpressionNotEquals_OnlyReturnsNonMatchingRecords()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(document, recordSet, CreateOptions(Expression: "Name != \"Mandy Nebel\""));

        Assert.NotNull(result);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("Alfred Nebel", GetFieldValue(result.Records[0], "Name"));
        Assert.Equal("Ernest Wright", GetFieldValue(result.Records[1], "Name"));
    }

    [Fact]
    public void Select_WithExpressionContains_OnlyReturnsMatchingRecords()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(document, recordSet, CreateOptions(Expression: "Email ~ \"example.com\""));

        Assert.NotNull(result);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("Alfred Nebel", GetFieldValue(result.Records[0], "Name"));
        Assert.Equal("Mandy Nebel", GetFieldValue(result.Records[1], "Name"));
    }

    [Fact]
    public void Select_WithJoinField_InnerJoinsAndAddsPrefixedJoinedFields()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var personRecordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(document, personRecordSet, CreateOptions(JoinField: "Abode"));

        Assert.NotNull(result);
        Assert.Equal(3, result.Records.Count);
        Assert.Equal("42 Abbeter Way, Inprooving, WORCS", GetFieldValue(result.Records[0], "Residence.Address"));
        Assert.Equal("01234 5676789", GetFieldValue(result.Records[1], "Residence.Telephone"));
        Assert.Equal("1 Wanter Rise, Greater Inncombe, BUCKS", GetFieldValue(result.Records[2], "Residence.Address"));
    }

    [Fact]
    public void Select_WithJoinFieldAndProjection_CanProjectJoinedFields()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var personRecordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            personRecordSet,
            CreateOptions(
                JoinField: "Abode",
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Name", "Residence.Address" }));

        Assert.NotNull(result);
        Assert.Equal(3, result.Records.Count);
        Assert.Equal(2, result.Records[0].Fields.Count);
        Assert.Equal("42 Abbeter Way, Inprooving, WORCS", GetFieldValue(result.Records[0], "Residence.Address"));
    }

    [Fact]
    public void Select_WithGroupByFieldAndCount_GroupsRecordsAndAddsCount()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var personRecordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            personRecordSet,
            CreateOptions(
                GroupByFields: ["Abode"],
                Count: true,
                CountFieldName: "Total"));

        Assert.NotNull(result);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("42AbbeterWay", GetFieldValue(result.Records[0], "Abode"));
        Assert.Equal("2", GetFieldValue(result.Records[0], "Total"));
        Assert.Equal("ChezGrampa", GetFieldValue(result.Records[1], "Abode"));
        Assert.Equal("1", GetFieldValue(result.Records[1], "Total"));
    }

    [Fact]
    public void Select_WithCountOnly_ReturnsSingleCountRecord()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var personRecordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            personRecordSet,
            CreateOptions(Count: true, CountFieldName: "Count"));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Equal("3", GetFieldValue(result.Records[0], "Count"));
    }

    [Fact]
    public void Select_WithJoinFieldNotDeclaredAsRecType_ThrowsInvalidOperationException()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var personRecordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var exception = Assert.Throws<InvalidOperationException>(() =>
            engine.Select(document, personRecordSet, CreateOptions(JoinField: "Name")));

        Assert.Equal("join field 'Name' is not declared with a rec type.", exception.Message);
    }

    [Fact]
    public void Select_WithInvalidExpression_ThrowsFormatException()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var exception = Assert.Throws<FormatException>(() =>
            engine.Select(document, recordSet, CreateOptions(Expression: "Name ~~ Mandy")));

        Assert.Equal("Invalid selection expression: 'Name ~~ Mandy'.", exception.Message);
    }

    [Fact]
    public void Select_WithNullRecordSet_ReturnsNull()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();

        var result = engine.Select(document, recordSet: null, CreateOptions());

        Assert.Null(result);
    }

    [Fact]
    public void Select_WithEmptyRecordSet_ReturnsEmptyRecordSet()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var emptyRecordSet = new RecRecordSet(
            TypeName: "Person",
            Descriptor: new RecDescriptor(
                Fields: [],
                KeyFieldName: null,
                FieldTypes: new Dictionary<string, string>(StringComparer.Ordinal),
                MandatoryFieldNames: [],
                Documentation: null),
            Records: []);

        var result = engine.Select(document, emptyRecordSet, CreateOptions(SelectedIndexes: new HashSet<int> { 0, 1 }, QuickFilter: "anything"));

        Assert.NotNull(result);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void Select_WithOutOfRangeIndexes_ReturnsEmptyRecordSet()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(document, recordSet, CreateOptions(SelectedIndexes: new HashSet<int> { 99, 100 }));

        Assert.NotNull(result);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void Select_WithUnknownProjectedFields_ReturnsRecordsWithNoFields()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "UnknownField" }));

        Assert.NotNull(result);
        Assert.Equal(3, result.Records.Count);
        Assert.All(result.Records, record => Assert.Empty(record.Fields));
    }

    [Fact]
    public void Select_WithWhitespaceQuickFilter_TreatedAsLiteralAndReturnsNoMatches()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(document, recordSet, CreateOptions(QuickFilter: "   "));

        Assert.NotNull(result);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void Select_WithCaseMismatchedQuickFilter_DoesNotMatch()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(document, recordSet, CreateOptions(QuickFilter: "chez"));

        Assert.NotNull(result);
        Assert.Empty(result.Records);
    }

    [Fact]
    public void Select_WithIndexesQuickAndProjection_AppliesCombinedSemantics()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Name" },
                SelectedIndexes: new HashSet<int> { 1, 2 },
                QuickFilter: "Chez"));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Single(result.Records[0].Fields);
        Assert.Equal("Name", result.Records[0].Fields[0].Name);
        Assert.Equal("Ernest Wright", result.Records[0].Fields[0].Value);
    }

    [Fact]
    public void Select_WithIndexesQuickExpressionAndProjection_AppliesCombinedSemantics()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Name" },
                SelectedIndexes: new HashSet<int> { 0, 1, 2 },
                QuickFilter: "example.com",
                Expression: "Name != \"Mandy Nebel\""));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Single(result.Records[0].Fields);
        Assert.Equal("Alfred Nebel", result.Records[0].Fields[0].Value);
    }

    [Fact]
    public void Select_WithSortBySingleField_SortsAscendingByFieldValue()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(SortFields: ["Name"]));

        Assert.NotNull(result);
        Assert.Equal(3, result.Records.Count);
        Assert.Equal("Alfred Nebel", GetFieldValue(result.Records[0], "Name"));
        Assert.Equal("Ernest Wright", GetFieldValue(result.Records[1], "Name"));
        Assert.Equal("Mandy Nebel", GetFieldValue(result.Records[2], "Name"));
    }

    [Fact]
    public void Select_WithSortByMultipleFields_AppliesFieldOrder()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(SortFields: ["Abode", "Name"]));

        Assert.NotNull(result);
        Assert.Equal(3, result.Records.Count);
        Assert.Equal("Alfred Nebel", GetFieldValue(result.Records[0], "Name"));
        Assert.Equal("Mandy Nebel", GetFieldValue(result.Records[1], "Name"));
        Assert.Equal("Ernest Wright", GetFieldValue(result.Records[2], "Name"));
    }

    [Fact]
    public void Select_WithGroupByCountAndSort_SortsGroupedResult()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(
                GroupByFields: ["Abode"],
                Count: true,
                CountFieldName: "Total",
                SortFields: ["Abode"]));

        Assert.NotNull(result);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("42AbbeterWay", GetFieldValue(result.Records[0], "Abode"));
        Assert.Equal("ChezGrampa", GetFieldValue(result.Records[1], "Abode"));
    }

    [Fact]
    public void Select_WithUniq_RemovesDuplicateFieldsByNameAndValuePerRecord()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreateDuplicateFieldDocument();
        var recordSet = document.RecordSets.Single();

        var result = engine.Select(document, recordSet, CreateOptions(Uniq: true));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Equal(3, result.Records[0].Fields.Count);
        Assert.Equal("Name", result.Records[0].Fields[0].Name);
        Assert.Equal("Erik", result.Records[0].Fields[0].Value);
        Assert.Equal("Tag", result.Records[0].Fields[1].Name);
        Assert.Equal("one", result.Records[0].Fields[1].Value);
        Assert.Equal("Tag", result.Records[0].Fields[2].Name);
        Assert.Equal("two", result.Records[0].Fields[2].Value);
    }

    [Fact]
    public void Select_WithUniqAndProjection_DeduplicatesAfterProjection()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreateDuplicateFieldDocument();
        var recordSet = document.RecordSets.Single();

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Tag" },
                Uniq: true));

        Assert.NotNull(result);
        Assert.Single(result.Records);
        Assert.Equal(2, result.Records[0].Fields.Count);
        Assert.Equal("one", result.Records[0].Fields[0].Value);
        Assert.Equal("two", result.Records[0].Fields[1].Value);
    }

    [Fact]
    public void Select_WithIndexesAndSort_AppliesSelectionBeforeSorting()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(
                SelectedIndexes: new HashSet<int> { 0, 1 },
                SortFields: ["Email"]));

        Assert.NotNull(result);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("Alfred Nebel", GetFieldValue(result.Records[0], "Name"));
        Assert.Equal("Mandy Nebel", GetFieldValue(result.Records[1], "Name"));
    }

    [Fact]
    public void Select_WithSortFieldNotProjected_StillSortsBeforeProjection()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(
                SortFields: ["Email"],
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Name" }));

        Assert.NotNull(result);
        Assert.Equal(3, result.Records.Count);
        Assert.Equal("Ernest Wright", GetFieldValue(result.Records[0], "Name"));
        Assert.Equal("Alfred Nebel", GetFieldValue(result.Records[1], "Name"));
        Assert.Equal("Mandy Nebel", GetFieldValue(result.Records[2], "Name"));
    }

    [Fact]
    public void Select_WithGroupByCountAndProjection_AppliesGroupingBeforeProjection()
    {
        var engine = new RecSelectionQueryEngine();
        var document = CreatePeopleAndResidencesDocument();
        var recordSet = document.RecordSets.Single(set => set.TypeName == "Person");

        var result = engine.Select(
            document,
            recordSet,
            CreateOptions(
                GroupByFields: ["Abode"],
                Count: true,
                CountFieldName: "Total",
                SortFields: ["Abode"],
                ProjectedFields: new HashSet<string>(StringComparer.Ordinal) { "Total" }));

        Assert.NotNull(result);
        Assert.Equal(2, result.Records.Count);
        Assert.Equal("2", GetFieldValue(result.Records[0], "Total"));
        Assert.Equal("1", GetFieldValue(result.Records[1], "Total"));
    }

    private static RecSelectionQueryOptions CreateOptions(
        IReadOnlySet<string>? ProjectedFields = null,
        IReadOnlySet<int>? SelectedIndexes = null,
        string? QuickFilter = null,
        string? Expression = null,
        string? JoinField = null,
        IReadOnlyList<string>? GroupByFields = null,
        bool Count = false,
        string CountFieldName = "Count",
        IReadOnlyList<string>? SortFields = null,
        bool Uniq = false)
    {
        return new RecSelectionQueryOptions(
            ProjectedFields,
            SelectedIndexes,
            QuickFilter,
            Expression,
            JoinField,
            GroupByFields,
            Count,
            CountFieldName,
            SortFields,
            Uniq);
    }

    private static RecFileDocument CreatePeopleAndResidencesDocument()
    {
        var personDescriptor = new RecDescriptor(
            Fields: [],
            KeyFieldName: null,
            FieldTypes: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["Abode"] = "rec Residence"
            },
            MandatoryFieldNames: [],
            Documentation: null);

        var residenceDescriptor = new RecDescriptor(
            Fields: [],
            KeyFieldName: "Id",
            FieldTypes: new Dictionary<string, string>(StringComparer.Ordinal),
            MandatoryFieldNames: [],
            Documentation: null);

        var personRecordSet = new RecRecordSet(
            TypeName: "Person",
            Descriptor: personDescriptor,
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

        var residenceRecordSet = new RecRecordSet(
            TypeName: "Residence",
            Descriptor: residenceDescriptor,
            Records:
            [
                new RecRecord([
                    new RecField("Id", "42AbbeterWay"),
                    new RecField("Address", "42 Abbeter Way, Inprooving, WORCS"),
                    new RecField("Telephone", "01234 5676789")]),
                new RecRecord([
                    new RecField("Id", "ChezGrampa"),
                    new RecField("Address", "1 Wanter Rise, Greater Inncombe, BUCKS")])
            ]);

        return new RecFileDocument([], [], [personRecordSet, residenceRecordSet]);
    }

    private static RecFileDocument CreateDuplicateFieldDocument()
    {
        var descriptor = new RecDescriptor(
            Fields: [],
            KeyFieldName: null,
            FieldTypes: new Dictionary<string, string>(StringComparer.Ordinal),
            MandatoryFieldNames: [],
            Documentation: null);

        var recordSet = new RecRecordSet(
            TypeName: "User",
            Descriptor: descriptor,
            Records:
            [
                new RecRecord([
                    new RecField("Name", "Erik"),
                    new RecField("Name", "Erik"),
                    new RecField("Tag", "one"),
                    new RecField("Tag", "one"),
                    new RecField("Tag", "two")])
            ]);

        return new RecFileDocument([], [], [recordSet]);
    }

    private static string? GetFieldValue(RecRecord record, string fieldName)
    {
        return record.Fields.FirstOrDefault(field => field.Name == fieldName)?.Value;
    }
}
