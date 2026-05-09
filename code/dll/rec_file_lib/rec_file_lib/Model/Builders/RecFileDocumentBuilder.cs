using rec_file_lib.Model;

namespace rec_file_lib.Model.Builders;

internal sealed class RecFileDocumentBuilder
{
    private readonly List<string> _leadingCommentLines = new();
    private readonly List<string> _trailingCommentLines = new();
    private readonly List<RecRecordSetBuilder> _recordSets = new();

    public IReadOnlyList<string> LeadingCommentLines => _leadingCommentLines;

    public IReadOnlyList<string> TrailingCommentLines => _trailingCommentLines;

    public IReadOnlyList<RecRecordSetBuilder> RecordSets => _recordSets;

    public void AddCommentLine(string commentLine, bool isTrailing)
    {
        if (isTrailing)
        {
            _trailingCommentLines.Add(commentLine);
            return;
        }

        _leadingCommentLines.Add(commentLine);
    }

    public void AddRecordSet(RecRecordSetBuilder recordSet)
    {
        _recordSets.Add(recordSet);
    }

    public RecFileDocument Build()
    {
        return new RecFileDocument(
            _leadingCommentLines.ToArray(),
            _trailingCommentLines.ToArray(),
            _recordSets.Select(recordSet => recordSet.Build()).ToArray());
    }
}
