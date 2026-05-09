# rec_file_lib roadmap

## Constraints
- One production project: `rec_file_lib`
- Separate test project is required and has been introduced early
- A separate desktop editor project will live in the same repository later
- Production output should stay a single runtime DLL
- COM-visible entry points remain small and Unity-friendly
- Internal separation is done with namespaces, folders, and `internal` types
- Shared examples in `docs/examples` are used by both documentation and tests

## Planned namespace layout
- `rec_file_lib.Common`
- `rec_file_lib.Model`
- `rec_file_lib.Parsing`
- `rec_file_lib.Validation`
- `rec_file_lib.Query`
- `rec_file_lib.DirectFileServer`
- `rec_file_lib.SqlFileServer`
- `rec_file_lib.Interop`

## Repository tracks
1. DLL/library project
2. Test project with small bite-sized scenario coverage
3. Desktop editor project
4. GitHub-ready manuals and shared examples

## Phase overview
1. Foundation and example-driven test setup
2. DirectFileServerV1
3. Query and recutils compatibility expansion
4. SqlFileServerV1
5. Packaging, manuals, editor, and release visibility

## Design rules
- `DirectFileServerV1` and `SqlFileServerV1` share the same internal model and query engine
- Public surface is kept narrow
- COM contracts prefer `string`, `int`, `bool`, and `string[]`
- Avoid exposing internal model types through COM
- Start with pragmatic recutils compatibility, not full feature parity on day one
- Every commit should add a visible working increment backed by tests where practical
- Shared example files should anchor both documentation and realistic test coverage

## Current status
- Immutable record-based runtime model is in place
- Builder-based parsing flow is in place
- Stream-capable parser and serializer are implemented
- Shared example scenario wrappers are in place for tests
- Valid and malformed example sets are both part of the workflow
- `DirectFileServerV1` now uses an options-based selection API: `RecSel(filePath, options)`
- Selection options are grouped as type/select/project with support for index and quick substring selection
- A dedicated internal query layer is introduced (`rec_file_lib.Query`) and used by `DirectFileServerV1`

## Current examples
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

## Current deliverables already implemented
- Load and save `.rec` files
- Parse descriptors and records
- Parse comments and multiline fields
- Preserve descriptor order for serialization
- Round-trip valid examples through parser and serializer
- Parse malformed semantic examples for later validation
- Fail early on malformed syntactical examples
- Provide options-based direct selection through `RecSel(filePath, options)`
- Provide type selection, index selection, projection, and quick substring filtering in direct selection
- Provide initial typed record insertion with validation-aware file updates
- Provide query-layer unit coverage for selection/projection/filter combinations

## Next likely deliverables
- Expand validation logic for semantic-invalid examples
- Add expression-based selection into the shared query layer
- Extend query engine capabilities (sorting, join expansion, grouping groundwork)
- Evolve DirectFileServer result access toward structured cursor or reader-style patterns
