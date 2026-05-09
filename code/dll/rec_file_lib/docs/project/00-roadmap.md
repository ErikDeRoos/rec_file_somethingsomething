# rec_file_lib roadmap

## Constraints
- One production project: `rec_file_lib`
- Separate test project is required and should be introduced early
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

## Current first example
- `docs/examples/1_simple_singlefile/user.rec`
- The first example is intentionally small
- It demonstrates a typed record set, a comment line, and multiline field data
- It is intended to be read directly by future tests

## Initial deliverables
- Load and save `.rec` files
- Parse descriptors and records
- Start from the shared example in `docs/examples`
- Create, update, and delete records and fields
- Basic selection and joins
- SQL-like facade over the same engine after direct API is stable
