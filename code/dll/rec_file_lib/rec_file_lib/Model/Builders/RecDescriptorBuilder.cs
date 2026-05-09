using rec_file_lib.Model;

namespace rec_file_lib.Model.Builders;

internal sealed class RecDescriptorBuilder
{
    private readonly List<RecFieldBuilder> _fields = new();

    public IReadOnlyList<RecFieldBuilder> Fields => _fields;

    public void AddField(RecFieldBuilder field)
    {
        _fields.Add(field);
    }

    public RecDescriptor Build()
    {
        var fields = _fields.Select(field => field.Build()).ToArray();
        var fieldTypes = new Dictionary<string, string>(StringComparer.Ordinal);
        var mandatoryFieldNames = new List<string>();
        string? keyFieldName = null;
        string? documentation = null;

        foreach (var field in fields)
        {
            switch (field.Name)
            {
                case "%key":
                    keyFieldName = field.Value;
                    break;

                case "%type":
                    var typeParts = field.Value.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (typeParts.Length == 2)
                    {
                        fieldTypes[typeParts[0]] = typeParts[1];
                    }
                    break;

                case "%mandatory":
                    mandatoryFieldNames.AddRange(field.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                    break;

                case "%doc":
                    documentation = field.Value;
                    break;
            }
        }

        return new RecDescriptor(
            fields,
            keyFieldName,
            fieldTypes,
            mandatoryFieldNames.ToArray(),
            documentation);
    }
}
