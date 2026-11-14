# Web3 Collection Studio — Installed Acceptance Matrix

**Artifact:** `CyberBank-Web3-Collection-Studio-Final-Setup-win-x64.exe`  
**Version:** `1.0.0`  
**Installer SHA-256:** `67145663CCC7046A423E3F22D7BB65B7F1B2DE54B457B4BBDC105A370ABAF1A1`  
**Installer size:** `102,231,667` bytes

> All interactive evidence in this matrix was gathered from the installed executable under `%LocalAppData%\Programs\CyberBank\Web3 Collection Studio`, never from a source or publish executable.

| Final gate | Installed evidence | Result |
|---|---|---|
| Installer payload confinement | Installed EXE, `cyberbank-app.json`, and `.ico` are present under the per-user install root. Manifest version is `1.0.0`; executable/icon paths are relative and confined. | PASS |
| First run and local account | A non-production QA account was created through the native **Create Local Workspace** form. The account file was isolated non-destructively before each fresh run; collection projects and assets were never moved. | PASS |
| PFP workflow | A local PFP project was created through the Workstation, received a layer and local PNG trait through native dialogs, passed preflight, rendered a local PNG, and exported local metadata. | PASS |
| PFP metadata integrity | Installed metadata export referenced `outputs/preview-…png` by a relative path. No absolute project path was present. | PASS |
| Badge/Gaming profile | A local Badge/Gaming collection was created through the installed profile selector. | PASS |
| Digital Art profile | A local Digital Art collection was created through the installed profile selector and showed the expected 2048×2048 profile state. | PASS |
| Open Local Collection and recovery | Deliberately corrupting only the QA primary manifest while retaining its backup caused the installed Workstation to activate the project and show **Primary project manifest could not be read. Project restored from local backup.** | PASS |
| Recovery audit boundary | The local audit emitted only `project_recovered_from_backup` with a timestamp; it contains no root path, source content, or manifest body. | PASS |
| Local render cancellation | The final installed UI disables Cancel while idle. In a two-layer 4096×4096 local QA project, Cancel was invoked only when enabled and no new PNG was committed to `outputs`. | PASS |
| No external scope | Acceptance used local folders, native file/folder dialogs, local PNGs, local JSON, and local queues only. No wallet, private key, minting, blockchain call, IPFS/cloud upload, marketplace, payment, or trading capability was exercised or present. | PASS |
| Panel registration and launch | A live CyberBank Panel host was launched. It listed `CyberBank NFT Studio`, `web3-collection-studio`, version `1.0.0`, and `AVAILABLE` after reading the manifest-derived local registration. Its UI `Launch selected product` command then reported `LAUNCH DISPATCHED — CYBERBANK NFT STUDIO` and started the installed product process. | PASS |
| Visual identity — Login runtime | After a process-clean reinstall, the published and installed Desktop DLL SHA-256 values matched. The installed Login renders the owner-selected Flat Prismatic L with its dark native-app background visible through the asset; the previous baked checkerboard/white square is absent. | PASS |
| Visual identity — Workstation runtime | The full installed Workstation was captured after restoring the window from a minimized/off-screen state. It displays the Flat Prismatic L in its native header without a white/checkerboard square. | PASS |
| Visual identity — Splash runtime | The source contract proves Splash routes to the same alpha-corrected asset. The 1350ms GPU client surface could not be captured by the available PrintWindow path; the owner approved closure of the Final evidence gate with this limitation explicitly retained rather than misrepresenting the black GPU capture as proof. | OWNER-APPROVED WAIVER |

## Regression Evidence

The post-alpha-fix full regression run passed **76/76** tests: Domain 14, Application 9, Infrastructure 14, and Desktop 39. The two additional Desktop tests enforce that the Flat Prismatic L runtime PNG uses a true transparent background and contains no opaque neutral-light checkerboard fringe. Relevant Final regressions also include trait identity persistence, Require/Exclude persistence, workspace restoration, backup recovery outcomes, trailing-separator picker normalization, local recovery audit, visible open states, metadata/export flow, and queued/active cancellation availability.

## Evidence Index

- `installed-payload-verification.txt`
- `installed-launch-smoke.txt`
- `installed-pfp-0001.json`
- `installed-recovery-feedback.json`
- `installed-recovery-audit.jsonl`
- `installed-cancel-availability.json`
- `installed-cancellation-result.json`
- `installed-recovery-discrepancy-resolution.md`
- `installed-cancellation-resolution.md`
- `post-queued-cancel-fix-full-regression.log`
- `post-alpha-fix-full-regression.log`
- `flat-prismatic-l-alpha-red.log`
- `flat-prismatic-l-fringe-green.log`
- `flat-prismatic-l-alpha-regression.md`
- `brand-reinstall-payload-verification.txt`
- `installed-login-flat-prismatic-l-alpha-fixed.png`
- `installed-workstation-flat-prismatic-l-alpha-fixed.png`
- `panel-registration-result.json`
- `panel-uia-inventory.json`
- `panel-web3-launch-result.json`

## Current Release State

Functional Final gates are evidenced for the installed 1.0.0 artifact. The owner-rejected globe-L and the subsequent checkerboard/white-square asset regression are resolved in the installed Login and Workstation proofs. CyberBank Panel registration and UI-mediated launch are also evidenced. The transient GPU-only Splash capture limitation remains documented as an owner-approved waiver; it is not represented as a successful content screenshot.
