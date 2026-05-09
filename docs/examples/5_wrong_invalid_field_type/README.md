# 5_wrong_invalid_field_type

## Purpose
This example is intentionally invalid.

## What is wrong
The field `Status` is declared as `%type: Status enum open closed`, but the record uses `Status: pending`.

## Why it fails
According to GNU recutils semantic checking rules, field values must conform to the declared type. Here the enum does not include `pending`.

## How to fix it
Change the field value to one of the allowed enum values such as `open` or `closed`, or update the enum definition if `pending` should be supported.

## Files
- `invalid_status.rec` - invalid example showing a field value that does not match its declared type
