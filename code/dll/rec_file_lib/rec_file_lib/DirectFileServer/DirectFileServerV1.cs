using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using rec_file_lib.Model;
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

        public string RecSel(string filePath, IRecSelOptions? options)
        {
            _documentStore.LoadFromFile(filePath);

            var collapse = options?.Select?.Collapse ?? false;
            var includeDescriptors = options?.Select?.IncludeDescriptors ?? false;
            var recordType = options?.Type?.RecordType;
            if (string.IsNullOrEmpty(recordType))
            {
                return _recSelFormatter.FormatSelection(_documentStore.GetDocument(), collapse, includeDescriptors);
            }

            var selectedRecordSet = SelectRecordSet(_documentStore.GetDocument(), recordType, options);
            return _recSelFormatter.FormatRecordSet(selectedRecordSet, collapse, includeDescriptors);
        }

        public IRecSelTypedResult RecSel_Typed(string filePath, IRecSelOptions? options)
        {
            try
            {
                _documentStore.LoadFromFile(filePath);
                var document = _documentStore.GetDocument();
                var recordType = options?.Type?.RecordType;

                if (string.IsNullOrEmpty(recordType))
                {
                    if (document.RecordSets.Count == 0)
                    {
                        return RecSelTypedResult.Error(1, "No matching records.");
                    }

                    if (document.RecordSets.Count > 1)
                    {
                        return RecSelTypedResult.Error(2, "several record types found. Please use -t to specify one.");
                    }

                    return RecSelTypedResult.Success(document.RecordSets[0]);
                }

                var selectedRecordSet = SelectRecordSet(document, recordType, options);
                if (selectedRecordSet is null)
                {
                    return RecSelTypedResult.Error(1, "No matching records.");
                }

                return RecSelTypedResult.Success(selectedRecordSet);
            }
            catch (FormatException ex)
            {
                return RecSelTypedResult.Error(3, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return RecSelTypedResult.Error(4, ex.Message);
            }
            catch (Exception ex)
            {
                return RecSelTypedResult.Error(5, ex.Message);
            }
        }

        public string RecIns(string filePath, IRecInsOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(options.RecordType);

            _documentStore.LoadFromFile(filePath);
            var record = _mutationParser.ParseRecord(options.RecordText);
            var updatedRecordSet = _documentStore.InsertRecord(options.RecordType, record);
            _documentStore.SaveToFile(filePath);
            return _recSelFormatter.FormatRecordSet(updatedRecordSet);
        }

        public IRecSelTypedResult RecIns_Typed(string filePath, IRecInsOptions options)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(options);
                ArgumentNullException.ThrowIfNull(options.RecordType);

                _documentStore.LoadFromFile(filePath);
                var record = _mutationParser.ParseRecord(options.RecordText);
                var updatedRecordSet = _documentStore.InsertRecord(options.RecordType, record);
                _documentStore.SaveToFile(filePath);
                return RecSelTypedResult.Success(updatedRecordSet);
            }
            catch (FormatException ex)
            {
                return RecSelTypedResult.Error(3, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return RecSelTypedResult.Error(4, ex.Message);
            }
            catch (Exception ex)
            {
                return RecSelTypedResult.Error(5, ex.Message);
            }
        }

        public string RecDel(string filePath, IRecDelOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(options.RecordType);

            _documentStore.LoadFromFile(filePath);
            var updatedRecordSet = _documentStore.DeleteRecords(options.RecordType);
            _documentStore.SaveToFile(filePath);
            return _recSelFormatter.FormatRecordSet(updatedRecordSet);
        }

        public IRecSelTypedResult RecDel_Typed(string filePath, IRecDelOptions options)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(options);
                ArgumentNullException.ThrowIfNull(options.RecordType);

                _documentStore.LoadFromFile(filePath);
                var updatedRecordSet = _documentStore.DeleteRecords(options.RecordType);
                _documentStore.SaveToFile(filePath);
                return RecSelTypedResult.Success(updatedRecordSet);
            }
            catch (FormatException ex)
            {
                return RecSelTypedResult.Error(3, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return RecSelTypedResult.Error(4, ex.Message);
            }
            catch (Exception ex)
            {
                return RecSelTypedResult.Error(5, ex.Message);
            }
        }

        private RecRecordSet? SelectRecordSet(RecFileDocument document, string recordType, IRecSelOptions? options)
        {
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

            return _selectionQueryEngine.Select(document, _documentStore.FindRecordSet(recordType), queryOptions);
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