# Copilot Instructions

## Project Guidelines
- Prefer fluent LINQ method syntax (`source.Where(...).Select(...)`) over query expression syntax (`from ... in ...`) in this codebase.
- Prefer test method names without the ending `Async`, even when the test methods return Task.
