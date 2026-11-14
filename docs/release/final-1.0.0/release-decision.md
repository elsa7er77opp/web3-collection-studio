# Web3 Collection Studio 1.0.0 Release Decision

## Decision: VERIFIED FINAL 1.0.0

The current `1.0.0` installer is approved as the verified **Final** release. It has a fresh self-contained `win-x64` build, SHA-256 fingerprint, publish-to-installed DLL hash match, 76/76 post-fix regression, installed local workflow evidence, and visible Flat Prismatic L Login and Workstation evidence without the former white/checkerboard-square regression.[1] [2] [3] [4]

CyberBank Panel host integration is also verified. The live local host listed the manifest-derived `web3-collection-studio` record at version `1.0.0` as available, then its UI dispatched the installed application successfully.[5]

The only retained evidence qualification is narrow and explicit: the `1350ms` Avalonia Splash GPU client surface could not be captured by the available PrintWindow route. The asset route is protected by the compiled source contract, which asserts that Splash, Login, and MainWindow all use the same alpha-corrected runtime mark. The owner requested closure of the remaining gates and approved this documented evidence waiver; no document claims the black GPU capture as a valid content screenshot.[6] [7]

## Final scope statement

The Final release remains a native, local-first Windows creative workstation. It does not add wallet, seed phrase, private key, minting, smart contracts, blockchain publishing, IPFS/cloud upload, marketplace, payment, trading, or investment behavior.

## References

[1]: ./final-tests.md "Final Test Record"
[2]: ./final-installer.md "Final Installer Record"
[3]: ./installed-login-flat-prismatic-l-alpha-fixed.png "Installed Login identity capture"
[4]: ./installed-workstation-flat-prismatic-l-alpha-fixed.png "Installed Workstation identity capture"
[5]: ./panel-host-launch-validation.md "Panel Host Launch Validation"
[6]: ./known-limitations.md "Known Limitations and Evidence Qualifications"
[7]: ./flat-prismatic-l-alpha-regression.md "Flat Prismatic L Alpha Regression"
