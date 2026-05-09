# Phase 2 - DirectFileServerV1

## Goal
Provide the first COM-visible, recutils-inspired API for manipulating rec files directly.

## API direction
Keep the API similar in intent to recutils operations, but COM-safe and options-driven where method explosion would hurt maintainability.

## Current implemented slice
- `RecSel(filePath, options)` with grouped selection/output options covering:
  - type (`-t` style)
  - index selection (`-n` style)
  - quick substring selection (`-q` style)
  - expression subset (`-e` subset: `=`, `!=`, `~`)
  - join (`-j` style, inner join)
  - projection (`-p` style)
  - grouping (`-G` groundwork)
  - count aggregate groundwork
  - sorting (`-S` style, ascending field sorting)
  - collapse (`-C` style output sectioning)
  - uniq (`-U` style field dedupe by name+value)
  - include descriptors (`-d` style output)
- `RecInsType(filePath, recordType, recordText)`
- `RecDelType(filePath, recordType)`

Selection and output options are modeled as COM-visible option classes in `DirectFileServer`.

## Functional scope
- Create or open a rec file
- Save a rec file
- List record types
- Create a record set descriptor
- Alter descriptor metadata
- Insert records
- Replace records
- Delete records
- Add, set, delete, and rename fields
- Select records by type, index, quick substring, expression subset, join, grouping, sorting, and projection

## Suggested public contract style
- Tool-shaped operations such as `RecSel` and `RecInsType`
- Options-based selection input to avoid proliferating method variants
- Compatibility text output where that helps mirror recutils behavior
- Future cursor or reader-style access for richer result handling
- Explicit operation methods rather than exposing internal object graphs

## Example-driven note
- `docs/examples/1_simple_singlefile/user.rec` validates the single-record-set path
- `docs/examples/8_multiple_record_types_single_file/people_and_residences.rec` validates typed selection, indexes, quick filtering, expression subset, join, grouping/count, sorting, collapse, uniq, and include-descriptor combinations
- `docs/examples/2_simple_recutils_book_example/books.rec` and `docs/examples/8_multiple_record_types_single_file/people_and_residences.rec` validate `RecInsType`
- malformed examples under `docs/examples/3_wrong_...` through `7_wrong_...` help verify current insertion and deletion error handling

## Internal dependencies
- `Model`
- `Parsing`
- `Validation`
- `Query`

## Exit criteria
- DirectFileServerV1 can manipulate a real `.rec` file end-to-end
- Unity-facing COM methods remain simple to consume
- Selection methods are consolidated into options-based input
- No SQL layer required yet
