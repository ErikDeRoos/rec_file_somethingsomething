# 8_multiple_record_types_single_file

## Purpose
This example demonstrates that a single rec file can contain multiple typed record sets.

## What it demonstrates
- Multiple `%rec:` blocks in one file
- A `Person` record set and a `Residence` record set in the same `.rec` document
- A simple foreign-key style relationship using `%type: Abode rec Residence`
- A concrete case where `RecSel(filePath)` and `RecSelType(filePath, recordType)` have meaningfully different behavior

## Files
- `people_and_residences.rec` - valid example showing multiple record types in one file

## Usage
- Use this example to validate multi-record-set parsing
- Use this example to validate `RecSelType` filtering behavior
- Use this example later as a join-oriented example when join support is added
