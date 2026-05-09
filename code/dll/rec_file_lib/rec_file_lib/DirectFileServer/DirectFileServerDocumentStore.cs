using rec_file_lib.Model;
using rec_file_lib.Parsing;
using rec_file_lib.Validation;

namespace rec_file_lib.DirectFileServer;

internal sealed class DirectFileServerDocumentStore
{
    private readonly RecParser _parser = new();
    private readonly RecSerializer _serializer = new();
    private readonly RecValidator _validator = new();
    private RecFileDocument _document = new([], [], []);

    public void CreateEmptyDocument()
    {
        _document = new RecFileDocument([], [], []);
    }

    public void LoadFromText(string text)
    {
        _document = _parser.Parse(text);
    }

    public void LoadFromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        var text = File.ReadAllText(filePath);
        _document = _parser.Parse(text);
    }

    public string SaveToText()
    {
        return _serializer.Serialize(_document);
    }

    public void SaveToFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        File.WriteAllText(filePath, SaveToText());
    }

    public string[] GetRecordSetTypes()
    {
        return _document.RecordSets
            .Select(recordSet => recordSet.TypeName ?? string.Empty)
            .ToArray();
    }

    public int GetRecordCount(string recordSetType)
    {
        ArgumentNullException.ThrowIfNull(recordSetType);

        return FindRecordSet(recordSetType)?.Records.Count ?? 0;
    }

    public RecFileDocument GetDocument()
    {
        return _document;
    }

    public RecRecordSet? FindRecordSet(string recordSetType)
    {
        ArgumentNullException.ThrowIfNull(recordSetType);

        if (recordSetType.Length == 0)
        {
            return _document.RecordSets
                .FirstOrDefault(recordSet => string.IsNullOrEmpty(recordSet.TypeName));
        }

        return _document.RecordSets
            .FirstOrDefault(recordSet => string.Equals(recordSet.TypeName, recordSetType, StringComparison.Ordinal));
    }

    public RecRecordSet InsertRecord(string recordSetType, RecRecord record)
    {
        ArgumentNullException.ThrowIfNull(recordSetType);
        ArgumentNullException.ThrowIfNull(record);

        var recordSet = FindRecordSet(recordSetType);
        if (recordSet is null)
        {
            throw new InvalidOperationException($"record set '{recordSetType}' not found.");
        }

        var updatedRecordSets = _document.RecordSets
            .Select(existingRecordSet => ReferenceEquals(existingRecordSet, recordSet)
                ? existingRecordSet with { Records = existingRecordSet.Records.Concat([record]).ToArray() }
                : existingRecordSet)
            .ToArray();

        var updatedDocument = _document with { RecordSets = updatedRecordSets };
        _validator.EnsureValid(updatedDocument);
        _document = updatedDocument;

        return FindRecordSet(recordSetType)!;
    }
}
