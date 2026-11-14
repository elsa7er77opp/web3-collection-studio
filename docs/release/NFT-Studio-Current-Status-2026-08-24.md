# CyberBank NFT Studio — Current Status Report

**Report date:** 2026-08-24  
**Product:** CyberBank Web3 Collection Studio / NFT Studio  
**Current release state:** **Windows Alpha 0.1.0**  
**Final Edition state:** **Paused; not ready to be declared Final**

> NFT Studio is a local-first creative collection workstation. It is not a wallet, a minting application, a blockchain publisher, an IPFS/cloud uploader, a marketplace, or a trading product.

## Executive Status

The Windows Alpha is a substantive native desktop product rather than a static mockup. It has a self-contained Windows x64 installer, local account gate, three collection profiles, manifest persistence and backup recovery, local PNG trait workflows, preflight, bounded local composition, local metadata output, and a four-column workstation bound to real local project state. The original Alpha install and launch defects were repaired and the installer was revalidated. [1]

The product has not been promoted to Final because the remaining work is concentrated in a small release delta: the Desktop still lacks a visible **Open local collection** command, the existing manifest-backup recovery has not yet been accepted through that visible workflow, the installed Panel manifest bundle has not been revalidated as a Final payload, and version/package evidence remains Alpha 0.1.0. [2]

| Area | Current state | Status |
|---|---|---|
| Native Windows packaging | Per-user Inno Setup installer; self-contained .NET 10 x64 publish; installed login smoke documented. | **Alpha evidence passed** |
| Local security | Local account setup/sign-in with password hashing and local state; no remote account claim. | **Implemented** |
| Collection workflow | PFP, Badge/Gaming, and Digital Art creation, profile defaults, projects, layers, PNG traits, Canvas/Gallery/Compare. | **Implemented** |
| Local generation | Preflight, bounded queue/progress/cancellation, atomic local PNG composition, relative-path metadata export. | **Implemented** |
| Recovery | Manifest v2 persistence, backup creation, and backup fallback exist. | **Implemented; Desktop reopening UX pending** |
| Panel integration | Explicit local `cyberbank-app.json` is shipped and constrained to installed files. | **Alpha evidence passed; Final revalidation pending** |
| Final release | Final versioning, fresh build/test/install/hash evidence, and Final acceptance have not been completed. | **Not ready** |

## Implemented Product Capabilities

NFT Studio creates and manages local collection projects only. A project can use one of three profiles—**PFP**, **Badge/Gaming**, or **Digital Art**—and persists local identity and profile information in a versioned manifest. The project workflow supports ordered layers, trait options and weights, local PNG import subject to project-root containment, compatibility Require/Exclude rules, duplicate fingerprints, and preflight validation of impossible or unsafe local configurations. [1]

The desktop Workstation exposes real state for local profile, canvas, imported traits, selected layers, Gallery and Compare modes, preflight, and a bounded queue. Rendering stays local, reads only approved local assets, uses atomic output, and writes OpenSea-style JSON metadata with relative image paths rather than cloud references. The workflow includes cancellation and local audit events. [1]

| User-visible area | Delivered behavior |
|---|---|
| Local account gate | First-run local account creation followed by local sign-in. No wallet or hosted identity requirement. |
| New collection | Creates local PFP, Badge/Gaming, or Digital Art projects with profile-aware defaults. |
| Collection Explorer | Shows local layers, selected assets, and trait state bound to active project data. |
| Canvas / Gallery / Compare | Switches among local creative review modes without fabricated marketplace analytics. |
| Preflight | Reports invalid assets, incompatible rules, and unique-output issues before local generation. |
| Local output | Runs a bounded local queue, tracks state/progress/cancellation, composes PNGs atomically, and exports local metadata. |

## Windows Alpha Evidence and Delivery Location

The current installer is **`CyberBank-Web3-Collection-Studio-Alpha-Setup-win-x64.exe`**, stored at `artifacts/installer/`. Its documented SHA-256 is:

```text
9E576DE154FEB9EF063B0C8B186193A47C6F90A8BB7CA12BF86EF1C1720EAC7C
```

The Alpha evidence records successful solution tests, self-contained x64 publish, Inno Setup compilation, silent per-user installation, installed executable and `cyberbank-app.json` presence, repaired local-login launch, and functional acceptance across the three profiles. [1]

The Alpha package is **unsigned**. It can therefore prompt Windows reputation or publisher warnings. This is disclosed in the release note and has not been represented as code-signed. [1]

## Final Edition Work: Exact Remaining Delta

The Final work is intentionally bounded. It must not introduce wallets, seed phrases, private keys, contract deployment, minting, blockchain publication, IPFS/cloud upload, marketplace functions, payments, trading, or investment claims. [2]

| Final gate | What already exists | What remains |
|---|---|---|
| Reopen a local project | `LocalProjectManifestStore.Load` supports local manifest and backup fallback. | Add a visible **Open local collection** action using a native folder picker and clear local validation feedback. |
| Backup recovery | Contract tests already cover recovering a corrupted primary manifest from its local backup. | Exercise this recovery through the Desktop command and record acceptance evidence. |
| Invalid folder handling | The opener contract rejects a folder that is not a collection project. | Show that local validation state in the Workstation rather than leaving it service-only. |
| Panel registration | An explicit local manifest exists with exe/icon confinement. | Verify the Final installed payload’s `cyberbank-app.json`, executable, and icon together in CyberBank Panel. |
| Final packaging | Alpha installer and hash are documented. | Update assembly/Inno metadata to Final 1.0.0, run current full tests, publish, compile, install, launch, and record a new Final installer hash. |

## Work Paused State

Final work was paused at a controlled point. The `LocalCollectionProjectOpener` contract has tests for normal reopen, corrupt-primary-manifest fallback to local backup, and invalid-folder rejection. [3] The MainWindow still creates new projects and manages layers/assets, but it has no visible handler for opening an existing collection. [4]

This means the recovery foundation exists and has focused test coverage, but the user cannot yet reach it from the normal desktop workflow. No wallet, network, or commercial Web3 features were added while this work was paused.

## Decision

NFT Studio is a **credible Windows Alpha with strong local creative workflow coverage**. It is not ready for a truthful Final Edition label yet, but the remaining work is targeted rather than a rebuild: expose reopen/recovery in the desktop, prove Final Panel registration, then produce current Final test/build/install evidence.

## References

[1]: `Windows-Alpha-0.1.0.md` — current Alpha installer, launch, functional acceptance, scope, and limitations.

[2]: `../plans/2026-08-24-Web3-Collection-Studio-Final-Edition-Plan.md` — bounded Final delta and acceptance workflow.

[3]: `../../tests/Web3CollectionStudio.Desktop.Tests/LocalCollectionProjectOpenerContractTests.cs` — reopen, backup recovery, and invalid-folder contracts.

[4]: `../../src/Web3CollectionStudio.Desktop/Views/MainWindow.axaml.cs` — current New Collection, layer, asset, mode, and preflight handlers; no visible open-existing handler.
