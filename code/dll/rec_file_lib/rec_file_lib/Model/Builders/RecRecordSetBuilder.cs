using rec_file_lib.Model;

namespace rec_file_lib.Model.Builders;

internal sealed class RecRecordSetBuilder
{
    private readonly List<RecRecordBuilder> _records = new();

    public string? TypeName { get; set; }

    public RecDescriptorBuilder Descriptor { get; } = new();

    public IReadOnlyList<RecRecordBuilder> Records => _records;

    public void AddRecord(RecRecordBuilder record)
    {
        _records.Add(record);
    }

    public RecRecordSet Build()
    {
        return new RecRecordSet(TypeName, Descriptor.Build(), _records.Select(record => record.Build()).ToArray());
    }
}
