# Current DLL capabilities

This page tracks what the DLL supports today.

## Current documented direction
- Single production DLL
- COM-visible entry points planned on top of a shared internal engine
- Example-driven implementation using files under `docs/examples`
- Tests read shared examples directly

## Currently implemented foundation
- Immutable record-based runtime model
- Builder-based parsing flow
- Stream-capable parser
- Stream-capable serializer
- Round-trip coverage for valid examples
- Parser coverage for malformed examples that distinguishes syntax problems from semantic-invalid data

## Descriptor support currently implemented
- `%rec`
- `%key`
- `%type`
- `%mandatory`
- `%doc`

## Direct API status
- `DirectFileServerV1` currently exposes tool-shaped `RecSel(filePath)` and `RecSelType(filePath, recordType)` methods
- Current direct-selection output is formatted text intended as a recutils-style compatibility layer
- `RecSelType` now has a meaningful behavioral difference in multi-record-type single-file examples
- The preferred longer-term direction is structured cursor or reader-style result access rather than raw text as the main contract

## Current example coverage
### Valid examples
- `docs/examples/1_simple_singlefile/user.rec`
- `docs/examples/2_simple_recutils_book_example/books.rec`
- `docs/examples/8_multiple_record_types_single_file/people_and_residences.rec`

### Intentionally invalid examples
- `docs/examples/3_wrong_missing_mandatory_field/missing_name.rec`
- `docs/examples/4_wrong_duplicate_key_value/duplicate_id.rec`
- `docs/examples/5_wrong_invalid_field_type/invalid_status.rec`
- `docs/examples/6_wrong_bad_multiline_continuation/orphan_continuation.rec`
- `docs/examples/7_wrong_missing_field_separator/missing_colon.rec`

## Next capability target
- Add validation logic for semantic-invalid examples
- Evolve DirectFileServer result access toward structured cursor or reader-style patterns
- Add the next small recutils-style direct operation only after the result-shape direction stays coherent
