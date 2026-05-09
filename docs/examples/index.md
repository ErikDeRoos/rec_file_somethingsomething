# Examples index

Shared examples in this directory are used for two purposes at the same time:

1. GitHub-facing documentation examples
2. Realistic test inputs

## Rules
- Each example lives in its own folder.
- Each example folder has its own `README.md`.
- Tests should read example files directly from `docs/examples` when validating realistic behavior.
- Intentionally invalid examples should clearly state that they are wrong, why they fail, and how to fix them.

## Current examples
- `docs/examples/1_simple_singlefile/`
  - Minimal single-file example with a typed `User` record set, a comment header, and multiline field data.
- `docs/examples/2_simple_recutils_book_example/`
  - Recutils-inspired books example with descriptor fields, repeated fields, and multiple records.
- `docs/examples/3_wrong_missing_mandatory_field/`
  - Intentionally invalid example showing a `%mandatory` rule violation.
- `docs/examples/4_wrong_duplicate_key_value/`
  - Intentionally invalid example showing duplicate `%key` values.
- `docs/examples/5_wrong_invalid_field_type/`
  - Intentionally invalid example showing a field value that violates a declared type.
- `docs/examples/6_wrong_bad_multiline_continuation/`
  - Intentionally invalid example showing a continuation line without a current field.
- `docs/examples/7_wrong_missing_field_separator/`
  - Intentionally invalid example showing a malformed field line without the `:` separator.
- `docs/examples/8_multiple_record_types_single_file/`
  - Valid example showing multiple typed record sets in a single rec file.
- `docs/examples/9_int_field_type/`
  - Valid example showing `int` field type values in decimal, negative, hexadecimal, and octal forms.
- `docs/examples/10_wrong_invalid_int_field_type/`
  - Intentionally invalid example showing a value that violates a declared `int` field type.
- `docs/examples/11_repeated_fields_backtracking/`
  - Valid example showing repeated fields that are expected to participate in expression backtracking.
