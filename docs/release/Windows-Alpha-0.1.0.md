# CyberBank Web3 Collection Studio — Windows Alpha 0.1.0

**Release date:** 2026-08-23  
**Runtime:** Windows x64, self-contained .NET 10 publish  
**Installation model:** Per-user Inno Setup installer; no administrator privilege required.

## Distribution artifacts

| Artifact | Path | SHA-256 |
|---|---|---|
| Windows installer | `artifacts/installer/CyberBank-Web3-Collection-Studio-Alpha-Setup-win-x64.exe` | `9e576de154feb9ef063b0c8b186193a47c6f90a8bb7ca12bf86ef1c1720eac7c` |
| Portable ZIP | `artifacts/CyberBank-Web3-Collection-Studio-Alpha-win-x64.zip` | `2dab69379e211cd41f1cc77e3f9d8cf453a0c737fca6c906ef36376e1214164b` |
| Self-contained publish root | `artifacts/Web3CollectionStudio-Alpha-win-x64/` | Not distributed separately from the ZIP. |

The publish payload includes the dedicated `collection-studio-app-icon.ico` and an explicit `cyberbank-app.json`. The manifest is the sole supported registration input for CyberBank Panel; it exposes product identity, version, executable path, icon path, and public local capabilities only.

## Verification evidence

| Check | Result |
|---|---|
| Full `Web3CollectionStudio.slnx` test suite | Passed after the startup repair. |
| Self-contained Windows x64 publish | Passed. |
| Installer compiler | Inno Setup 7.1.0 completed successfully. |
| Silent clean install | Passed into a fresh user-local smoke-test directory. |
| Installed payload validation | `Web3CollectionStudio.Desktop.exe` and `cyberbank-app.json` were both present. |
| One-time launch smoke test | The repaired installer was installed into a new local directory; `Web3 Collection Studio — Local Sign In` became a responsive Windows window with no new Application exception. |
| Functional acceptance | Passed in an isolated local profile: account creation/sign-in, PFP, Badge/Gaming, and Digital Art project creation, manifest persistence, Workstation profile/canvas state, blocked empty-project preflight, and idle bounded queue status. |
| Improved installer smoke test | Passed: the updated per-user installer replaced the standard installation and opened `Web3 Collection Studio — Local Sign In` successfully. |
| Interactive collection build | Passed: layers and imported PNG traits persist in manifest v2; Gallery and Compare use local traits only; the installer EXE hash was verified against the final publish payload after installation. |
| NFT Studio identity bundle | Passed: the NFT Studio globe/monogram identity, EXE ICO, local manifest name, Splash, Login, and Workstation branding were rebuilt and installed. `CyberBank NFT Studio — Local Sign In` opened successfully, with the manifest and ICO verified in the installed payload. |
| Standalone symbol correction | Passed: the icon was rebuilt as a dedicated true-alpha near-circular `I`/hex/micro-globe emblem. The Splash retains the larger master globe visual, while Login, Workstation chrome, and the EXE use the standalone symbol. The rebuilt installer opened `CyberBank NFT Studio — Local Sign In` successfully. |

## Startup repair

The first Alpha payload referenced `avares://Web3CollectionStudio.Desktop/Assets/Fonts#Inter`, while that font asset was not included in the publish output. Avalonia therefore terminated when rendering the login window. The invalid font resource has been removed, a regression contract now prevents the reference from returning, and the installer/ZIP were rebuilt from the corrected publish output.

## Included Alpha capabilities

The product remains a **local-first NFT collection and creative asset workstation**. This build includes local account setup/sign-in, PFP, Badge/Gaming, and Digital Art project profiles, interactive local layers, local PNG trait import, Gallery and Compare modes, layer and trait validation, Require/Exclude rules, duplicate fingerprints, manifest backup/recovery, preflight validation, bounded local PNG composition, progress/cancellation states, safe local audit events, and OpenSea-style local metadata writing with relative image paths.

## Explicit exclusions

This Alpha does **not** contain a wallet, seed phrase/private-key handling, blockchain minting, IPFS/cloud asset upload, marketplace publication, payments, trading features, investment claims, or financial advice. User assets and project data remain local.

## Alpha limitations

The installer and executable are **not code-signed**. Windows may therefore display reputation or publisher warnings. The four-column Collection Workstation projects real local profile, canvas, layers, imported traits, Gallery/Compare selections, preflight, and bounded queue state; continued hardening will focus on deeper trait editing, additional recovery states, and final Panel registration of the stable installed application manifest.

## Safe Panel registration

After installation, open **CyberBank Panel**, choose **Register local app**, and select the installed Collection Studio folder. Panel validates the adjacent `cyberbank-app.json` and confines the executable and icon paths to that folder before listing or enabling launch. It does not inspect Collection Studio project data, local account records, asset files, or private databases.
