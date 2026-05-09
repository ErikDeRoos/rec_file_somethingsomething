using rec_file_lib.Model;
using rec_file_lib.Model.Builders;

namespace rec_file_lib.DirectFileServer;

internal sealed class DirectRecMutationParser
{
    public RecRecord ParseRecord(string recordText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recordText);

        using var reader = new StringReader(recordText);
        var record = new RecRecordBuilder();
        RecFieldBuilder? currentField = null;
        string? rawLine;

        while ((rawLine = reader.ReadLine()) is not null)
        {
            var line = rawLine.TrimStart();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (line.StartsWith("+", StringComparison.Ordinal))
            {
                if (currentField is null)
                {
                    throw new FormatException("Continuation line found without a current field.");
                }

                var continuationValue = line.Length > 1 && line[1] == ' '
                    ? line[2..]
                    : line[1..];
                currentField.AppendContinuation(continuationValue);
                continue;
            }

            var separatorIndex = line.IndexOf(':');
            if (separatorIndex < 0)
            {
                throw new FormatException($"Invalid field line: {line}");
            }

            var fieldName = line[..separatorIndex].Trim();
            if (fieldName.StartsWith("%", StringComparison.Ordinal))
            {
                throw new FormatException($"Record text cannot contain descriptor field '{fieldName}'.");
            }

            var fieldValue = separatorIndex == line.Length - 1 ? string.Empty : line[(separatorIndex + 1)..].TrimStart();
            currentField = new RecFieldBuilder(fieldName, fieldValue);
            record.AddField(currentField);
        }

        var builtRecord = record.Build();
        if (builtRecord.Fields.Count == 0)
        {
            throw new FormatException("Record text does not contain any fields.");
        }

        return builtRecord;
    }
}
