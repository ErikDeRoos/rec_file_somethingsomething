# 7_wrong_missing_field_separator

## Purpose
This example is intentionally invalid.

## What is wrong
The line `%key Id` is missing the colon separator between the field name and its value.

## Why it fails
This is a syntactical error. GNU recutils expects field lines to use the `FieldName: value` form.

## How to fix it
Change `%key Id` to `%key: Id`.

## Files
- `missing_colon.rec` - invalid example showing a malformed field line
