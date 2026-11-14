# Flat Prismatic L Alpha Regression

**Status:** Open blocking visual regression as of 2026-08-25.

## Observed behavior

The installed LocalAppData Login now renders the Flat Prismatic L rather than the retired globe-L identity. However, the large product mark visibly includes a white and gray checkerboard square behind the symbol. This is unacceptable for the approved native Windows identity.

## Boundary evidence

| Boundary | Evidence | Result |
| --- | --- | --- |
| Published payload → installed payload | `brand-reinstall-payload-verification.txt` | `Web3CollectionStudio.Desktop.dll` SHA-256 matched after a process-clean Inno reinstall. |
| Installed runtime | `installed-window.png` | Login renders the Flat Prismatic L mark, but it includes the checkerboard/white-square background. |
| Master source asset | `/home/ubuntu/webdev-static-assets/nft-studio-flat-prismatic-l-clean-two.png` | The selected source visual itself visibly contains the same checkerboard field. |

## Root cause

The selected Flat Prismatic L master PNG was generated with the transparency checkerboard baked into the image pixels rather than with a true transparent alpha background. The Windows application correctly packages and renders that contaminated source asset. This is an **asset-alpha defect**, not a stale compiled-XAML, Inno replacement, or Windows icon-cache defect.

## Required resolution

Create a RED automated asset contract that rejects an opaque checkerboard corner/background; regenerate the runtime PNG and derived ICO from a true-alpha version of the approved L symbol; then rebuild, reinstall, and inspect installed Login, Splash, and Workstation captures before accepting the identity bundle.

## Source-level resolution evidence

The regression contract was expanded from outer-corner sampling to test four background-field positions and reject opaque, neutral light fringe pixels. It failed against the contaminated asset with background samples reporting alpha `255`, then passed after the alpha pipeline stripped the baked checkerboard before crop and again after the resize output was scrubbed for interpolation residue. The corrected runtime PNG has been visually inspected at source level and no longer contains the large white/checkerboard square. This is **not yet installed acceptance**; a fresh publish, installer, LocalAppData install, and runtime capture remain required.

## Installed verification update

The fresh installer was built, installed from LocalAppData, and its installed Desktop DLL matched the publish DLL hash. The retained `installed-login-flat-prismatic-l-alpha-fixed.png` capture shows the Flat Prismatic L on the native dark Login background with no checkerboard/white square. QA sign-in reached the Workstation title successfully. The subsequent Workstation screenshot fallback based on `PrintWindow` produced only title-bar geometry, so it is explicitly not accepted as visual content evidence; the Login runtime proof is the verified installed identity evidence for this regression.

## Follow-up authenticated capture

The original title-bar-only Workstation capture was traced to a minimized/off-screen window (`-32000,-32000`). After restoring the installed window to a visible rectangle, `PrintWindow` produced the full Workstation content. The retained `installed-workstation-flat-prismatic-l-alpha-fixed.png` shows the Flat Prismatic L in the installed workstation header without a white/checkerboard square.

The same `PrintWindow` route cannot capture Avalonia's Splash GPU client surface reliably: the timed installed Splash capture retained the native title bar and a black client area. That artifact is not accepted as visual proof of Splash content. The compiled source contract still verifies Splash references the exact runtime asset route; any Final decision must retain the distinction between that structural proof and a successful visual capture.
