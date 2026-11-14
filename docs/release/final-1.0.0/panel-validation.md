# CyberBank Panel Validation

The installed `cyberbank-app.json`, executable, and icon were structurally validated as a confined local application registration bundle. The manifest reports version `1.0.0` and uses relative executable/icon paths inside the installed application root.[1]

A live CyberBank Panel host was then launched from its local publish artifact. It read the manifest-derived local registration and listed `CyberBank NFT Studio`, `web3-collection-studio`, `1.0.0`, and `AVAILABLE`. The Panel UI command **Launch selected product** emitted `LAUNCH DISPATCHED — CYBERBANK NFT STUDIO`, and a new responsive `Web3CollectionStudio.Desktop` process was observed afterward.[2] [3] [4]

## References

[1]: ./installed-acceptance-matrix.md "Installed Acceptance Matrix"
[2]: ./panel-registration-result.json "Panel registration result"
[3]: ./panel-uia-inventory.json "Panel UI Automation inventory"
[4]: ./panel-web3-launch-result.json "Panel UI-mediated product launch result"
