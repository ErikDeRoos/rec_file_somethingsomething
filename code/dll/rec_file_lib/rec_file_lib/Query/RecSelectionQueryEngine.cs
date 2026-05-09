using rec_file_lib.Model;

namespace rec_file_lib.Query;

internal sealed class RecSelectionQueryEngine
{
    public RecRecordSet? Select(RecFileDocument document, RecRecordSet? recordSet, RecSelectionQueryOptions options)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(options);

        if (recordSet is null)
        {
            return null;
        }

        var expression = RecSelectionExpressionParser.Parse(options.Expression);
        var sourceRecordSet = ApplyJoinIfRequested(document, recordSet, options.JoinField);
        var filteredRecords = new List<RecRecord>();

        for (var recordIndex = 0; recordIndex < sourceRecordSet.Records.Count; recordIndex++)
        {
            if (options.SelectedIndexes is not null && !options.SelectedIndexes.Contains(recordIndex))
            {
                continue;
            }

            var record = sourceRecordSet.Records[recordIndex];
            if (!MatchesQuickFilter(record, options.QuickFilter))
            {
                continue;
            }

            if (!MatchesExpression(record, expression))
            {
                continue;
            }

            filteredRecords.Add(record);
        }

        var groupedRecords = ApplyGroupingAndCount(filteredRecords, options.GroupByFields, options.Count, options.CountFieldName);
        var sortedRecords = ApplySorting(groupedRecords, options.SortFields);

        var projectedRecords = sortedRecords
            .Select(record => ProjectRecord(record, options.ProjectedFields))
            .ToArray();

        var uniqRecords = options.Uniq
            ? projectedRecords.Select(DeduplicateRecordFields).ToArray()
            : projectedRecords;

