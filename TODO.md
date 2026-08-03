# TODO

Lightweight tracker, not a spec. Check things off as they land.

## Now: Range references + aggregate functions

- [x] Tokenizer: recognize range syntax (`A1:B3`) (`Formula.GetTokens`, 16 unit tests)
- [x] Helper: expand a range into the list of cell names it covers (`Formula.ExpandRange`, 10 unit tests)
- [ ] Tokenizer/parser: recognize function calls `SUM(...)`, `AVERAGE(...)`, `MIN(...)`, `MAX(...)`, `COUNT(...)`
- [ ] `GetVariables()`: include every cell referenced via a range/function arg
- [ ] `Evaluate()`: compute function calls over the expanded arguments
- [ ] `ToString()`: canonical form for ranges + function calls
- [ ] Spreadsheet-level test: `=SUM(A1:A3)` recalculates correctly; circular deps through a range are still caught

## Backlog (basic spreadsheet functionality)

- [ ] In-cell error display (`#DIV/0!`, `#REF!`, `#VALUE!`) instead of popup-only
- [ ] Keyboard navigation + inline cell editing
- [ ] Copy / paste, fill-down
- [ ] Basic number formatting
- [ ] Undo / redo
- [ ] Configurable grid size (currently hardcoded 50 rows x 26 cols)
- [ ] Autosave (e.g. localStorage) so closing the tab doesn't lose work

## Done

- [x] Stripped proprietary CS3500 grading test files from repo + git history
