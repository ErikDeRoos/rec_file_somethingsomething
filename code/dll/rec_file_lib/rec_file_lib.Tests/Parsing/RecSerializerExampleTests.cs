using rec_file_lib.Parsing;
using rec_file_lib.Tests.ExampleData;

namespace rec_file_lib.Tests.Parsing;

public sealed class RecSerializerExampleTests
{
    [Fact]
    public void Serialize_SimpleSingleFile_RoundTripsToOriginalText()
    {
        var parser = new RecParser();
        var serializer = new RecSerializer();
        var originalText = RecExampleData.ReadAllText(RecExampleScenario.SimpleSingleFile);

        var document = parser.Parse(originalText);
        var serializedText = serializer.Serialize(document);

        Assert.Equal(NormalizeForComparison(originalText), NormalizeForComparison(serializedText));
    }

    [Fact]
    public void Serialize_SimpleSingleFile_UsingTextReaderAndTextWriter_RoundTripsToOriginalText()
    {
        var parser = new RecParser();
        var serializer = new RecSerializer();
        using var reader = RecExampleData.OpenText(RecExampleScenario.SimpleSingleFile);
        using var writer = new StringWriter();

        var document = parser.Parse(reader);
        serializer.Serialize(writer, document);

        var originalText = RecExampleData.ReadAllText(RecExampleScenario.SimpleSingleFile);
        Assert.Equal(NormalizeForComparison(originalText), NormalizeForComparison(writer.ToString()));
    }

    [Fact]
    public void Serialize_SimpleRecutilsBookExample_RoundTripsToOriginalText()
    {
        var parser = new RecParser();
        var serializer = new RecSerializer();
        var originalText = RecExampleData.ReadAllText(RecExampleScenario.SimpleRecutilsBookExample);

        var document = parser.Parse(originalText);
        var serializedText = serializer.Serialize(document);

        Assert.Equal(NormalizeForComparison(originalText), NormalizeForComparison(serializedText));
    }

    [Fact]
    public void Serialize_SimpleRecutilsBookExample_UsingTextReaderAndTextWriter_RoundTripsToOriginalText()
    {
        var parser = new RecParser();
        var serializer = new RecSerializer();
        using var reader = RecExampleData.OpenText(RecExampleScenario.SimpleRecutilsBookExample);
        using var writer = new StringWriter();

        var document = parser.Parse(reader);
        serializer.Serialize(writer, document);

        var originalText = RecExampleData.ReadAllText(RecExampleScenario.SimpleRecutilsBookExample);
        Assert.Equal(NormalizeForComparison(originalText), NormalizeForComparison(writer.ToString()));
    }

    private static string NormalizeForComparison(string text)
    {
        var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal);
        var lines = normalized.Split('\n');
        return string.Join("\n", lines.Select(static line => line.TrimStart())).TrimEnd('\n');
    }
}
