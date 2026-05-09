using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using rec_file_lib.Query;

namespace rec_file_lib.DirectFileServer
{
    [ComVisible(true)]
    [Guid(ContractGuids.DirectFileServer_Implementation_V1)]
    public class DirectFileServerV1 : IDirectFileServer
    {
        private readonly DirectFileServerDocumentStore _documentStore = new();
        private readonly DirectRecSelFormatter _recSelFormatter = new();
        private readonly DirectRecMutationParser _mutationParser = new();
        private readonly RecSelectionQueryEngine _selectionQueryEngine = new();

        public string RecSel(string filePath, RecSelOptions? options)
        {
            _documentStore.LoadFromFile(filePath);

            var collapse = options?.Select?.Collapse ?? false;
            var includeDescriptors = options?.Select?.IncludeDescriptors ?? false;
            var recordType = options?.Type?.RecordType;
            if (string.IsNullOrEmpty(recordType))
            {
                return _recSelFormatter.FormatSelection(_documentStore.GetDocument(), collapse, includeDescriptors);
            }

            var document = _documentStore.GetDocument();
            var queryOptions = new RecSelectionQueryOptions(
                ProjectedFields: ParseProjectedFields(options?.Project?.FieldNames),
                SelectedIndexes: ParseSelectedIndexes(options?.Select?.Indexes),
                QuickFilter: ParseQuickFilter(options?.Select?.Quick),
                Expression: ParseExpression(options?.Select?.Expression),
                JoinField: ParseJoinField(options?.Select?.JoinField),
                GroupByFields: ParseGroupByFields(options?.Group?.FieldNames),
                Count: options?.Aggregate?.Count ?? false,
                CountFieldName: ParseCountFieldName(options?.Aggregate?.CountFieldName),
                SortFields: ParseSortFields(options?.Sort?.FieldNames),
                Uniq: options?.Select?.Uniq ?? false);

            var selectedRecordSet = _selectionQueryEngine.Select(
                document,
                _documentStore.FindRecordSet(recordType),
                queryOptions);

            return _recSelFormatter.FormatRecordSet(selectedRecordSet, collapse, includeDescriptors);
        }

        public string RecInsType(string filePath, string recordType, string recordText)
        {
            ArgumentNullException.ThrowIfNull(recordType);

            _documentStore.LoadFromFile(filePath);
            var record = _mutationParser.ParseRecord(recordText);
            var updatedRecordSet = _documentStore.InsertRecord(recordType, record);
            _documentStore.SaveToFile(filePath);
            return _recSelFormatter.FormatRecordSet(updatedRecordSet);
        }

        public string RecDelType(string filePath, string recordType)
        {
            ArgumentNullException.ThrowIfNull(recordType);

            _documentStore.LoadFromFile(filePath);
            var updatedRecordSet = _documentStore.DeleteRecords(recordType);
            _documentStore.SaveToFile(filePath);
            return _recSelFormatter.FormatRecordSet(updatedRecordSet);
        }

        private static IReadOnlySet<string>? ParseProjectedFields(string[]? fields)
        {
            if (fields is null || fields.Length == 0)
            {
                return null;
            }

            var projectedFields = fields
                .Where(static name => !string.IsNullOrWhiteSpace(name))
                .Select(static name => name.Trim())
                .ToHashSet(StringComparer.Ordinal);

            return projectedFields.Count == 0 ? null : projectedFields;
        }

        private static IReadOnlySet<int>? ParseSelectedIndexes(string? indexes)
        {
            if (string.IsNullOrWhiteSpace(indexes))
            {
                return null;
            }

            var selectedIndexes = new HashSet<int>();
            var tokens = indexes.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var token in tokens)
            {
                var dashIndex = token.IndexOf('-');
                if (dashIndex >= 0)
                {
                    var startToken = token[..dashIndex];
                    var endToken = token[(dashIndex + 1)..];
                    if (!int.TryParse(startToken, out var start) || !int.TryParse(endToken, out var end))
                    {
                        throw new FormatException($"Invalid record index range: '{token}'.");
                    }

                    if (start < 0 || end < 0 || end < start)
                    {
                        throw new FormatException($"Invalid record index range: '{token}'.");
                    }

                    for (var index = start; index <= end; index++)
                    {
                        selectedIndexes.Add(index);
                    }

                    continue;
                }

                if (!int.TryParse(token, out var value) || value < 0)
                {
                    throw new FormatException($"Invalid record index: '{token}'.");
                }

                selectedIndexes.Add(value);
            }

            return selectedIndexes.Count == 0 ? null : selectedIndexes;
        }

        private static string? ParseQuickFilter(string? quick)
        {
            if (string.IsNullOrWhiteSpace(quick))
            {
                return null;
            }

            return quick;
        }

        private static string? ParseExpression(string? expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return null;
            }

            return expression;
        }

        private static string? ParseJoinField(string? joinField)
        {
            if (string.IsNullOrWhiteSpace(joinField))
            {
                return null;
            }

            return joinField.Trim();
        }

        private static IReadOnlyList<string>? ParseGroupByFields(string[]? fieldNames)
        {
            if (fieldNames is null || fieldNames.Length == 0)
            {
                return null;
            }

            var normalized = fieldNames
                .Where(static name => !string.IsNullOrWhiteSpace(name))
                .Select(static name => name.Trim())
                .ToArray();

            return normalized.Length == 0 ? null : normalized;
        }

        private static string ParseCountFieldName(string? countFieldName)
        {
            if (string.IsNullOrWhiteSpace(countFieldName))
            {
                return "Count";
            }

            return countFieldName.Trim();
        }

        private static IReadOnlyList<string>? ParseSortFields(string[]? fieldNames)
        {
            if (fieldNames is null || fieldNames.Length == 0)
            {
                return null;
            }

            var normalized = fieldNames
                .Where(static name => !string.IsNullOrWhiteSpace(name))
                .Select(static name => name.Trim())
                .ToArray();

            return normalized.Length == 0 ? null : normalized;
        }
    }
}