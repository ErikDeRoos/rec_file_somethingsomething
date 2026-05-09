# Documentation index

This file is the entry point for repository documentation.

## Purpose
Use this index to quickly find where project planning, architecture guidance, manuals, and example-driven implementation notes are tracked.

## Files and directories

### `docs/index.md`
Primary documentation index for the repository. Start here.

### `docs/project/`
Project planning and phased delivery tracking.

- `docs/project/00-roadmap.md`  
  High-level roadmap, constraints, namespace layout, phase overview, and current delivery direction.
- `docs/project/01-phase-foundation.md`  
  Foundation phase scope: model, parser, serializer, validation groundwork, and example-driven first milestone.
- `docs/project/02-phase-direct-file-server-v1.md`  
  Phase plan and current status for the COM-facing direct API, including options-based `RecSel(filePath, options)` behavior across type/select/project/group/aggregate/sort output options.
- `docs/project/03-phase-query-engine.md`  
  Query-engine expansion plan and current implemented subset across selection, expression subset, join, grouping/count groundwork, sorting, uniq behavior, and ordering verification.
- `docs/project/04-phase-sql-file-server-v1.md`  
  Phase plan for the SQL-like façade and session/metadata behavior.
- `docs/project/05-com-and-unity-notes.md`  
  Notes about COM exposure, Unity integration, options-based contract shaping, result formats, documentation/manual placement, and versioning direction.
- `docs/project/06-recsel-supported-behavior-matrix.md`  
  Explicitly documented current RecSel supported subset, option interaction ordering, and golden behavior coverage.

### `docs/architecture/`
Architecture notes and long-lived engineering guidance.

- `docs/architecture/00-architecture-principles.md`  
  Core architecture rules: COM versioning, compatibility, single-DLL goal, and platform direction.
- `docs/architecture/01-namespace-boundaries.md`  
  Namespace purposes, responsibilities, and dependency boundaries.
- `docs/architecture/02-parsing-and-memory-practices.md`  
  Parsing best practices, memory allocation practices, portability notes, and example-driven parsing guidance.

### `docs/manual/dll/`
GitHub-ready documentation for the DLL/library itself.

- `docs/manual/dll/index.md`  
  DLL manual entry point.
- `docs/manual/dll/current-capabilities.md`  
  Snapshot of what the DLL supports today.
- `docs/manual/dll/examples.md`  
  Links from the DLL manual into the shared examples.

### `docs/manual/editor/`
GitHub-ready documentation for the future editor application.

- `docs/manual/editor/index.md`  
  Editor manual entry point.
- `docs/manual/editor/roadmap.md`  
  Editor-specific near-term goals and intended relationship to the DLL.

### `docs/examples/`
Shared example data used by documentation and tests.

- `docs/examples/index.md`  
  Example catalog and usage notes.
- `docs/examples/1_simple_singlefile/README.md`  
  Explains the first example and what it is intended to validate.
- `docs/examples/1_simple_singlefile/user.rec`  
  First tangible rec file used by both GitHub documentation and tests.
- `docs/examples/2_simple_recutils_book_example/README.md`  
  Explains the recutils-inspired books example and what it validates.
- `docs/examples/2_simple_recutils_book_example/books.rec`  
  Recutils-derived books example used by documentation and tests.
- `docs/examples/3_wrong_missing_mandatory_field/README.md`  
  Explains the intentionally invalid missing-mandatory-field example and how to fix it.
- `docs/examples/3_wrong_missing_mandatory_field/missing_name.rec`  
  Invalid example showing a `%mandatory` rule violation.
- `docs/examples/4_wrong_duplicate_key_value/README.md`  
  Explains the intentionally invalid duplicate-key example and how to fix it.
- `docs/examples/4_wrong_duplicate_key_value/duplicate_id.rec`  
  Invalid example showing duplicate `%key` values.
- `docs/examples/5_wrong_invalid_field_type/README.md`  
  Explains the intentionally invalid type-mismatch example and how to fix it.
- `docs/examples/5_wrong_invalid_field_type/invalid_status.rec`  
  Invalid example showing a field value that violates a declared type.
- `docs/examples/6_wrong_bad_multiline_continuation/README.md`  
  Explains the intentionally invalid multiline continuation example and how to fix it.
- `docs/examples/6_wrong_bad_multiline_continuation/orphan_continuation.rec`  
  Invalid example showing a continuation line without an active field.
- `docs/examples/7_wrong_missing_field_separator/README.md`  
  Explains the intentionally invalid missing-colon example and how to fix it.
- `docs/examples/7_wrong_missing_field_separator/missing_colon.rec`  
  Invalid example showing a malformed field line without the `:` separator.
- `docs/examples/8_multiple_record_types_single_file/README.md`  
  Explains the valid multiple-record-types single-file example and why it matters.
- `docs/examples/8_multiple_record_types_single_file/people_and_residences.rec`  
  Valid example showing multiple typed record sets in a single rec file.
- `docs/examples/9_int_field_type/README.md`  
  Explains the valid int-typed field example and supported integer literal forms.
- `docs/examples/9_int_field_type/tasks.rec`  
  Valid example showing int-typed field values (decimal, negative, hexadecimal, octal).
- `docs/examples/10_wrong_invalid_int_field_type/README.md`  
  Explains the intentionally invalid int-type mismatch example and how to fix it.
- `docs/examples/10_wrong_invalid_int_field_type/invalid_priority.rec`  
  Invalid example showing a non-integer value in an int-typed field.

## Usage guidance
- Put phase planning and delivery tracking in `docs/project/`.
- Put stable architectural rules and engineering boundaries in `docs/architecture/`.
- Put GitHub-ready user-facing manuals in `docs/manual/dll/` and `docs/manual/editor/`.
- Put shared example data and example readmes in `docs/examples`.
- Tests should read shared examples directly from `docs/examples` when validating realistic behavior.
- Keep this file updated when new long-lived documentation is added.
