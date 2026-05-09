# rec_file_somethingsomething

C# rec file reader/manipulation library with a COM-facing API and a growing recutils-compatible selection surface.

## Status
- Active pre-release development
- Primary focus: `DirectFileServerV1` option-driven `RecSel(filePath, options)` behavior

## Repository layout
- `code/dll/rec_file_lib/` - production DLL and tests
- `docs/` - architecture, project phases, manuals, and examples
- `recutils/manual/recutils.txt` - reference behavior source used for compatibility alignment

## Current DirectFileServer selection support (subset)
`RecSel(filePath, options)` currently supports:
- type (`-t` style)
- indexes (`-n` style)
- quick substring (`-q` style)
- expression subset (`-e` subset)
  - `=`, `!=`, `~`, `<`, `<=`, `>`, `>=`
  - `&&`, `||`, `!`, and parentheses
  - numeric-aware scalar comparisons with ordinal string fallback
- join (`-j` style)
- projection (`-p` style)
- grouping + count groundwork (`-G` + count)
- sorting (`-S` style)
- uniq (`-U` style)
- collapse (`-C` style)
- include descriptors (`-d` style)

## Current mutation and typed contracts
`DirectFileServerV1` currently exposes:
- `RecSel(...)` and `RecSel_Typed(...)`
- `RecIns(...)` and `RecIns_Typed(...)`
- `RecDel(...)` and `RecDel_Typed(...)`

## Compatibility notes
This project currently implements a **documented supported subset** of recutils-like behavior, not full parity yet.

Use this matrix for exact supported behavior and operation ordering:
- `docs/project/06-recsel-supported-behavior-matrix.md`

## Documentation entry points
- `docs/index.md` - repository documentation index
- `docs/manual/dll/index.md` - DLL manual entry point
- `docs/project/` - phased implementation notes and planning
- `docs/examples/` - shared example data used by docs and tests
