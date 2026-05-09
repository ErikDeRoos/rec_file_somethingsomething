using System.IO;
using rec_file_lib.Model;
using rec_file_lib.Model.Builders;

namespace rec_file_lib.Parsing;

internal sealed class RecParser
{
    public RecFileDocument Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        using var reader = new StringReader(text);
        return Parse(reader);
    }

    public RecFileDocument Parse(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var document = new RecFileDocumentBuilder();
        RecRecordSetBuilder? currentRecordSet = null;
        RecRecordBuilder? currentRecord = null;
        RecFieldBuilder? currentField = null;
        var hasSeenRecordData = false;

        string? rawLine;
        while ((rawLine = reader.ReadLine()) is not null)
        {
            var line = rawLine.TrimStart();
            var lineKind = InspectLine(line);

            switch (lineKind)
            {
                case RecLineKind.Blank:
                    currentRecord = null;
                    currentField = null;
                    continue;

                case RecLineKind.Comment:
                    document.AddCommentLine(line, isTrailing: hasSeenRecordData);
                    currentField = null;
                    continue;

                case RecLineKind.Continuation:
                    if (currentField is null)
                    {
                        throw new FormatException("Continuation line found without a current field.");
                    }

                    currentField.AppendContinuation(ReadContinuationValue(line));
                    continue;

                case RecLineKind.Field:
                    var (fieldName, fieldValue) = ParseField(line);

                    if (string.Equals(fieldName, "%rec", StringComparison.Ordinal))
                    {
                        currentRecordSet = new RecRecordSetBuilder { TypeName = fieldValue };
                        document.AddRecordSet(currentRecordSet);
                        currentRecord = null;
                        currentField = null;
                        continue;
                    }

                    if (fieldName.StartsWith("%", StringComparison.Ordinal))
                    {
                        currentRecordSet ??= CreateAnonymousRecordSet(document);
                        currentField = new RecFieldBuilder(fieldName, fieldValue);
                        currentRecordSet.Descriptor.AddField(currentField);
                        currentRecord = null;
                        continue;
                    }

                    currentRecordSet ??= CreateAnonymousRecordSet(document);
                    currentRecord ??= CreateRecord(currentRecordSet);

                    currentField = new RecFieldBuilder(fieldName, fieldValue);
                    currentRecord.AddField(currentField);
                    hasSeenRecordData = true;
                    continue;

                default:
                    throw new InvalidOperationException($"Unsupported line kind: {lineKind}");
            }
        }

        return document.Build();
    }

    private static RecLineKind InspectLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return RecLineKind.Blank;
        }

        return line[0] switch
        {
            '#' => RecLineKind.Comment,
            '+' => RecLineKind.Continuation,
            _ => RecLineKind.Field,
        };
    }

    private static string ReadContinuationValue(string line)
    {
        return line.Length > 1 && line[1] == ' '
            ? line[2..]
            : line[1..];
    }

    private static (string FieldName, string FieldValue) ParseField(string line)
    {
        var separatorIndex = line.IndexOf(':');
        if (separatorIndex < 0)
        {
            throw new FormatException($"Invalid field line: {line}");
        }

        var fieldName = line[..separatorIndex].Trim();
        var fieldValue = separatorIndex == line.Length - 1 ? string.Empty : line[(separatorIndex + 1)..].TrimStart();
        return (fieldName, fieldValue);
    }

    private static RecRecordSetBuilder CreateAnonymousRecordSet(RecFileDocumentBuilder document)
    {
        var recordSet = new RecRecordSetBuilder();
        document.AddRecordSet(recordSet);
        return recordSet;
    }

    private static RecRecordBuilder CreateRecord(RecRecordSetBuilder recordSet)
    {
        var record = new RecRecordBuilder();
        recordSet.AddRecord(record);
        return record;
    }

    private enum RecLineKind
    {
        Blank,
        Comment,
        Continuation,
        Field,
    }
}
