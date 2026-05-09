using System.IO;
using rec_file_lib.Model;

namespace rec_file_lib.DirectFileServer;

internal sealed class DirectRecSelFormatter
{
    public string FormatSelection(RecFileDocument document, bool collapse = false, bool includeDescriptors = false)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (document.RecordSets.Count == 0)
        {
            return string.Empty;
        }

        if (document.RecordSets.Count > 1)
        {
            throw new InvalidOperationException("several record types found. Please use -t to specify one.");
        }

        return FormatRecordSet(document.RecordSets[0], collapse, includeDescriptors);
    }

    public string FormatRecordSet(RecRecordSet? recordSet, bool collapse = false, bool includeDescriptors = false)
    {
        if (recordSet is null)
        {
            return string.Empty;
        }

        using var writer = new StringWriter();

        if (includeDescriptors)
        {
            WriteDescriptor(writer, recordSet);
            if (recordSet.Records.Count > 0)
            {
                writer.WriteLine();
            }
        }

        WriteRecordSetRecords(writer, recordSet, collapse);
        return writer.ToString().TrimEnd('\r', '\n');
    }

    private static void WriteDescriptor(TextWriter writer, RecRecordSet recordSet)
    {
        if (!string.IsNullOrWhiteSpace(recordSet.TypeName))
        {
            writer.WriteLine($"%rec: {recordSet.TypeName}");
        }

        foreach (var descriptorField in recordSet.Descriptor.Fields)
        {
            WriteField(writer, descriptorField);
        }
    }

    private static void WriteRecordSetRecords(TextWriter writer, RecRecordSet recordSet, bool collapse)
    {
        for (var recordIndex = 0; recordIndex < recordSet.Records.Count; recordIndex++)
        {
            if (!collapse && recordIndex > 0)
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
