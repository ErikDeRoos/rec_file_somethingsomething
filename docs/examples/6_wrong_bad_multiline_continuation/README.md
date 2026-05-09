# 6_wrong_bad_multiline_continuation

## Purpose
This example is intentionally invalid.

## What is wrong
The file starts a continuation line with `+` before any field value has been opened.

## Why it fails
This is a syntactical error. Continuation lines are only valid immediately after a field whose value is being continued.

## How to fix it
Add a proper field before the continuation line, or convert the text into a normal field line.

## Files
- `orphan_continuation.rec` - invalid example showing a continuation line without a current field
