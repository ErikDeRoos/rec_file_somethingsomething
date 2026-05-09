using System.Collections.Generic;

namespace rec_file_lib.Query;

internal sealed record class RecSelectionQueryOptions(
    IReadOnlySet<string>? ProjectedFields,
    IReadOnlySet<int>? SelectedIndexes,
    string? QuickFilter,
    string? Expression,
    string? JoinField);
