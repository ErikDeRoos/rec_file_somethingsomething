# Phase 1 - Foundation

## Goal
Create the internal engine needed by both `DirectFileServerV1` and `SqlFileServerV1` without splitting the production code into multiple projects, and do it in an example-driven way starting from shared example data.

## Scope
- In-memory model for rec files
- Parser for records, fields, comments, and descriptors
- Serializer for writing rec data back to text
- Initial validation groundwork for descriptor-driven rules
- Separate test project
- Tests that read shared example files directly from `docs/examples`

## Current implementation status
### Implemented
- Immutable runtime model using record classes
- Builder types used as mutable parser staging structures
- Parser for records, fields, comments, descriptor fields, and multiline values
- Serializer with round-trip coverage for valid examples
- Shared example scenario wrapper for tests
- Tests for valid examples and malformed parser scenarios

### Still to do in foundation
- Validation layer for semantic-invalid examples
- Broader descriptor support beyond the currently used subset
- More malformed input coverage as edge cases are identified

## Proposed folders and namespaces
- `Common/`
- `Model/`
- `Parsing/`
- `Validation/`
- `Query/`
- `DirectFileServer/`
- `SqlFileServer/`
- `Interop/`

## Current model types
- `RecFileDocument`
- `RecRecordSet`
- `RecDescriptor`
- `RecRecord`
- `RecField`
- builder variants for parser mutation

## Parsing targets currently covered
- Fields
- Multiline fields using `+`
- Records separated by blank lines
- Comment lines
- `%rec`
- `%type`
- `%mandatory`
- `%key`
- `%doc`

## Example-driven milestone status
- `docs/examples/1_simple_singlefile/user.rec` is covered by parser and serializer tests
- `docs/examples/2_simple_recutils_book_example/books.rec` is covered by parser and serializer tests
- `docs/examples/8_multiple_record_types_single_file/people_and_residences.rec` is covered by parser and direct-selection tests
- malformed examples under `docs/examples/3_wrong_...` through `7_wrong_...` now help define parser-versus-validator boundaries

## Non-goals for first cut
- Full expression grammar
- Full recutils integrity coverage
- Encryption/confidential fields
- Remote descriptors
- Full aggregate engine

## Exit criteria
- Load a `.rec` file into the model
- Save the model back to `.rec`
- Preserve enough structure for data operations
- Tests can read shared example files directly from `docs/examples`
- Semantic-invalid examples are ready to be used by the future validation layer
