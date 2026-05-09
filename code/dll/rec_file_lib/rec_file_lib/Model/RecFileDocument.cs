namespace rec_file_lib.Model;

internal sealed record class RecFileDocument(
    IReadOnlyList<string> LeadingCommentLines,
    IReadOnlyList<string> TrailingCommentLines,
    IReadOnlyList<RecRecordSet> RecordSets)
{
    public IReadOnlyList<string> CommentLines => LeadingCommentLines.Concat(TrailingCommentLines).ToArray();
}
