# Phase 4 - SqlFileServerV1

## Goal
Add a SQL-like facade over the shared rec engine while keeping all production code inside the same assembly.

## Concept
`SqlFileServerV1` is not a separate storage engine. It is a facade over the same document, descriptor, and query infrastructure used by `DirectFileServerV1`.

## Functional scope
- Attach and detach databases
- Create and close connections or sessions
- Execute SQL-like commands
- Support sys-style metadata queries
- Query record sets as tables
- Insert, update, and delete through SQL-like commands

## Suggested first command set
- `ATTACH DATABASE`
- `DETACH DATABASE`
- `SHOW DATABASES`
- `SHOW TABLES`
- `DESCRIBE <table>`
- `SELECT ... FROM ... WHERE ...`
- `INSERT INTO ...`
- `UPDATE ...`
- `DELETE FROM ...`
- `CREATE TABLE`
- `ALTER TABLE`

## Important design rules
- Keep grammar small at first
- Map commands to internal direct/query operations where possible
- Return simple COM-safe results
- Demonstrate early SQL-like behavior against shared examples before broadening scope

## Exit criteria
- SQL-like interface can inspect and modify rec-backed data
- Metadata queries work across attached databases and connections
