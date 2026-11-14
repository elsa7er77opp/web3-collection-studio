# NFT Studio Final 1.0.0 Implementation Plan

> **For Manus:** Execute this plan task-by-task with test-first RED/GREEN evidence. Do not add wallet, seed phrase, private key, minting, contract, blockchain, IPFS/cloud, marketplace, payment, trading, or investment functionality.

**Goal:** Deliver a truthful Windows Final 1.0.0 release of CyberBank NFT Studio by making local collection reopening and recovery visible, proving all three local profile workflows from the installed executable, validating the local Panel payload, and producing fresh release evidence.

**Architecture:** The Final release continues as a native Avalonia/.NET local-first workstation. Local project state remains under the project root and is loaded only after the user explicitly chooses a folder. Recovery is represented as a typed local result rather than a silent side effect, enabling the Workstation to show deterministic success, recovery, invalid, and unrecoverable states without source execution or network activity.

**Tech stack:** C#/.NET 10, Avalonia 12, CommunityToolkit.Mvvm, xUnit, Inno Setup, native Windows folder pickers, self-contained `win-x64` publish.

---

## Scope and Non-Negotiable Boundaries

The release covers local PFP, Badge/Gaming, and Digital Art collections; local manifests; ordered PNG layers and traits; weights; Require/Exclude compatibility rules; preflight; bounded local generation; cancellation; Gallery/Compare; local metadata; local audit events; and local Panel registration. It must not add a wallet, seed/private-key processing, blockchain interactions, minting, IPFS/cloud upload, marketplace publication, payment, trading, token pricing, or investment content.

## Current Evidence-Based Gap Analysis

| Required Final behavior | Current evidence | Final action |
|---|---|---|
| Visible project reopening | `LocalCollectionProjectOpener` tests can open, recover backup, and reject invalid roots; MainWindow has an unbound `Open local project` button. | Bind the button to a native picker and a typed Desktop open workflow. |
| Visible recovery feedback | `LocalProjectManifestStore.Load` silently reads backup and overwrites the damaged primary manifest. | Introduce a non-destructive typed recovery result and surface a clear banner/status/audit event. |
| Persist and restore weights/rules/state | Traits persist weights, but their IDs are not persisted; `CollectionProject` does not store rules or workspace selections. | Complete the existing product contract: persist trait identity/weights, Require/Exclude rules, selected layer/asset comparison state, and relevant local workspace state. |
| User editing of weights/rules | Trait import currently sets weight to 100; no verified visible rule editor exists. | Add the narrow local inspector controls needed to edit weight and add/remove Require/Exclude rules. |
| Final Panel identity | `cyberbank-app.json` and installer are Alpha-labelled. | Change all user-visible package/manifest versions and revalidate installed-file confinement. |
| Final proof | Alpha installer evidence exists only. | Produce new tests, publish, installer, installed E2E, hash, and `docs/release/final-1.0.0/` evidence. |

## Task 1: Establish Final Baseline and Evidence Layout

**Files:**
- Create: `docs/release/final-1.0.0/README.md`
- Create: `docs/release/final-1.0.0/final-tests.md`
- Modify: `todo.md`

1. Record the exact pre-Final artifact/version and the local-only boundary.
2. Create an evidence index with a table for build, tests, installer, manifest, installed login, open/recovery, invalid root, each profile, output integrity, cancellation, final E2E, and known limitations.
3. Record a release gate as **NOT VERIFIED** until every required acceptance item has evidence.
4. Do not reuse Alpha test totals, Alpha hashes, or Alpha screenshots as Final evidence.

## Task 2: Make Persisted Collection State Complete

