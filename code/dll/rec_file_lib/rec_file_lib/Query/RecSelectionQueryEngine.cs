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

        var valuesByFieldName = BuildValuesByFieldName(record);
        return EvaluateExpression(valuesByFieldName, expression, ExpressionBindingContext.Empty).Any();
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<string>> BuildValuesByFieldName(RecRecord record)
    {
        var valuesByFieldName = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        foreach (var field in record.Fields)
        {
            if (!valuesByFieldName.TryGetValue(field.Name, out var values))
            {
                values = [];
                valuesByFieldName[field.Name] = values;
            }

            values.Add(field.Value);
        }

        return valuesByFieldName.ToDictionary(
            pair => pair.Key,
            pair => (IReadOnlyList<string>)pair.Value,
            StringComparer.Ordinal);
    }

    private static IEnumerable<ExpressionBindingContext> EvaluateExpression(
        IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByFieldName,
        RecSelectionExpression expression,
        ExpressionBindingContext context)
    {
        return expression switch
        {
            RecSelectionComparisonExpression comparison => EvaluateComparison(valuesByFieldName, comparison, context),
            RecSelectionUnaryExpression unary => EvaluateUnary(valuesByFieldName, unary, context),
            RecSelectionBinaryExpression binary => EvaluateBinary(valuesByFieldName, binary, context),
            _ => []
        };
    }

    private static IEnumerable<ExpressionBindingContext> EvaluateUnary(
        IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByFieldName,
        RecSelectionUnaryExpression expression,
        ExpressionBindingContext context)
    {
        return expression.Operator switch
        {
            RecSelectionUnaryOperator.Not => EvaluateExpression(valuesByFieldName, expression.Operand, context).Any()
                ? []
                : [context],
            _ => []
        };
    }

    private static IEnumerable<ExpressionBindingContext> EvaluateBinary(
        IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByFieldName,
        RecSelectionBinaryExpression expression,
        ExpressionBindingContext context)
    {
        return expression.Operator switch
        {
            RecSelectionBinaryOperator.And => EvaluateAnd(valuesByFieldName, expression.Left, expression.Right, context),
            RecSelectionBinaryOperator.Or => EvaluateOr(valuesByFieldName, expression.Left, expression.Right, context),
            _ => []
        };
    }

    private static IEnumerable<ExpressionBindingContext> EvaluateAnd(
        IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByFieldName,
        RecSelectionExpression left,
        RecSelectionExpression right,
        ExpressionBindingContext context)
    {
        foreach (var leftContext in EvaluateExpression(valuesByFieldName, left, context))
        {
            foreach (var rightContext in EvaluateExpression(valuesByFieldName, right, leftContext))
            {
                yield return rightContext;
            }
        }
    }

    private static IEnumerable<ExpressionBindingContext> EvaluateOr(
        IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByFieldName,
        RecSelectionExpression left,
        RecSelectionExpression right,
        ExpressionBindingContext context)
    {
        foreach (var leftContext in EvaluateExpression(valuesByFieldName, left, context))
        {
            yield return leftContext;
        }

        foreach (var rightContext in EvaluateExpression(valuesByFieldName, right, context))
        {
            yield return rightContext;
        }
    }

    private static IEnumerable<ExpressionBindingContext> EvaluateComparison(
        IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByFieldName,
        RecSelectionComparisonExpression expression,
        ExpressionBindingContext context)
    {
        if (context.TryGetBinding(expression.BindingId, out var boundValue))
        {
            if (MatchesComparison(boundValue, expression.Operator, expression.Value))
            {
                yield return context;
            }

            yield break;
        }

        if (!valuesByFieldName.TryGetValue(expression.FieldName, out var candidateValues))
        {
            yield break;
        }

        foreach (var candidateValue in candidateValues)
        {
            if (!MatchesComparison(candidateValue, expression.Operator, expression.Value))
            {
                continue;
            }

            yield return context.Bind(expression.BindingId, candidateValue);
        }
    }

    private sealed class ExpressionBindingContext
    {
        private readonly IReadOnlyDictionary<int, string> _boundValues;

        private ExpressionBindingContext(IReadOnlyDictionary<int, string> boundValues)
        {
            _boundValues = boundValues;
        }

        public static ExpressionBindingContext Empty { get; } = new(new Dictionary<int, string>());

        public bool TryGetBinding(int bindingId, out string boundValue)
        {
            return _boundValues.TryGetValue(bindingId, out boundValue!);
        }

        public ExpressionBindingContext Bind(int bindingId, string value)
        {
            var updated = _boundValues.ToDictionary(pair => pair.Key, pair => pair.Value);
            updated[bindingId] = value;
            return new ExpressionBindingContext(updated);
        }
    }

    private static int CompareValues(string left, string right)
    {
        if (TryParseScalarNumber(left, out var leftNumber)
            && TryParseScalarNumber(right, out var rightNumber))
        {
            return leftNumber.CompareTo(rightNumber);
        }

        return string.CompareOrdinal(left, right);
    }

    private static bool TryParseScalarNumber(string value, out decimal number)
    {
        number = 0;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var text = value.Trim();
        var sign = 1m;

        if (text[0] == '+' || text[0] == '-')
        {
            sign = text[0] == '-' ? -1m : 1m;
            text = text[1..];
            if (text.Length == 0)
            {
                return false;
            }
        }

        if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            var digits = text[2..];
            if (digits.Length == 0)
            {
                return false;
            }

            try
            {
                var parsed = Convert.ToInt64(digits, 16);
                number = sign * parsed;
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (text.Contains('.', StringComparison.Ordinal)
            || text.Contains('e', StringComparison.OrdinalIgnoreCase))
        {
            if (!decimal.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var realValue))
            {
                return false;
            }

            number = sign * realValue;
            return true;
        }

        if (text.Length > 1 && text[0] == '0')
        {
            if (text.Any(c => c is < '0' or > '7'))
            {
                return false;
            }

            try
            {
                var parsed = Convert.ToInt64(text, 8);
                number = sign * parsed;
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (!decimal.TryParse(text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var integerValue))
        {
            return false;
        }

        number = sign * integerValue;
        return true;
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

    private static bool MatchesComparison(string leftValue, RecSelectionExpressionOperator op, string rightValue)
    {
        return op switch
        {
            RecSelectionExpressionOperator.Equals => string.Equals(leftValue, rightValue, StringComparison.Ordinal),
            RecSelectionExpressionOperator.NotEquals => !string.Equals(leftValue, rightValue, StringComparison.Ordinal),
            RecSelectionExpressionOperator.Contains => leftValue.Contains(rightValue, StringComparison.Ordinal),
            RecSelectionExpressionOperator.LessThan => CompareValues(leftValue, rightValue) < 0,
            RecSelectionExpressionOperator.LessThanOrEqual => CompareValues(leftValue, rightValue) <= 0,
            RecSelectionExpressionOperator.GreaterThan => CompareValues(leftValue, rightValue) > 0,
            RecSelectionExpressionOperator.GreaterThanOrEqual => CompareValues(leftValue, rightValue) >= 0,
            _ => false
        };
    }
}
