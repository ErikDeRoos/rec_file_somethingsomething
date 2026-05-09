# Phase 3 - Query engine and recutils compatibility

## Goal
Expand the shared engine so both direct and SQL-like surfaces can rely on the same querying behavior.

## Scope
- Type-aware sorting based on `%sort`
- Simple selection expressions
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
  - join (`-j` style inner join)
  - projection (`-p` style)
  - grouping groundwork with count aggregate
  - sorting by field list
  - uniq field deduplication behavior
- Dedicated unit tests for query logic combinations and edge cases
- Explicit tests verify recutils-aligned operation ordering for selection/filtering, grouping, sorting, and projection

## Priority order
1. Type selection
2. Index selection
3. Basic predicates
4. Projection
5. Join expansion
6. Sorting
7. Aggregate groundwork

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
