# Current DLL capabilities

This page tracks what the DLL supports today.

## Current documented direction
- Single production DLL
- COM-visible entry points over a shared internal engine
- Example-driven implementation using files under `docs/examples`
- Tests read shared examples directly

## Currently implemented foundation
- Immutable record-based runtime model
- Builder-based parsing flow
- Stream-capable parser
- Stream-capable serializer
- Round-trip coverage for valid examples
- Parser coverage for malformed examples that distinguishes syntax problems from semantic-invalid data
- Disposable working-copy test infrastructure for file-mutating tests

## Descriptor support currently implemented
- `%rec`
- `%key`
- `%type`
- `%mandatory`
- `%doc`

## Direct API status
- `DirectFileServerV1` exposes:
  - `RecSel(filePath, options)`
  - `RecSel_Typed(filePath, options)`
  - `RecIns(filePath, options)`
  - `RecIns_Typed(filePath, options)`
  - `RecDel(filePath, options)`
  - `RecDel_Typed(filePath, options)`
- Current direct-selection output is formatted text used as a recutils-style compatibility layer
- `RecSel` option support currently includes:
  - type selection
  - index selection
  - quick substring filtering
  - expression subset
    - `=`, `!=`, `~`, `<`, `<=`, `>`, `>=`
    - boolean composition: `&&`, `||`, `!`
    - parentheses grouping
    - numeric-aware scalar comparisons (decimal/hex/octal/sign) with ordinal string fallback
  - join
  - projection
  - grouping + count groundwork
  - sorting
  - uniq
  - collapse
  - include descriptors

## Validation and type support status
- Implemented validation types:
  - `line`
  - `enum ...`
  - `rec ...`
  - `int` (decimal, signed, hex `0x`, octal leading `0`)
- Additional recutils types are planned incrementally.

## Compatibility scope
This project tracks recutils behavior incrementally.

The currently supported subset and explicit operation ordering are documented in:
- `docs/project/06-recsel-supported-behavior-matrix.md`

## Current example coverage
### Valid examples
- `docs/examples/1_simple_singlefile/user.rec`
- `docs/examples/2_simple_recutils_book_example/books.rec`
- `docs/examples/8_multiple_record_types_single_file/people_and_residences.rec`
- `docs/examples/9_int_field_type/tasks.rec`

### Intentionally invalid examples
- `docs/examples/3_wrong_missing_mandatory_field/missing_name.rec`
- `docs/examples/4_wrong_duplicate_key_value/duplicate_id.rec`
- `docs/examples/5_wrong_invalid_field_type/invalid_status.rec`
- `docs/examples/6_wrong_bad_multiline_continuation/orphan_continuation.rec`
- `docs/examples/7_wrong_missing_field_separator/missing_colon.rec`
- `docs/examples/10_wrong_invalid_int_field_type/invalid_priority.rec`

## Next capability target
- Add case-insensitive matching (`-i` style)
- Continue tightening recutils-compatibility interactions
- Extend type validation beyond current subset (`int`, `line`, `enum`, `rec`)
- Extend aggregates beyond count groundwork
- Evolve DirectFileServer result access toward structured cursor/reader-style patterns
