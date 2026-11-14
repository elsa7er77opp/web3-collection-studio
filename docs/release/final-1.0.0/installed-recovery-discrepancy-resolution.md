# Installed Backup Recovery Discrepancy — Resolution

## Observed Failure

The first Final installer acceptance selected a valid PFP QA project with a deliberately corrupted primary manifest and a valid `project.json.bak`. The Workstation correctly remained local-only but displayed the unrecoverable-project feedback instead of opening the backup.

## Root Cause

The native Windows folder picker provided the selected project root with a terminal directory separator. `LocalCollectionProjectOpener` compared that path literally to the manifest's `LocalRoot`, which is stored without a terminal separator. Manifest reconstruction had already succeeded, but the lexical comparison then raised `InvalidDataException`; the visible workflow therefore reported the project as unrecoverable.

## Test-First Correction

The regression contract `Open_with_outcome_recovers_backup_when_native_folder_selection_has_a_trailing_separator` first failed with:

> `The local collection manifest points to a different project folder.`

The minimal correction normalizes the selected root and manifest root through `Path.TrimEndingDirectorySeparator(Path.GetFullPath(...))` before containment comparison. The contract then passed. Full post-fix regression passed **73/73**: Domain 14, Application 9, Infrastructure 14, and Desktop 36.

## Repackaged Installed Evidence

The Final payload and Inno installer were rebuilt and reinstalled. Opening the same PFP QA root from the installed executable produced these user-visible and local audit results:

| Gate | Observed evidence |
|---|---|
| Project activation | `Final PFP QA` was active in the installed Workstation. |
| Visible recovery notice | `Primary project manifest could not be read. Project restored from local backup.` |
| Local audit | `{"eventType":"project_recovered_from_backup"}` with timestamp only. |
| Data boundary | The audit entry contains no project path and no manifest/source content. |

The primary fixture remains intentionally corrupted for the test. No backup was overwritten, deleted, or automatically promoted over the damaged primary manifest.

## Evidence Files

- `trailing-separator-recovery-red.log`
- `trailing-separator-recovery-green.log`
- `post-recovery-fix-full-regression.log`
- `installed-recovery-feedback.json`
- `installed-recovery-audit.jsonl`
