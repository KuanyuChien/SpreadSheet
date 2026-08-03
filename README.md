# Spreadsheet

A basic working spreadsheet: cell references, formulas, circular-dependency
detection, JSON file save/load, and a Blazor Server grid UI. Originally built
as a series of University of Utah CS 3500 assignments; now being extended
into a standalone project. Not aiming for a polished UI or feature parity
with Excel — just solid basic functionality.

See [TODO.md](TODO.md) for the current work queue.

## Architecture

| Project | Responsibility |
|---|---|
| `Spreadsheet/DependencyGraph` | Tracks which cells depend on which, for recalculation ordering and circular-reference detection |
| `Spreadsheet/Formula` | Parses/validates/evaluates infix formulas (`=A1+B2*3`) |
| `Spreadsheet/Spreadsheet` | Cell storage, JSON save/load, ties Formula + DependencyGraph together |
| `Spreadsheet/GUI` | Blazor Server front end (grid, save/load, cell coloring, dependency highlighting) |
| `*Tests` projects | Unit tests for each library |

## Status

Builds cleanly on .NET 8 (`dotnet build Spreadsheet/Spreadsheet.sln`).
Currently supported: numbers, strings, and formulas with `+ - * /` and
single-cell references in each cell; no ranges or built-in functions yet
(see TODO).

## Running it

```bash
cd Spreadsheet
dotnet build Spreadsheet.sln
dotnet test Spreadsheet.sln
dotnet run --project GUI
```

The GUI launches at `http://localhost:5168` (see `GUI/Properties/launchSettings.json`).

## Notes

- The original repo's course-provided grading test files were removed
  (from history, not just HEAD) since they carried a "do not redistribute"
  license from the course.
