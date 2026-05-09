# DLL examples

The DLL manual uses shared example data from `docs/examples`.

## Valid examples
### `docs/examples/1_simple_singlefile/user.rec`
This intentionally small example demonstrates:
- `%rec: User`
- a leading comment line
- two records
- multiline `Notes` field values

### `docs/examples/2_simple_recutils_book_example/books.rec`
This recutils-inspired example demonstrates:
- `%rec: Book`
- comment lines before and after the record set
- descriptor fields such as `%mandatory`, `%type`, and `%doc`
- repeated fields such as multiple `Author` entries
- several records in a single file

## Intentionally invalid examples
The documentation and tests also use intentionally invalid examples to show what goes wrong and why.

- `docs/examples/3_wrong_missing_mandatory_field/`
- `docs/examples/4_wrong_duplicate_key_value/`
- `docs/examples/5_wrong_invalid_field_type/`
- `docs/examples/6_wrong_bad_multiline_continuation/`
- `docs/examples/7_wrong_missing_field_separator/`

These malformed examples help define the current parser-versus-validator boundary and are intended to grow as validation support is added.
