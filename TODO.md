# TODO

Lightweight tracker, not a spec. Check things off as they land.

## Backlog (basic spreadsheet functionality)

- [ ] In-cell error display (`#DIV/0!`, `#REF!`, `#VALUE!`) instead of popup-only
- [ ] Keyboard navigation + inline cell editing
- [ ] Copy / paste, fill-down
- [ ] Basic number formatting
- [ ] Undo / redo
- [ ] Configurable grid size (currently hardcoded 50 rows x 26 cols)
- [ ] Autosave (e.g. localStorage) so closing the tab doesn't lose work
- [ ] Pre-existing gap: `new Formula("A1B2")` doesn't throw -- two operands with no operator between them aren't rejected (Rule 8 only tracks numeric literals via `PreviousIsNumber`, never variables). Found while building function-call grammar rules; unrelated to that work, not fixed yet.

## Done

- [x] Stripped proprietary CS3500 grading test files from repo + git history
- [x] Range references + aggregate functions (`SUM`/`AVERAGE`/`MIN`/`MAX`/`COUNT`): tokenizing, `ExpandRange`, grammar rules, `GetVariables()`, `Evaluate()`, `ToString()`, Spreadsheet-level recalculation + circular-dependency tests. 182 tests passing across the solution. Also fixed a pre-existing precedence bug (`A1*A2+A3` gave 14 instead of 10) found along the way.
