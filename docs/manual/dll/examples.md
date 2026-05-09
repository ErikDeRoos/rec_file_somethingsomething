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
- `RecInsType(filePath, "Book", recordText)` insertion into a single typed record set

### `docs/examples/8_multiple_record_types_single_file/people_and_residences.rec`
This example demonstrates:
- multiple `%rec:` blocks in one file
- a `Person` record set and a `Residence` record set in the same document
- a simple foreign-key style relationship using `%type: Abode rec Residence`
- a concrete case where `RecSel(filePath)` and `RecSelType(filePath, recordType)` behave differently
- `RecInsType(filePath, recordType, recordText)` inserting into one selected record type without affecting the other

## Intentionally invalid examples
The documentation and tests also use intentionally invalid examples to show what goes wrong and why.

- `docs/examples/3_wrong_missing_mandatory_field/`
- `docs/examples/4_wrong_duplicate_key_value/`
- `docs/examples/5_wrong_invalid_field_type/`
- `docs/examples/6_wrong_bad_multiline_continuation/`
- `docs/examples/7_wrong_missing_field_separator/`

These malformed examples help define the current parser-versus-validator boundary and now also help validate `RecInsType` error handling.
