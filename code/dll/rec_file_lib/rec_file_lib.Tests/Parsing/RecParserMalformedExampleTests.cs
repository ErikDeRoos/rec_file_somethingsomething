using rec_file_lib.Parsing;
using rec_file_lib.Tests.ExampleData;

namespace rec_file_lib.Tests.Parsing;

public sealed class RecParserMalformedExampleTests
{
    [Fact]
    public void Parse_WrongMissingMandatoryField_ParsesStructureForLaterValidation()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.WrongMissingMandatoryField);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal("User", recordSet.TypeName);
        Assert.Equal(new[] { "Name" }, recordSet.Descriptor.MandatoryFieldNames);
        Assert.Equal(2, recordSet.Records.Count);
        Assert.DoesNotContain(recordSet.Records[1].Fields, field => field.Name == "Name");
    }

    [Fact]
    public void Parse_WrongDuplicateKeyValue_ParsesStructureForLaterValidation()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.WrongDuplicateKeyValue);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal("Item", recordSet.TypeName);
        Assert.Equal("Id", recordSet.Descriptor.KeyFieldName);
        Assert.Equal(2, recordSet.Records.Count);
        Assert.Equal("1", recordSet.Records[0].Fields.Single(field => field.Name == "Id").Value);
        Assert.Equal("1", recordSet.Records[1].Fields.Single(field => field.Name == "Id").Value);
    }

    [Fact]
    public void Parse_WrongInvalidFieldType_ParsesStructureForLaterValidation()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.WrongInvalidFieldType);

        var document = parser.Parse(text);

        var recordSet = Assert.Single(document.RecordSets);
        Assert.Equal("Task", recordSet.TypeName);
        Assert.Equal("enum open closed", recordSet.Descriptor.FieldTypes["Status"]);
        Assert.Equal("pending", recordSet.Records[0].Fields.Single(field => field.Name == "Status").Value);
    }

    [Fact]
    public void Parse_WrongBadMultilineContinuation_ThrowsFormatException()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.WrongBadMultilineContinuation);

        var exception = Assert.Throws<FormatException>(() => parser.Parse(text));

        Assert.Equal("Continuation line found without a current field.", exception.Message);
    }

    [Fact]
    public void Parse_WrongMissingFieldSeparator_ThrowsFormatException()
    {
        var parser = new RecParser();
        var text = RecExampleData.ReadAllText(RecExampleScenario.WrongMissingFieldSeparator);

        var exception = Assert.Throws<FormatException>(() => parser.Parse(text));

        Assert.Equal("Invalid field line: %key Id", exception.Message);
    }
}