**Files:**
- Modify: `src/Web3CollectionStudio.Domain/Layers/TraitOption.cs`
- Modify: `src/Web3CollectionStudio.Domain/Layers/CollectionLayer.cs`
- Modify: `src/Web3CollectionStudio.Domain/Collections/CollectionProject.cs`
- Modify: `src/Web3CollectionStudio.Domain/Rules/CompatibilityRule.cs` only if a stable serializable rule reference is required
- Modify: `src/Web3CollectionStudio.Infrastructure/ProjectStorage/LocalProjectManifestStore.cs`
- Create/modify tests under: `tests/Web3CollectionStudio.Domain.Tests/` and `tests/Web3CollectionStudio.Infrastructure.Tests/`

### RED/GREEN sequence

1. Write a failing domain test proving a trait keeps its stable ID and an edited non-negative weight.
2. Run only that test; record RED because current load recreates IDs and has no editing operation.
3. Add the minimum domain API for updating a local trait weight while preserving its identity and relative PNG path.
4. Rerun the test for GREEN.
5. Write a failing persistence test creating a project with weights, Require/Exclude rules, selected layer, selected compare assets, and workspace mode; save/load it and assert exact restoration.
6. Run RED. Then expand the manifest document in a backward-compatible schema version, preserving v1/v2 read support and adding no external paths.
7. Rerun GREEN and all affected Domain/Infrastructure tests.

**Success criteria:** Loaded projects have the same profile, canvas, ordered layers, traits, trait IDs, weights, rules, and documented local workspace selections. All paths remain relative and project-contained.

## Task 3: Use a Typed, Non-Destructive Manifest Load Result

**Files:**
- Modify: `src/Web3CollectionStudio.Infrastructure/ProjectStorage/LocalProjectManifestStore.cs`
- Modify: `src/Web3CollectionStudio.Desktop/Services/LocalCollectionProjectOpener.cs`
- Create/modify: `tests/Web3CollectionStudio.Infrastructure.Tests/*ManifestStore*Tests.cs`
- Modify: `tests/Web3CollectionStudio.Desktop.Tests/LocalCollectionProjectOpenerContractTests.cs`

### RED/GREEN sequence

1. Write a failing Infrastructure test expecting an explicit `PrimaryLoaded` outcome for a healthy manifest.
2. Write a failing test expecting `RecoveredFromBackup` for corrupt primary plus valid backup, with the damaged primary and backup bytes unchanged after load.
3. Write a failing test expecting an unrecoverable local result/exception for corrupt primary without usable backup.
4. Run RED and confirm failures concern the absent typed outcome or current primary overwrite.
5. Implement a minimal typed load result consumed by the Desktop opener. Preserve a compatibility `Load` method only where legacy callers require it.
6. Record a safe local recovery audit event with no source contents, raw asset data, or network activity.
7. Run the focused tests to GREEN, then the complete Infrastructure and Desktop suites.

**Success criteria:** Recovery is explicit and non-destructive. No invalid/corrupt root creates a project, deletes files, or silently rewrites the primary manifest.

## Task 4: Add Visible Open Local Collection Workflow

**Files:**
- Modify: `src/Web3CollectionStudio.Desktop/Views/MainWindow.axaml`
- Modify: `src/Web3CollectionStudio.Desktop/Views/MainWindow.axaml.cs`
- Modify: `src/Web3CollectionStudio.Desktop/ViewModels/MainViewModel.cs`
- Modify/create: `tests/Web3CollectionStudio.Desktop.Tests/*OpenLocalCollection*Tests.cs`

### RED/GREEN sequence

1. Write a failing Desktop ViewModel contract for `Opening`, `Opened`, `RecoveredFromBackup`, `InvalidProject`, and `UnrecoverableProject` user-visible states.
2. Run RED and confirm state projection is absent.
3. Add minimal ViewModel state methods/messages; reuse the existing top command strip and status/preflight regions rather than redesigning the workstation.
4. Rerun GREEN.
5. Write a failing handler/service contract for native folder selection calling `LocalCollectionProjectOpener`, activating the loaded project only on success, and leaving the current project untouched on failure/cancel.
6. Run RED, bind `Open local project` to `OpenLocalCollection_Click`, use `StorageProvider.OpenFolderPickerAsync`, and implement the smallest exception/result mapping.
7. Rerun GREEN and confirm the UI binds loading, success, invalid, recovered, and unrecoverable messages.

