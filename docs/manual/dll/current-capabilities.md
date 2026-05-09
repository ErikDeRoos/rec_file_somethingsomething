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

## Current example coverage
### Valid examples
- `docs/examples/1_simple_singlefile/user.rec`
- `docs/examples/2_simple_recutils_book_example/books.rec`

### Intentionally invalid examples
- `docs/examples/3_wrong_missing_mandatory_field/missing_name.rec`
- `docs/examples/4_wrong_duplicate_key_value/duplicate_id.rec`
- `docs/examples/5_wrong_invalid_field_type/invalid_status.rec`
- `docs/examples/6_wrong_bad_multiline_continuation/orphan_continuation.rec`
- `docs/examples/7_wrong_missing_field_separator/missing_colon.rec`

## Next capability target
- Add validation logic for semantic-invalid examples
- Build the first minimal `DirectFileServerV1` API over the current foundation
