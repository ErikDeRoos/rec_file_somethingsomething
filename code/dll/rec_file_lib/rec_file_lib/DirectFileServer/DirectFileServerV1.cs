using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace rec_file_lib.DirectFileServer
{
    [ComVisible(true)]
    [Guid(ContractGuids.DirectFileServer_Implementation_V1)]
    public class DirectFileServerV1 : IDirectFileServer
    {
        private readonly DirectFileServerDocumentStore _documentStore = new();
        private readonly DirectRecSelFormatter _recSelFormatter = new();
        private readonly DirectRecMutationParser _mutationParser = new();

        public string RecSel(string filePath)
        {
            _documentStore.LoadFromFile(filePath);
            return _recSelFormatter.FormatSelection(_documentStore.GetDocument());
        }

        public string RecSelType(string filePath, string recordType)
        {
            ArgumentNullException.ThrowIfNull(recordType);

            _documentStore.LoadFromFile(filePath);
            return _recSelFormatter.FormatRecordSet(_documentStore.FindRecordSet(recordType));
        }

        public string RecSelTypeSelect(string filePath, string recordType, string? indexes, string? fields)
        {
            ArgumentNullException.ThrowIfNull(recordType);

            _documentStore.LoadFromFile(filePath);

            var projectedFields = ParseProjectedFields(fields);
            var selectedIndexes = ParseSelectedIndexes(indexes);

            return _recSelFormatter.FormatRecordSet(
                _documentStore.FindRecordSet(recordType),
                projectedFields,
                selectedIndexes);
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

        private static IReadOnlySet<string>? ParseProjectedFields(string? fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return null;
            }

            var projectedFields = new HashSet<string>(StringComparer.Ordinal);
            var names = fields.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in names)
            {
                projectedFields.Add(name);
            }

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
    }
}