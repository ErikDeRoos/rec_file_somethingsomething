# Architecture principles

## Versioned COM contract rules
- Public exposed COM libraries must keep both interface shape and observable behavior as compatible as possible across versions.
- `V1` interfaces and their implementations are treated as stable contracts.
- Breaking behavioral or contract changes must result in a new interface and implementation version such as `V2`.
- Internal code may be reused across versions, but compatibility-specific behavior must be controlled explicitly through version-aware logic and not by accident.
- Compatibility requirements apply both backward and forward within the supported versioning model.

## Single production assembly goal
- The project output should stay focused on one primary DLL for production use.
- The primary production DLL must remain COM accessible.
- Internal architectural layering should therefore be implemented inside the same production project using namespaces, folders, `internal` types, and strict boundaries.

## Platform direction
- The current delivery target is the Windows ecosystem.
- COM-specific integration is therefore acceptable in the public surface for now.
- However, anything that may complicate later Linux or Unix support should be called out explicitly in code and design notes.
- Windows-only assumptions should be isolated as much as possible so the core parsing, model, validation, and query logic can later be reused in non-COM environments.

## Shared engine rule
- `DirectFileServerV1` and `SqlFileServerV1` must share the same internal engine where possible.
- The direct API and SQL-like API are different façades over the same rec-oriented document and query capabilities.
- Duplicate storage logic should be avoided.

## Example and manual rule
- Shared example data should live under `docs/examples` and be suitable for both documentation and tests.
- Each example folder should include a `README.md` explaining what the example demonstrates.
- GitHub-ready DLL documentation belongs under `docs/manual/dll`.
- GitHub-ready editor documentation belongs under `docs/manual/editor`.
