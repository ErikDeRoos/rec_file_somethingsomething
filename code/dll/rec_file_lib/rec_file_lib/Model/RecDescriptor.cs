namespace rec_file_lib.Model;

internal sealed record class RecDescriptor(
    IReadOnlyList<RecField> Fields,
    string? KeyFieldName,
    IReadOnlyDictionary<string, string> FieldTypes,
    IReadOnlyList<string> MandatoryFieldNames,
    string? Documentation);
