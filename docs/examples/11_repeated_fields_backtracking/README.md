# 11_repeated_fields_backtracking

## Purpose
This example demonstrates that repeated fields in a single record are valid and are expected to participate in selection-expression evaluation.

## What it demonstrates
- multiple `Tag` fields in one `Entry` record
- repeated field values are first-class data, not malformed input
- selection expressions may need backtracking across repeated field occurrences (for example `Tag = "red" && Tag = "blue"`)

## Why this matters
recutils-style records allow repeated fields by design. Query behavior should treat these repetitions as queryable occurrences, not as accidental duplicates.

## Files
- `repeated_tags.rec` - valid example showing repeated field occurrences used by backtracking-oriented query tests
