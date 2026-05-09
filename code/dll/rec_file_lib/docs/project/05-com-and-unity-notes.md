# COM and Unity notes

## Goals
- Keep COM exposure stable
- Keep Unity integration practical
- Avoid leaking internal implementation details

## Contract guidance
Prefer:
- `string`
- `int`
- `bool`
- `string[]`

Avoid exposing:
- generic collections
- nested object graphs
- internal model types
- implementation-specific parser/query objects

## Result format guidance
For complex outputs, prefer one of:
- plain rec text
- JSON strings
- delimited string arrays

## Registration and packaging
- Keep the production output centered on the single `rec_file_lib` assembly
- Allow tests and developer docs to live separately

## Versioning
- `DirectFileServerV1` remains stable even as internal namespaces evolve
- `SqlFileServerV1` grows incrementally behind a stable COM boundary