**Required visible wording:** a recovery state must clearly say that the primary project manifest could not be read and that the project was restored from a local backup. Invalid and unrecoverable state must explicitly state no project was opened and no project was created.

## Task 5: Complete Local Trait Weight and Require/Exclude Editing

**Files:**
- Modify: `src/Web3CollectionStudio.Desktop/Services/LocalProjectEditor.cs`
- Modify: `src/Web3CollectionStudio.Desktop/ViewModels/MainViewModel.cs`
- Modify: `src/Web3CollectionStudio.Desktop/Views/MainWindow.axaml`
- Modify: `src/Web3CollectionStudio.Desktop/Views/MainWindow.axaml.cs`
- Create/modify: `tests/Web3CollectionStudio.Desktop.Tests/*Trait*Tests.cs`

### RED/GREEN sequence

1. Write a failing editor contract for changing a selected local trait’s weight and retaining it after reopen.
2. Write a failing contract for adding a Require and an Exclude rule between contained trait IDs and retaining both after reopen.
3. Run RED. Add only contained-project validation and explicit duplicate/invalid-rule feedback.
4. Add narrow inspector controls for editing weight and creating/removing rules. They must be disabled without a local active project and show local validation errors, not generic success.
5. Run GREEN plus preflight tests to confirm rules affect validation but never trigger remote actions.

**Success criteria:** Each profile can set a weight and at least one Require or Exclude rule from the visible Workstation, then close/reopen and retain state.

## Task 6: Test Local Recovery and Invalid-Root Desktop Acceptance

**Files:**
- Create: `docs/release/final-1.0.0/fixtures/README.md`
- Create: `docs/release/final-1.0.0/open-local-collection.md`
- Create: `docs/release/final-1.0.0/backup-recovery.md`
- Create: `docs/release/final-1.0.0/invalid-folder.md`
- Create safe fixture scripts under `docs/release/final-1.0.0/` only

1. Create a harmless local PFP fixture through the product, verify primary and backup manifests exist, then corrupt only the primary manifest text.
2. From the **installed** Final executable, use Open Local Collection and select that root. Capture the visible recovery message and project state.
3. Test empty, random, manifest-free, inaccessible (if safely reproducible), and corrupt-with-no-backup roots. Capture only real UI states; no fabricated screenshot is acceptable.
4. Verify no project is created or deleted, existing active project remains intact, and a recovery audit event is written safely.

## Task 7: Three-Profile Installed Acceptance Matrix

**Files:**
- Create: `docs/release/final-1.0.0/pfp-acceptance.md`
- Create: `docs/release/final-1.0.0/badge-gaming-acceptance.md`
- Create: `docs/release/final-1.0.0/digital-art-acceptance.md`
- Create: `docs/release/final-1.0.0/output-integrity.md`

For **PFP**, **Badge/Gaming**, and **Digital Art**, execute from the installed EXE only:

1. Create a local project and confirm profile canvas/defaults.
2. Add layer(s); import safe local PNG trait(s); set weights; add Require or Exclude rule.
3. Run preflight and record both valid and controlled-invalid feedback where practical.
4. Start bounded local generation, capture progress, cancel a run, verify no incomplete final output, then complete a later run.
5. Open Gallery and Compare; inspect output PNGs and relative-path metadata.
6. Close/reopen application, use Open Local Collection, and prove project state persistence.

Each line in each matrix must be **PASS** or **FAIL** with an evidence path; no implied result is allowed.

## Task 8: Cancellation, Restart, and Output Integrity

**Files:**
- Create: `docs/release/final-1.0.0/cancellation-recovery.md`
- Modify/add focused tests under `tests/Web3CollectionStudio.Application.Tests/` and `tests/Web3CollectionStudio.Infrastructure.Tests/`

