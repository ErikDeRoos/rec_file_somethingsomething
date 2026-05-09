# Examples index

Shared examples in this directory are used for two purposes at the same time:

1. GitHub-facing documentation examples
2. Realistic test inputs

## Rules
- Each example lives in its own folder.
- Each example folder has its own `README.md`.
- Tests should read example files directly from `docs/examples` when validating realistic behavior.

## Current examples
- `docs/examples/1_simple_singlefile/`
  - Minimal single-file example with a typed `User` record set, a comment header, and multiline field data.
- `docs/examples/2_simple_recutils_book_example/`
  - Recutils-inspired books example with descriptor fields, repeated fields, and multiple records.
