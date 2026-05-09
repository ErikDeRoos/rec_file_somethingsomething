using rec_file_lib.Model;

namespace rec_file_lib.Model.Builders;

internal sealed class RecRecordBuilder
{
    private readonly List<RecFieldBuilder> _fields = new();

    public IReadOnlyList<RecFieldBuilder> Fields => _fields;

    public void AddField(RecFieldBuilder field)
    {
        _fields.Add(field);
    }

    public RecRecord Build()
    {
        return new RecRecord(_fields.Select(field => field.Build()).ToArray());
    }
}