1. Add failing automated contracts for a cancelled render leaving no promoted incomplete output and for a later render succeeding after cancellation.
2. Record RED. Implement the smallest atomic cleanup/state fix only if existing contracts expose a defect.
3. Run GREEN and full affected suites.
4. From installed EXE, cancel mid-run; confirm responsive UI, stopped queue, no phantom running job, no invalid final file, then restart app and complete a subsequent run.
5. Verify output stays local, PNG count matches completed generation, metadata image paths are relative, and no cloud/IPFS/marketplace references appear. Hash useful final PNG/JSON artifacts.

## Task 9: Final Identity and Panel Payload

**Files:**
- Modify: `src/Web3CollectionStudio.Desktop/cyberbank-app.json`
- Modify: `installer/Web3CollectionStudio.iss`
- Modify: desktop project version metadata in `src/Web3CollectionStudio.Desktop/*.csproj` or current version source
- Create: `docs/release/final-1.0.0/panel-validation.md`

1. Change user-visible version labels from Alpha 0.1.0 to Final 1.0.0; retain historic Alpha documents untouched.
2. Update Inno `AppVersion`, Final publish root, and Final installer filename to `CyberBank-Web3-Collection-Studio-Final-Setup-win-x64.exe`.
3. Update Panel manifest version to `1.0.0`; retain only relative exe/icon paths.
4. Validate in the installed payload that manifest, executable, and icon exist and all resolved paths remain under the install root.
5. If CyberBank Panel is available, register/launch the installed Final entry and record product name/icon/launch. If unavailable, record structural validation as PASS and Panel launch as PENDING; never infer a Panel launch pass.

## Task 10: Regression, Final Build, Installer, and Installed E2E

**Files:**
- Create: `docs/release/final-1.0.0/final-build.md`
- Create: `docs/release/final-1.0.0/final-tests.md`
- Create: `docs/release/final-1.0.0/final-installer.md`
- Create: `docs/release/final-1.0.0/final-installer-sha256.txt`
- Create: `docs/release/final-1.0.0/final-e2e.md`
- Create: `docs/release/final-1.0.0/known-limitations.md`
- Create: `docs/release/final-1.0.0/release-notes.md`

1. Run the full `Web3CollectionStudio.slnx` regression. Capture exact passed/failed/skipped totals. Stop and fix any release-critical failure before packaging.
2. Publish a fresh `Release`, self-contained `win-x64` payload. Capture command, warnings, and errors. The installed run may not depend on the source tree or a developer launch command.
3. Compile the Final Inno installer. Record filename, version, size, date, and SHA-256.
4. Install the Final installer in the current-user location. Validate actual EXE, `cyberbank-app.json`, and icon existence.
5. Launch the installed EXE and run final local account setup/sign-in → PFP create/generate/export → close → reopen → Open Local Collection persistence/recovery smoke.
6. Capture only genuine installed UI evidence: `installed-login.png`, `final-workstation.png`, `open-local-collection.png`, `backup-recovery.png`, and `invalid-folder.png`.

## Task 11: Final Decision

Create `docs/release/final-1.0.0/release-decision.md` with exactly one status:

```text
NFT STUDIO FINAL 1.0.0 — VERIFIED
```

or

```text
NFT STUDIO FINAL 1.0.0 — NOT VERIFIED
```

Use **VERIFIED** only after every requirement in Tasks 3–10 has PASS evidence, the installed E2E is real, a fresh Final SHA-256 is recorded, no release-blocking defect remains, and the scope boundary has remained local-only. Otherwise use **NOT VERIFIED** and enumerate the blocking evidence.

## Rollback Rule

Before any significant domain/persistence schema or packaging change, preserve a project checkpoint. If a persistence migration, desktop behavior, or package update cannot be repaired safely, return to the most recent verified checkpoint rather than keeping an uncertain Final candidate.
