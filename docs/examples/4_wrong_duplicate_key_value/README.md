# 4_wrong_duplicate_key_value

## Purpose
This example is intentionally invalid.

## What is wrong
The record set declares `%key: Id`, but two records use the same `Id` value.

## Why it fails
According to GNU recutils semantic checking rules, a `%key` field must be unique across the record set.

## How to fix it
Assign a distinct `Id` value to each record, or remove the `%key` rule if uniqueness is not required.

## Files
- `duplicate_id.rec` - invalid example showing duplicate key values
