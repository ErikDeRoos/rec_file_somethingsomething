# Copilot Instructions

## Project Guidelines
- User wants architectural notes tracked in markdown files under `docs/architecture`, including versioned COM compatibility rules, single-DLL output focus, forward-compatibility notes for future Linux/Unix support, namespace boundary documentation, parsing best practices, and memory allocation practices.
- In this repository, the docs live at the repository root, not inside the Visual Studio DLL solution root. Use repo-root docs paths going forward.
- Repository documentation should include a general `docs/index.md` file describing what each file is for, and the root `AGENTS.md` must reference that docs index.
- User wants iterative delivery with every commit adding visible working code, a separate test project with small bite-sized unit tests validating decision paths, NuGet packaging and publishing from GitHub with GitHub-friendly markdown/documentation, and a separate .NET desktop editor project in the same repo with its own build/release visibility and documentation.
- For future extensions beyond recutils compatibility, user wants improved file/folder structure support and native run-from-container support (for example zip), but this should not affect current compatibility-first implementation work.

## Documentation Structure
- GitHub-ready DLL documentation should live under `docs/manual/dll`.
- Editor documentation should be located under `docs/manual/editor`.
- Shared example data should be stored under `docs/examples`, starting with `docs/examples/1_simple_singlefile/user.rec` to be used both in documentation and tests. Tests using shared examples should hide file structure behind wrappers, such as enums for example directories/scenarios, to improve readability and maintainability.
- Malformed example cases should live under `docs/examples` using folders named like `<no>_wrong_<whats_wrong>`, and each should clearly explain that the example is intentionally invalid, why it fails, and how to fix it.

## Domain Model Guidelines
- Prefer immutable C# record-based domain model types for record document structures where practical. User prefers record class over struct for now; structs should only be used later for truly performance-critical parts with massive array-based operations on complex data structures.
- If mutation is needed during parsing, use separate builder types to keep runtime behavior predictable.

## Parser Architecture Guidelines
- Parser architecture should be more memory-efficient: prefer stream-based reading, token inspection with cached/preloaded parsing decisions, and avoid whole-text line enumeration when possible.

## API Design Guidelines
- User prefers the DirectFileServerV1 public API to mirror recutils tool-style commands, e.g., direct methods like `RecSel(filename)` or tool-shaped accessors, with outputs similar to recutils command output.
- User prefers not to return raw strings from the main API when avoidable; favor structured response wrappers and potentially reader/stream-style access over heavy DataTable-style contracts by default.
- Prefer a cursor/reader pattern with structured iterable result access for DirectFileServer, including future async support; formatted text is a compatibility/view layer, not the main internal data contract.