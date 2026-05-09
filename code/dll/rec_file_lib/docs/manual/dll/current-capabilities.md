# Current DLL capabilities

This page tracks what the DLL supports today.

## Current documented direction
- Single production DLL
- COM-visible entry points
- Shared internal engine for direct and SQL-like APIs
- Example-driven implementation using files under `docs/examples`
- Tests should read shared examples directly

## First target
The first target is to load, inspect, and save the shared example:
- `docs/examples/1_simple_singlefile/user.rec`

That first example currently focuses on structural parsing concerns:
- comment lines
- `%rec` record-set declaration
- records
- multiline field values
