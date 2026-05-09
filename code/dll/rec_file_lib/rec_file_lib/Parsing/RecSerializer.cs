using System.IO;
using rec_file_lib.Model;

namespace rec_file_lib.Parsing;

internal sealed class RecSerializer
{
    public string Serialize(RecFileDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        using var writer = new StringWriter();
        Serialize(writer, document);
        return writer.ToString();
    }

    public void Serialize(TextWriter writer, RecFileDocument document)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(document);

        var wroteSection = false;

        foreach (var commentLine in document.LeadingCommentLines)
        {
            writer.WriteLine(commentLine);
            wroteSection = true;
        }

        foreach (var recordSet in document.RecordSets)
        {
            if (wroteSection)
            {
                writer.WriteLine();
            }

            WriteRecordSet(writer, recordSet);
            wroteSection = true;
        }

        foreach (var trailingComment in document.TrailingCommentLines)
        {
            if (wroteSection)
            {
                writer.WriteLine();
            }

            writer.WriteLine(trailingComment);
            wroteSection = true;
        }
    }

    private static void WriteRecordSet(TextWriter writer, RecRecordSet recordSet)
    {
        if (!string.IsNullOrWhiteSpace(recordSet.TypeName))
        {
            writer.WriteLine($"%rec: {recordSet.TypeName}");
        }

        foreach (var descriptorField in recordSet.Descriptor.Fields)
        {
            WriteField(writer, descriptorField);
        }

        if (!string.IsNullOrWhiteSpace(recordSet.TypeName) || recordSet.Descriptor.Fields.Count > 0)
        {
            writer.WriteLine();
        }

        for (var recordIndex = 0; recordIndex < recordSet.Records.Count; recordIndex++)
        {
            if (recordIndex > 0)
            {
                writer.WriteLine();
            }

            WriteRecord(writer, recordSet.Records[recordIndex]);
        }
    }

    private static void WriteRecord(TextWriter writer, RecRecord record)
    {
        foreach (var field in record.Fields)
        {
            WriteField(writer, field);
        }
    }

    private static void WriteField(TextWriter writer, RecField field)
    {
        var lines = field.Value.Split('\n');
        if (lines.Length == 1)
        {
            writer.WriteLine($"{field.Name}: {lines[0]}");
            return;
        }

        if (lines[0].Length == 0)
        {
            writer.WriteLine($"{field.Name}:");
        }
        else
        {
            writer.WriteLine($"{field.Name}: {lines[0]}");
        }

        for (var lineIndex = 1; lineIndex < lines.Length; lineIndex++)
        {
            writer.WriteLine($"+ {lines[lineIndex]}");
        }
    }
}
