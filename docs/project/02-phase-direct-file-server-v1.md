# Phase 2 - DirectFileServerV1

## Goal
Provide the first COM-visible, recutils-inspired API for manipulating rec files directly.

## API direction
Keep the API similar in intent to recutils operations, but COM-safe.

## Current implemented slice
- `RecSel(filePath)`
- `RecSelType(filePath, recordType)`

These methods currently return formatted text as a compatibility layer while the longer-term result direction remains structured cursor or reader-style access.

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
- Select records by type, index, and simple expression
- Join records using foreign-key style fields

## Suggested public contract style
- Tool-shaped operations such as `RecSel`
- Compatibility text output where that helps mirror recutils behavior
- Future cursor or reader-style access for richer result handling
- Explicit operation methods rather than exposing internal object graphs

## Example-driven note
- `docs/examples/1_simple_singlefile/user.rec` validates the single-record-set path
- `docs/examples/8_multiple_record_types_single_file/people_and_residences.rec` validates the meaningful difference between `RecSel` and `RecSelType`
- Tests should validate direct operations against shared example data where practical

## Internal dependencies
- `Model`
- `Parsing`
- `Validation`
- `Query`

## Exit criteria
- DirectFileServerV1 can manipulate a real `.rec` file end-to-end
- Unity-facing COM methods remain simple to consume
- No SQL layer required yet
