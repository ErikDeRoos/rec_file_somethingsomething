using rec_file_lib.Model;
using rec_file_lib.Parsing;

namespace rec_file_lib.DirectFileServer;

internal sealed class DirectFileServerDocumentStore
{
    private readonly RecParser _parser = new();
    private readonly RecSerializer _serializer = new();
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
}
