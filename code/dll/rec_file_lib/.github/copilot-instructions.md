# Copilot Instructions

## Project Guidelines
- User wants architectural notes tracked in markdown files under `docs/architecture`, including versioned COM compatibility rules, single-DLL output focus, forward-compatibility notes for future Linux/Unix support, namespace boundary documentation, parsing best practices, and memory allocation practices.
- Repository documentation should include a general `docs/index.md` file describing what each file is for, and the root `AGENTS.md` must reference that docs index.
- User wants iterative delivery with every commit adding visible working code, a separate test project with small bite-sized unit tests validating decision paths, NuGet packaging and publishing from GitHub with GitHub-friendly markdown/documentation, and a separate .NET desktop editor project in the same repo with its own build/release visibility and documentation.

## Documentation Structure
- GitHub-ready DLL documentation should live under `docs/manual/dll`.
- Editor documentation should be located under `docs/manual/editor`.
- Shared example data should be stored under `docs/examples`, starting with `docs/examples/1_simple_singlefile/user.rec` to be used both in documentation and tests.