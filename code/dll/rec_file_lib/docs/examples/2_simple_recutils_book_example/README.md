# 2_simple_recutils_book_example

## Purpose
This shared example mirrors the simple books example from the GNU recutils manual.

## What it demonstrates
- A typed record set using `%rec: Book`
- Comment lines before and after the record set
- Descriptor fields such as `%mandatory`, `%type`, and `%doc`
- Repeated fields such as multiple `Author` entries
- Several simple records in a single file

## Files
- `books.rec` - the recutils-inspired books example used by documentation and tests

## Usage
- Refer to this example from DLL documentation
- Use this example as direct test input from the test project
- Use this example to validate descriptor parsing, repeated fields, and round-trip behavior
