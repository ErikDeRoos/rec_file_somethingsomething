# Phase 2 - DirectFileServerV1

## Goal
Provide the first COM-visible, recutils-inspired API for manipulating rec files directly.

## API direction
Keep the API similar in intent to recutils operations, but COM-safe.

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
- Text input for descriptors and records where practical
- String or JSON output for complex results
- Simple arrays for lists
- Explicit operation methods rather than exposing internal object graphs

## Example-driven note
- The first realistic target should work against `docs/examples/1_simple_singlefile/user.rec`
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
