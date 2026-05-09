# Phase 3 - Query engine and recutils compatibility

## Goal
Expand the shared engine so both direct and SQL-like surfaces can rely on the same querying behavior.

## Scope
- Type-aware sorting based on `%sort`
- Selection expression subset compatible with current `RecSel` options
- Field projection
- Join expansion using `rec <Type>` semantics
- Grouping and aggregate groundwork
- Auto-generated field groundwork for `%auto`

## Current implemented subset
- Shared query layer introduced under `rec_file_lib.Query`
- Selection query options model used by DirectFileServer now covers:
  - index selection (`-n` style)
  - quick substring filtering (`-q` style)
  - expression subset (`-e` subset)
    - `=`, `!=`, `~`, `<`, `<=`, `>`, `>=`
    - boolean composition with `&&`, `||`, `!`
    - parenthesized grouping
    - numeric-aware scalar comparison and ordinal string fallback
  - join (`-j` style inner join)
  - projection (`-p` style)
  - grouping groundwork with count aggregate
  - sorting by field list
  - uniq field deduplication behavior
- Dedicated unit tests for query logic combinations and edge cases
- Explicit tests verify recutils-aligned operation ordering for selection/filtering, join, grouping, sorting, projection, and uniq

## Current operation order
1. Type selection
2. Selection/filtering (indexes, quick, expression)
3. Join expansion
4. Grouping + count groundwork
5. Sorting
6. Projection
7. Uniq
8. Output formatting (outside query engine)

## Notes
- Start with the subset needed by `DirectFileServerV1`
- Reuse the exact same engine later in `SqlFileServerV1`
- Prefer incremental compatibility over a huge parser upfront
- Keep formatting concerns separated from query execution concerns
- Use shared examples from `docs/examples` to validate realistic behavior as features are added

## Exit criteria
- Shared query engine supports the direct API sufficiently
- SQL-like surface can map onto the same operations
- Query behavior has focused unit coverage for logic combinations and edge cases
