# 3_wrong_missing_mandatory_field

## Purpose
This example is intentionally invalid.

## What is wrong
The record set declares `%mandatory: Name`, but one of the records does not contain a `Name` field.

## Why it fails
According to GNU recutils semantic checking rules, `%mandatory` requires the named field to be present in every record of the record set.

## How to fix it
Add a `Name` field to every record in the file, or remove the `%mandatory: Name` rule if that requirement is not intended.

## Files
- `missing_name.rec` - invalid example showing a missing mandatory field
