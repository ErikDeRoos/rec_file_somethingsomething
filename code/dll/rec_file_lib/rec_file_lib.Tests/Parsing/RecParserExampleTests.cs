using rec_file_lib.Parsing;
using rec_file_lib.Tests.ExampleData;

namespace rec_file_lib.Tests.Parsing;

public sealed class RecParserExampleTests
{
    [Fact]
    public void Parse_SimpleSingleFile_ReadsCommentAndRecordSetType()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.SimpleSingleFile);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal(new[] { "# first example" }, document.CommentLines);
        Assert.Equal("User", recordSet.TypeName);
        Assert.Null(recordSet.Descriptor.KeyFieldName);
        Assert.Empty(recordSet.Descriptor.FieldTypes);
    }

    [Fact]
    public void Parse_SimpleSingleFile_ReadsTwoRecords()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.SimpleSingleFile);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal(2, recordSet.Records.Count);
        Assert.Equal("Erik", recordSet.Records[0].Fields[0].Value);
        Assert.Equal("Buddy", recordSet.Records[1].Fields[0].Value);
    }

    [Fact]
    public void Parse_SimpleSingleFile_ReadsMultilineFieldValues()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.SimpleSingleFile);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        var erikNotes = recordSet.Records[0].Fields.Single(field => field.Name == "Notes");
        var buddyNotes = recordSet.Records[1].Fields.Single(field => field.Name == "Notes");

        Assert.Equal(
            "\nThinks 'Inventing a new wheel is the best thing to do ever'.\n\nSome x% of the people agree with that...\nBut does it matter? No percentage how little of how big, can be used to (by percentage alone) decide on its usefullness.",
            erikNotes.Value);
        Assert.Equal("\nLoyal dog.\nAlso goes by the name of 'Cuty-pie'.", buddyNotes.Value);
    }

    [Fact]
    public void Parse_SimpleRecutilsBookExample_ReadsCommentAndRecordSetType()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.SimpleRecutilsBookExample);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal(new[] { "# -*- mode: rec -*-", "# End of books.rec" }, document.CommentLines);
        Assert.Equal("Book", recordSet.TypeName);
    }

    [Fact]
    public void Parse_SimpleRecutilsBookExample_ReadsDescriptorFields()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.SimpleRecutilsBookExample);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal(new[] { "Title" }, recordSet.Descriptor.MandatoryFieldNames);
        Assert.Equal("enum loaned home unknown", recordSet.Descriptor.FieldTypes["Location"]);
        Assert.Equal("\nA book in my personal collection.", recordSet.Descriptor.Documentation);
    }

    [Fact]
    public void Parse_SimpleRecutilsBookExample_ReadsFiveRecords()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.SimpleRecutilsBookExample);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal(5, recordSet.Records.Count);
        Assert.Equal("GNU Emacs Manual", recordSet.Records[0].Fields[0].Value);
        Assert.Equal("Yeelong User Manual", recordSet.Records[4].Fields[0].Value);
    }

    [Fact]
    public void Parse_SimpleRecutilsBookExample_ReadsRepeatedAndTypedFields()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.SimpleRecutilsBookExample);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        var fourthRecord = recordSet.Records[3];
        var authors = fourthRecord.Fields.Where(field => field.Name == "Author").Select(field => field.Value).ToArray();
        var location = fourthRecord.Fields.Single(field => field.Name == "Location");

        Assert.Equal(new[] { "Nacho Gonzalez", "Jose E. Marchesi" }, authors);
        Assert.Equal("unknown", location.Value);
    }

    [Fact]
    public void Parse_MultipleRecordTypesSingleFile_ReadsTwoTypedRecordSets()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.MultipleRecordTypesSingleFile);

        var document = parser.Parse(text);

        Assert.Equal(new[] { "# multiple record types in a single file" }, document.CommentLines);
        Assert.Equal(2, document.RecordSets.Count);
        Assert.Equal("Person", document.RecordSets[0].TypeName);
        Assert.Equal("Residence", document.RecordSets[1].TypeName);
    }

    [Fact]
    public void Parse_MultipleRecordTypesSingleFile_ReadsDescriptorAndForeignKeyStyleFieldType()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.MultipleRecordTypesSingleFile);

        var document = parser.Parse(text);

        var personRecordSet = document.RecordSets[0];
        var residenceRecordSet = document.RecordSets[1];

        Assert.Equal("rec Residence", personRecordSet.Descriptor.FieldTypes["Abode"]);
        Assert.Equal("Id", residenceRecordSet.Descriptor.KeyFieldName);
    }

    [Fact]
    public void Parse_MultipleRecordTypesSingleFile_ReadsRecordsAcrossBothRecordSets()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.MultipleRecordTypesSingleFile);

        var document = parser.Parse(text);

        var personRecordSet = document.RecordSets[0];
        var residenceRecordSet = document.RecordSets[1];

        Assert.Equal(3, personRecordSet.Records.Count);
        Assert.Equal(2, residenceRecordSet.Records.Count);
        Assert.Equal("42AbbeterWay", personRecordSet.Records[0].Fields.Single(field => field.Name == "Abode").Value);
        Assert.Equal("ChezGrampa", residenceRecordSet.Records[1].Fields.Single(field => field.Name == "Id").Value);
    }

    [Fact]
    public void Parse_IntFieldType_ReadsIntFieldTypeAndMultipleIntegerFormats()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.IntFieldType);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal("Task", recordSet.TypeName);
        Assert.Equal("int", recordSet.Descriptor.FieldTypes["Priority"]);
        Assert.Equal(4, recordSet.Records.Count);
        Assert.Equal("1", recordSet.Records[0].Fields.Single(field => field.Name == "Priority").Value);
        Assert.Equal("-23", recordSet.Records[1].Fields.Single(field => field.Name == "Priority").Value);
        Assert.Equal("0x10", recordSet.Records[2].Fields.Single(field => field.Name == "Priority").Value);
        Assert.Equal("020", recordSet.Records[3].Fields.Single(field => field.Name == "Priority").Value);
    }

    [Fact]
    public void Parse_RepeatedFieldsBacktracking_ReadsRepeatedTagFields()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.RepeatedFieldsBacktracking);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal("Entry", recordSet.TypeName);
        Assert.Equal(3, recordSet.Records.Count);

        var firstRecordTags = recordSet.Records[0].Fields
            .Where(field => field.Name == "Tag")
            .Select(field => field.Value)
            .ToArray();

        Assert.Equal(new[] { "red", "blue" }, firstRecordTags);
    }

    [Fact]
    public void Parse_RepeatedFieldsBacktracking_ReadsRepeatedTagFieldsAsValidData()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.RepeatedFieldsBacktracking);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal("Entry", recordSet.TypeName);
        Assert.Equal(3, recordSet.Records.Count);

        var firstTags = recordSet.Records[0].Fields
            .Where(field => field.Name == "Tag")
            .Select(field => field.Value)
            .ToArray();

        Assert.Equal(new[] { "red", "blue" }, firstTags);
    }
}
