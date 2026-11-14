# Web3 Collection Studio — Final Edition Plan

## Final Delta

The Alpha implementation already has the important local creative workflow: profile-aware project creation, manifest persistence with atomic replacement and backup, local PNG trait import, Gallery/Compare, preflight, bounded composition, metadata export, local login, Panel manifest, and a tested installer. The release blocker is concentrated rather than broad: the desktop lets the operator **create** a new collection but does not offer a visible command to reopen an existing local project and surface the built-in manifest recovery path.

> Final Edition work will not add wallets, seed phrases, keys, minting, IPFS/cloud upload, marketplace functions, payments, or blockchain calls.

## Required Final Changes

| Gap | Final implementation | Acceptance evidence |
|---|---|---|
| Existing collection recovery is not exposed in the Desktop UX. | Add `Open local collection` using a native folder picker and a thin local manifest opener over the existing `LocalProjectManifestStore.Load` contract. | Desktop test opens a persisted project; corrupted primary manifest restores its local backup; invalid folder produces a local validation state. |
| Panel registration is documented but not reverified from Final payload. | Validate the installed `cyberbank-app.json` alongside the executable/icon and confirm it stays inside the installed folder. | Installer acceptance records manifest/executable/icon existence and local-only registration fields. |
| Alpha metadata still labels the package as an Alpha. | Raise assembly and Inno Setup metadata to Final 1.0.0; rebuild the self-contained x64 payload and installer. | Current build, full tests, installer hash, silent install, and installed launch smoke. |

## Final Acceptance Workflow

The final workflow is fully local: create a local profile; create one project in each PFP, Badge/Gaming, and Digital Art profile; add a local layer and a contained PNG trait; exercise Canvas, Gallery, Compare, preflight, and an idle/blocked render state; reopen a project via the new command; test backup recovery with a harmless corrupt primary manifest; verify local metadata output; then validate the installed Panel manifest fields. Any selected folder outside the local project flow produces a blocked local message rather than a network request or operational action.
