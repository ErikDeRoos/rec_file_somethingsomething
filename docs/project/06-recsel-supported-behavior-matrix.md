# RecSel supported behavior matrix (current subset)

This document defines the currently supported `RecSel(filePath, options)` subset and the intended operation ordering used by tests.

## Current supported options subset

| Recutils intent | Current options path | Supported |
|---|---|---|
| `-t` type | `options.Type.RecordType` | Yes |
| `-n` number/indexes | `options.Select.Indexes` | Yes |
| `-q` quick substring | `options.Select.Quick` | Yes |
| `-e` expression | `options.Select.Expression` | Partial (`=`, `!=`, `~`) |
| `-j` join | `options.Select.JoinField` | Yes (inner join) |
| `-p` print fields | `options.Project.FieldNames` | Yes |
| `-G` group by | `options.Group.FieldNames` | Groundwork |
| `-c` count | `options.Aggregate.Count` + `CountFieldName` | Groundwork |
| `-S` sort | `options.Sort.FieldNames` | Basic ascending sort |
| `-U` uniq | `options.Select.Uniq` | Yes |
| `-C` collapse | `options.Select.Collapse` | Yes |
| `-d` include descriptors | `options.Select.IncludeDescriptors` | Yes |

## Explicit operation ordering

The current implementation and tests enforce the following sequencing:

1. Type selection (`-t`)
2. Selection/filtering (`-n`, `-q`, `-e`)
3. Join (`-j`)
4. Grouping + aggregate groundwork (`-G`, count)
5. Sorting (`-S`)
6. Projection (`-p`)
7. Uniq (`-U`) on output records
8. Output formatting (`-C`, `-d`)

Notes:
- Grouping is performed before sorting, aligned with recutils guidance.
- Collapse and include-descriptors are output-shape concerns and are applied during formatting.

## Golden behavior coverage

The test suite includes golden-output scenarios for the supported subset, covering:
- type-only selection
- indexes
- quick filtering
- expression subset
- join + projection
- grouping/count + sorting
- sorting + collapse
- uniq interactions
- include-descriptor output and include-descriptor + collapse

## Known intentional gaps vs full recutils

- Full expression grammar is not implemented.
- Case-insensitive mode (`-i`) is not yet implemented.
- Random selection (`-m`) is not yet implemented.
- Output modes equivalent to `-P`/`-R` are not yet implemented.
- Aggregate support is currently count-focused groundwork.
