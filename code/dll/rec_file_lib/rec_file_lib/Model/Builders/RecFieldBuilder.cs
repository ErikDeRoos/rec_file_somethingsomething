using rec_file_lib.Model;

namespace rec_file_lib.Model.Builders;

internal sealed class RecFieldBuilder
{
    public RecFieldBuilder(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public string Value { get; private set; }

    public void AppendContinuation(string continuationValue)
    {
        Value += "\n" + continuationValue;
    }

    public RecField Build()
    {
        return new RecField(Name, Value);
    }
}
