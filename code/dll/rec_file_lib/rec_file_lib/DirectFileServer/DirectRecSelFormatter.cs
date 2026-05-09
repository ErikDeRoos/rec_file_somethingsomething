using System.IO;
using rec_file_lib.Model;

namespace rec_file_lib.DirectFileServer;

internal sealed class DirectRecSelFormatter
{
    public string FormatSelection(RecFileDocument document)
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

        return FormatRecordSet(document.RecordSets[0]);
    }

    public string FormatRecordSet(RecRecordSet? recordSet)
    {
        if (recordSet is null)
        {
            return string.Empty;
        }

        using var writer = new StringWriter();
        WriteRecordSetRecords(writer, recordSet);
        return writer.ToString().TrimEnd('\r', '\n');
    }

    private static void WriteRecordSetRecords(TextWriter writer, RecRecordSet recordSet)
    {
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
