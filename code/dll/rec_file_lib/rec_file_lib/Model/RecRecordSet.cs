namespace rec_file_lib.Model;

internal sealed record class RecRecordSet(
    string? TypeName,
    RecDescriptor Descriptor,
    IReadOnlyList<RecRecord> Records);
