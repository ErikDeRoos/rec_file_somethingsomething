# Phase 1 - Foundation

## Goal
Create the internal engine needed by both `DirectFileServerV1` and `SqlFileServerV1` without splitting the production code into multiple projects, and do it in an example-driven way starting from shared example data.

## Scope
- In-memory model for rec files
- Parser for records, fields, comments, and descriptors
- Serializer for writing rec data back to text
- Initial validation for descriptor-driven rules
- Early separate test project
- Tests that read shared example files directly from `docs/examples`

## Proposed folders and namespaces
- `Common/`
- `Model/`
- `Parsing/`
- `Validation/`
- `Query/`
- `DirectFileServer/`
- `SqlFileServer/`
- `Interop/`

## Initial model types
- `RecFileDocument`
- `RecRecordSet`
- `RecDescriptor`
- `RecRecord`
- `RecField`
- `RecFieldTypeDefinition`
- `RecSelectionResult`

## Parsing targets
- Fields
- Multiline fields using `+`
- Records separated by blank lines
- Comment lines
- `%rec`
- `%type`
- `%mandatory`
- `%allowed`
- `%prohibit`
- `%unique`
- `%key`
- `%sort`
- `%auto`

## First example-driven milestone
- Use `docs/examples/1_simple_singlefile/user.rec`
- The file acts as both documentation material and test input
- First tests should prove the document can be loaded, parsed, inspected, and saved
- The first example is intentionally minimal: typed record set, comment line, and multiline field data

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
- Basic validation can detect obvious descriptor violations
- Tests can read shared example files directly from `docs/examples`
