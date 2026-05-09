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

## Exit criteria
- Shared query engine supports the direct API sufficiently
- SQL-like surface can map onto the same operations
