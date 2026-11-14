# CyberBank Panel Host Launch Validation

## Result: PASS

The local CyberBank Panel host was launched from its existing `CyberBankPanel-win-x64` publish artifact. The NFT Studio application was registered from its installed `cyberbank-app.json` into the Panel registry using the manifest's declared, contained executable and icon paths. A pre-existing Panel registry file was copied to `registry.before-web3-panel-validation.json.bak` before this validation write.

| Validation step | Observed evidence | Result |
| --- | --- | --- |
| Panel host process | `CyberBankPanel.Desktop` opened with title `CyberBank Panel` and reported `Responding: true`. | PASS |
| Manifest-derived registration | `web3-collection-studio`, `CyberBank NFT Studio`, version `1.0.0`, installed EXE present, and installed ICO present were recorded. | PASS |
| Host registry projection | Panel UI displayed `CyberBank NFT Studio`, app ID `web3-collection-studio`, version `1.0.0`, and status `AVAILABLE`. | PASS |
| UI-mediated product launch | Panel's `Launch selected product` control was invoked; Panel emitted `LAUNCH DISPATCHED — CYBERBANK NFT STUDIO`, followed by a responsive installed product process. | PASS |

This test exercised local manifest registration and explicit user-interface launch only. The Panel did not open product project files, account data, credentials, or source assets, and the NFT Studio process retains its local-first boundary.

## References

[1]: ./panel-launch-smoke.json "Panel host smoke result"
[2]: ./panel-registration-result.json "Manifest-derived registration result"
[3]: ./panel-uia-inventory.json "Panel UI projection"
[4]: ./panel-web3-launch-result.json "UI-mediated product launch result"
