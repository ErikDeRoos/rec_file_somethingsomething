# 10_wrong_invalid_int_field_type

## Purpose
This example is intentionally invalid.

## What is wrong
The field `Priority` is declared as `%type: Priority int`, but the record uses `Priority: high`.

## Why it fails
According to GNU recutils semantic checking rules, `int` fields must contain integer-compatible values.

## How to fix it
Replace `high` with a valid integer value such as `1`, `-3`, `0x10`, or `020`.

## Files
- `invalid_priority.rec` - invalid example showing a value incompatible with `int`
