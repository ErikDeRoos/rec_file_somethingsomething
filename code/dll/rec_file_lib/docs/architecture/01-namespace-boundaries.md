# Namespace boundaries

## `rec_file_lib.Common`
### Purpose
Shared low-level helpers, constants, guard utilities, and small reusable primitives.

### Can do
- Provide general helper logic used by multiple namespaces.
- Provide shared constants and utility abstractions.

### Cannot do
- Own business rules for rec parsing or querying.
- Depend on COM-facing namespaces.

## `rec_file_lib.Model`
### Purpose
Own the in-memory representation of rec data and descriptor metadata.

### Can do
- Represent documents, record sets, descriptors, records, fields, and type metadata.
- Express stable internal domain concepts.

### Cannot do
- Perform file I/O directly.
- Depend on COM-facing API contracts.
- Contain UI, transport, or shell concerns.

## `rec_file_lib.Parsing`
### Purpose
Parse rec text into model structures and serialize model structures back to rec text.

### Can do
- Read and write rec format text.
- Handle multiline fields, descriptors, records, and related syntax.

### Cannot do
- Own query semantics.
- Depend on COM-facing concerns.
- Perform business-level mutation orchestration.

## `rec_file_lib.Validation`
### Purpose
Validate model instances against descriptor rules and structural expectations.

### Can do
- Enforce `%mandatory`, `%allowed`, `%prohibit`, `%unique`, `%key`, `%type`, `%sort`, `%auto`, and related rules as implemented.
- Return structured validation results.

### Cannot do
- Mutate documents as a side effect of validation unless explicitly designed for repair operations.
- Depend on COM or SQL façade types.

## `rec_file_lib.Query`
### Purpose
Provide record selection, projection, joins, sorting, and later aggregate behavior.

### Can do
- Execute selection and transformation logic over the model.
- Provide shared behavior used by direct and SQL-like APIs.

### Cannot do
- Parse SQL command text directly unless a dedicated SQL parsing sub-area is explicitly introduced.
- Depend on COM contract types.

## `rec_file_lib.DirectFileServer`
### Purpose
Provide the recutils-inspired direct API surface.

### Can do
- Coordinate document operations using parsing, model, validation, and query namespaces.
- Expose COM-visible direct manipulation methods.

### Cannot do
- Reimplement parser, validation, or core query rules locally.
- Bypass shared engine rules without explicit justification.

## `rec_file_lib.SqlFileServer`
### Purpose
Provide the SQL-like façade over the same shared engine.

### Can do
- Manage attachments, sessions, metadata inspection, and SQL-like commands.
- Translate SQL-like requests into shared document/query operations.

### Cannot do
- Become a separate storage engine.
- Duplicate core model, parsing, or validation logic.

## `rec_file_lib.Interop`
### Purpose
Hold interop-specific concerns, COM adaptation helpers, marshalling-facing DTOs, and compatibility helpers.

### Can do
- Isolate COM-related implementation details.
- Provide adapter shapes between public COM contracts and internal engine operations.

### Cannot do
- Own domain logic for parsing, validation, or queries.

## Boundary rule summary
- Dependencies should generally flow inward toward `Common`, `Model`, `Parsing`, `Validation`, and `Query`.
- COM-facing namespaces should orchestrate, not own, core logic.
- Windows-specific concerns should be isolated so future non-Windows hosting remains practical.
