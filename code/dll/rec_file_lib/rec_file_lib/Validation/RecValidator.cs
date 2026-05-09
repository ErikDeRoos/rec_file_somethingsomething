using rec_file_lib.Model;

namespace rec_file_lib.Validation;

internal sealed class RecValidator
{
    public void EnsureValid(RecFileDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var errors = Validate(document);
        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
    }

    public IReadOnlyList<string> Validate(RecFileDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var errors = new List<string>();
        foreach (var recordSet in document.RecordSets)
        {
            ValidateRecordSet(document, recordSet, errors);
        }

        return errors;
    }

    private static void ValidateRecordSet(RecFileDocument document, RecRecordSet recordSet, List<string> errors)
    {
        ValidateMandatoryFields(recordSet, errors);
        ValidateKeyField(recordSet, errors);
        ValidateFieldTypes(document, recordSet, errors);
    }

    private static void ValidateMandatoryFields(RecRecordSet recordSet, List<string> errors)
    {
        foreach (var mandatoryFieldName in recordSet.Descriptor.MandatoryFieldNames)
        {
            for (var recordIndex = 0; recordIndex < recordSet.Records.Count; recordIndex++)
            {
                if (recordSet.Records[recordIndex].Fields.Any(field => string.Equals(field.Name, mandatoryFieldName, StringComparison.Ordinal)))
                {
                    continue;
                }

                errors.Add($"Record set '{recordSet.TypeName ?? string.Empty}' record {recordIndex} is missing mandatory field '{mandatoryFieldName}'.");
            }
        }
    }

    private static void ValidateKeyField(RecRecordSet recordSet, List<string> errors)
    {
        if (string.IsNullOrEmpty(recordSet.Descriptor.KeyFieldName))
        {
            return;
        }

        var keyFieldName = recordSet.Descriptor.KeyFieldName;
        var seenValues = new Dictionary<string, int>(StringComparer.Ordinal);

        for (var recordIndex = 0; recordIndex < recordSet.Records.Count; recordIndex++)
        {
            var keyFields = recordSet.Records[recordIndex].Fields
                .Where(field => string.Equals(field.Name, keyFieldName, StringComparison.Ordinal))
                .ToArray();

            if (keyFields.Length == 0)
            {
                errors.Add($"Record set '{recordSet.TypeName ?? string.Empty}' record {recordIndex} is missing key field '{keyFieldName}'.");
                continue;
            }

            if (keyFields.Length > 1)
            {
                errors.Add($"Record set '{recordSet.TypeName ?? string.Empty}' record {recordIndex} has duplicate key field '{keyFieldName}'.");
                continue;
            }

            var keyValue = keyFields[0].Value;
            if (seenValues.TryGetValue(keyValue, out var existingRecordIndex))
            {
                errors.Add($"Record set '{recordSet.TypeName ?? string.Empty}' has duplicate key value '{keyValue}' in records {existingRecordIndex} and {recordIndex}.");
                continue;
            }

            seenValues[keyValue] = recordIndex;
        }
    }

    private static void ValidateFieldTypes(RecFileDocument document, RecRecordSet recordSet, List<string> errors)
    {
        foreach (var fieldType in recordSet.Descriptor.FieldTypes)
        {
            for (var recordIndex = 0; recordIndex < recordSet.Records.Count; recordIndex++)
            {
                var fields = recordSet.Records[recordIndex].Fields
                    .Where(field => string.Equals(field.Name, fieldType.Key, StringComparison.Ordinal));

                foreach (var field in fields)
                {
                    ValidateFieldType(document, recordSet, recordIndex, field, fieldType.Value, errors);
                }
            }
        }
    }

    private static void ValidateFieldType(RecFileDocument document, RecRecordSet recordSet, int recordIndex, RecField field, string fieldType, List<string> errors)
    {
        if (string.Equals(fieldType, "line", StringComparison.Ordinal))
        {
            if (field.Value.Contains('\n', StringComparison.Ordinal))
            {
                errors.Add($"Record set '{recordSet.TypeName ?? string.Empty}' record {recordIndex} field '{field.Name}' violates type 'line'.");
            }

            return;
        }

        if (fieldType.StartsWith("enum ", StringComparison.Ordinal))
        {
            var allowedValues = fieldType[5..]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!allowedValues.Contains(field.Value, StringComparer.Ordinal))
            {
                errors.Add($"Record set '{recordSet.TypeName ?? string.Empty}' record {recordIndex} field '{field.Name}' has invalid enum value '{field.Value}'.");
            }

            return;
        }

        if (fieldType.StartsWith("rec ", StringComparison.Ordinal))
        {
            var targetType = fieldType[4..];
            var targetRecordSet = document.RecordSets.FirstOrDefault(recordSet => string.Equals(recordSet.TypeName, targetType, StringComparison.Ordinal));
            if (targetRecordSet is null || string.IsNullOrEmpty(targetRecordSet.Descriptor.KeyFieldName))
            {
                errors.Add($"Record set '{recordSet.TypeName ?? string.Empty}' field '{field.Name}' references missing or non-keyed record set '{targetType}'.");
                return;
            }

            var keyFieldName = targetRecordSet.Descriptor.KeyFieldName;
            var knownKeyValues = targetRecordSet.Records
                .SelectMany(record => record.Fields.Where(candidate => string.Equals(candidate.Name, keyFieldName, StringComparison.Ordinal)))
                .Select(candidate => candidate.Value)
                .ToHashSet(StringComparer.Ordinal);

            if (!knownKeyValues.Contains(field.Value))
            {
                errors.Add($"Record set '{recordSet.TypeName ?? string.Empty}' record {recordIndex} field '{field.Name}' references unknown key '{field.Value}' in '{targetType}'.");
            }
        }
    }
}
