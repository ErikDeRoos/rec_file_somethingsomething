# rec_file_lib roadmap

## Constraints
- One production project: `rec_file_lib`
- Separate test project is allowed later
- Production output should stay a single runtime DLL
- COM-visible entry points remain small and Unity-friendly
- Internal separation is done with namespaces, folders, and `internal` types

## Planned namespace layout
- `rec_file_lib.Common`
- `rec_file_lib.Model`
- `rec_file_lib.Parsing`
- `rec_file_lib.Validation`
- `rec_file_lib.Query`
- `rec_file_lib.DirectFileServer`
- `rec_file_lib.SqlFileServer`
- `rec_file_lib.Interop`

## Phase overview
1. Foundation
2. DirectFileServerV1
3. Query and recutils compatibility expansion
4. SqlFileServerV1
5. COM/Unity hardening and tests

## Design rules
- `DirectFileServerV1` and `SqlFileServerV1` share the same internal model and query engine
- Public surface is kept narrow
- COM contracts prefer `string`, `int`, `bool`, and `string[]`
- Avoid exposing internal model types through COM
- Start with pragmatic recutils compatibility, not full feature parity on day one

## Initial deliverables
- Load and save `.rec` files
- Parse descriptors and records
- Create, update, and delete records and fields
- Basic selection and joins
- SQL-like facade over the same engine after direct API is stable
