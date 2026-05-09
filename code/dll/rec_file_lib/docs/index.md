# Documentation index

This file is the entry point for repository documentation.

## Purpose
Use this index to quickly find where project planning, architecture guidance, and implementation notes are tracked.

## Files and directories

### `docs/index.md`
Primary documentation index for the repository. Start here.

### `docs/project/`
Project planning and phased delivery tracking.

- `docs/project/00-roadmap.md`  
  High-level roadmap, constraints, namespace layout, and phase overview.
- `docs/project/01-phase-foundation.md`  
  Foundation phase scope: model, parser, serializer, and validation groundwork.
- `docs/project/02-phase-direct-file-server-v1.md`  
  Phase plan for the recutils-inspired direct API and COM-facing direct server.
- `docs/project/03-phase-query-engine.md`  
  Query-engine expansion plan: selection, projection, joins, sorting, and aggregates groundwork.
- `docs/project/04-phase-sql-file-server-v1.md`  
  Phase plan for the SQL-like façade and session/metadata behavior.
- `docs/project/05-com-and-unity-notes.md`  
  Notes about COM exposure, Unity integration, result formats, and versioning direction.

### `docs/architecture/`
Architecture notes and long-lived engineering guidance.

- `docs/architecture/00-architecture-principles.md`  
  Core architecture rules: COM versioning, compatibility, single-DLL goal, and platform direction.
- `docs/architecture/01-namespace-boundaries.md`  
  Namespace purposes, responsibilities, and dependency boundaries.
- `docs/architecture/02-parsing-and-memory-practices.md`  
  Parsing best practices, memory allocation practices, and portability notes.

## Usage guidance
- Put phase planning and delivery tracking in `docs/project/`.
- Put stable architectural rules and engineering boundaries in `docs/architecture/`.
- Keep this file updated when new long-lived documentation is added.