        return sourceRecordSet with { Records = uniqRecords };
    }

    private static IReadOnlyList<RecRecord> ApplyGroupingAndCount(
        IReadOnlyList<RecRecord> records,
        IReadOnlyList<string>? groupByFields,
        bool count,
        string countFieldName)
    {
        if (groupByFields is null || groupByFields.Count == 0)
        {
            if (!count)
            {
                return records;
            }

            var singleCountRecord = new RecRecord([
                new RecField(countFieldName, records.Count.ToString(System.Globalization.CultureInfo.InvariantCulture))
            ]);
            return [singleCountRecord];
        }

        var groups = new Dictionary<string, List<RecRecord>>(StringComparer.Ordinal);
        var groupValuesByKey = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var record in records)
        {
            var values = groupByFields
                .Select(fieldName => TryGetFieldValue(record, fieldName) ?? string.Empty)
                .ToArray();

            var key = string.Join("\u001F", values);
            if (!groups.TryGetValue(key, out var bucket))
            {
                bucket = [];
                groups[key] = bucket;
                groupValuesByKey[key] = values;
            }

            bucket.Add(record);
        }

        var grouped = new List<RecRecord>(groups.Count);
        foreach (var key in groups.Keys)
        {
            var values = groupValuesByKey[key];
            var bucket = groups[key];

            var fields = new List<RecField>(groupByFields.Count + (count ? 1 : 0));
            for (var index = 0; index < groupByFields.Count; index++)
            {
                fields.Add(new RecField(groupByFields[index], values[index]));
            }

            if (count)
            {
                fields.Add(new RecField(countFieldName, bucket.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            }

            grouped.Add(new RecRecord(fields.ToArray()));
        }

        return grouped;
    }

    private static IReadOnlyList<RecRecord> ApplySorting(IReadOnlyList<RecRecord> records, IReadOnlyList<string>? sortFields)
    {
        if (sortFields is null || sortFields.Count == 0 || records.Count <= 1)
        {
            return records;
        }

        var indexed = records
            .Select((record, index) => new { Record = record, Index = index })
            .ToList();

        indexed.Sort((left, right) => CompareBySortFields(left.Record, right.Record, sortFields, left.Index, right.Index));

        return indexed.Select(item => item.Record).ToArray();
    }

    private static int CompareBySortFields(
        RecRecord left,
        RecRecord right,
        IReadOnlyList<string> sortFields,
        int leftOriginalIndex,
        int rightOriginalIndex)
    {
        foreach (var sortField in sortFields)
        {
            var leftValue = TryGetFieldValue(left, sortField) ?? string.Empty;
            var rightValue = TryGetFieldValue(right, sortField) ?? string.Empty;

            var compare = string.CompareOrdinal(leftValue, rightValue);
            if (compare != 0)
            {
                return compare;
            }
        }

        return leftOriginalIndex.CompareTo(rightOriginalIndex);
    }

    private static RecRecord DeduplicateRecordFields(RecRecord record)
    {
        var uniqueFields = new List<RecField>(record.Fields.Count);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var field in record.Fields)
        {
            var key = $"{field.Name}\u001F{field.Value}";
            if (seen.Add(key))
            {
                uniqueFields.Add(field);
            }
        }

        if (uniqueFields.Count == record.Fields.Count)
        {
            return record;
        }

        return record with { Fields = uniqueFields.ToArray() };
    }

    private static RecRecordSet ApplyJoinIfRequested(RecFileDocument document, RecRecordSet sourceRecordSet, string? joinField)
    {
        if (string.IsNullOrEmpty(joinField))
        {
            return sourceRecordSet;
        }

        if (!sourceRecordSet.Descriptor.FieldTypes.TryGetValue(joinField, out var fieldTypeDeclaration)
            || string.IsNullOrWhiteSpace(fieldTypeDeclaration))
        {
            throw new InvalidOperationException($"join field '{joinField}' is not declared with a rec type.");
        }

        var declaration = fieldTypeDeclaration.Trim();
        const string recPrefix = "rec ";
        if (!declaration.StartsWith(recPrefix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"join field '{joinField}' is not declared with a rec type.");
        }

        var targetType = declaration[recPrefix.Length..].Trim();
        if (targetType.Length == 0)
        {
            throw new InvalidOperationException($"join field '{joinField}' is not declared with a rec type.");
        }

        var targetRecordSet = document.RecordSets
            .FirstOrDefault(set => string.Equals(set.TypeName, targetType, StringComparison.Ordinal));

        if (targetRecordSet is null)
        {
            throw new InvalidOperationException($"join target record set '{targetType}' not found.");
        }

        var targetKeyFieldName = targetRecordSet.Descriptor.KeyFieldName;
        if (string.IsNullOrEmpty(targetKeyFieldName))
        {
            throw new InvalidOperationException($"join target record set '{targetType}' does not declare a key field.");
        }

        var targetByKey = targetRecordSet.Records
            .Select(record => new { Record = record, Key = TryGetFieldValue(record, targetKeyFieldName) })
            .Where(item => !string.IsNullOrEmpty(item.Key))
            .ToDictionary(item => item.Key!, item => item.Record, StringComparer.Ordinal);

        var joinedRecords = new List<RecRecord>();
        foreach (var sourceRecord in sourceRecordSet.Records)
        {
            var foreignKeyValue = TryGetFieldValue(sourceRecord, joinField);
            if (string.IsNullOrEmpty(foreignKeyValue))
            {
                continue;
            }

            if (!targetByKey.TryGetValue(foreignKeyValue, out var targetRecord))
            {
                continue;
            }

            joinedRecords.Add(JoinRecords(sourceRecord, targetType, targetRecord));
        }

        return sourceRecordSet with { Records = joinedRecords.ToArray() };
    }

    private static string? TryGetFieldValue(RecRecord record, string fieldName)
    {
        return record.Fields
            .FirstOrDefault(field => string.Equals(field.Name, fieldName, StringComparison.Ordinal))
            ?.Value;
    }

    private static RecRecord JoinRecords(RecRecord sourceRecord, string targetType, RecRecord targetRecord)
    {
        var joinedFields = new List<RecField>(sourceRecord.Fields.Count + targetRecord.Fields.Count);
        joinedFields.AddRange(sourceRecord.Fields);

        foreach (var targetField in targetRecord.Fields)
        {
            joinedFields.Add(new RecField($"{targetType}.{targetField.Name}", targetField.Value));
        }

        return sourceRecord with { Fields = joinedFields.ToArray() };
    }

    private static bool MatchesQuickFilter(RecRecord record, string? quickFilter)
    {
        if (string.IsNullOrEmpty(quickFilter))
        {
            return true;
        }

        foreach (var field in record.Fields)
        {
            if (field.Value.Contains(quickFilter, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesExpression(RecRecord record, RecSelectionExpression? expression)
    {
        if (expression is null)
        {
            return true;
        }

        foreach (var field in record.Fields)
        {
            if (!string.Equals(field.Name, expression.FieldName, StringComparison.Ordinal))
            {
                continue;
            }

            return expression.Operator switch
            {
                RecSelectionExpressionOperator.Equals => string.Equals(field.Value, expression.Value, StringComparison.Ordinal),
                RecSelectionExpressionOperator.NotEquals => !string.Equals(field.Value, expression.Value, StringComparison.Ordinal),
                RecSelectionExpressionOperator.Contains => field.Value.Contains(expression.Value, StringComparison.Ordinal),
                _ => false
            };
        }

        return false;
    }

    private static RecRecord ProjectRecord(RecRecord record, IReadOnlySet<string>? projectedFields)
    {
        if (projectedFields is null)
        {
            return record;
        }

        var fields = record.Fields
            .Where(field => projectedFields.Contains(field.Name))
            .ToArray();

        return record with { Fields = fields };
    }
}
