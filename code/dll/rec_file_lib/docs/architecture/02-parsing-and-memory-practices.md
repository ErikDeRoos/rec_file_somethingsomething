# Parsing and memory practices

## Parsing best practices
- Treat parsing as a distinct concern from validation.
- Parse into a faithful internal model first, then validate rules separately.
- Prefer deterministic parsing rules that map closely to the rec format.
- Keep parser behavior explicit when handling ambiguous or unsupported syntax.
- Preserve enough source structure to support stable save behavior where practical.
- Add functionality incrementally: core fields and descriptors first, advanced syntax later.
- Isolate Windows-specific assumptions from parser logic.
- Mark any parser behavior that may complicate future Linux or Unix portability.

## Memory allocation practices
- Prefer simple and maintainable implementations first, then optimize when profiling shows it matters.
- Avoid unnecessary intermediate string allocations during parsing and serialization where straightforward to do so.
- Be careful with repeated concatenation in loops; prefer builders or buffered writing paths where appropriate.
- Keep public COM results simple and bounded to reduce marshalling overhead.
- Reuse internal helper logic where it improves allocation behavior without obscuring the code.
- Avoid premature pooling or overly clever allocation strategies until hotspots are known.
- When introducing caches, document ownership, invalidation, and thread-safety assumptions.

## Forward-compatibility note
- Any optimization that depends heavily on Windows-only runtime behavior should be documented clearly.
- Core parsing and model code should remain as platform-neutral as possible even when hosted today through COM on Windows.
