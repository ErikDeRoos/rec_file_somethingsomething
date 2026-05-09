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
For complex outputs, prefer a long-term direction based on:
- structured response wrappers
- cursor or reader-style iteration
- compatibility formatting layered on top when needed

For now, plain text output can still be used as a temporary compatibility layer where that helps mirror recutils behavior.

## Registration and packaging
- Keep the production output centered on the single `rec_file_lib` assembly
- Allow tests and developer docs to live separately
- Keep GitHub-ready DLL documentation under `docs/manual/dll`
- Keep GitHub-ready editor documentation under `docs/manual/editor`

## Example-driven workflow
- Shared example data lives under `docs/examples`
- Tests should read realistic shared examples directly from `docs/examples`
- Each example folder should include its own `README.md` explaining purpose and scope

## Versioning
- `DirectFileServerV1` remains stable even as internal namespaces evolve
- `SqlFileServerV1` grows incrementally behind a stable COM boundary
- Future result-shape changes that are not backward compatible should be versioned deliberately
