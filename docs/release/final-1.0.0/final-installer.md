# Final 1.0.0 Installer Record

The current candidate installer was rebuilt after the Flat Prismatic L alpha repair. It is a per-user Inno Setup installer targeting `%LocalAppData%\Programs\CyberBank\Web3 Collection Studio`.

| Property | Value |
| --- | --- |
| Filename | `CyberBank-Web3-Collection-Studio-Final-Setup-win-x64.exe` |
| Version | `1.0.0` |
| Size | `102,231,667` bytes |
| SHA-256 | `67145663CCC7046A423E3F22D7BB65B7F1B2DE54B457B4BBDC105A370ABAF1A1` |
| Publish/installed Desktop DLL match | `True` |

The installer was run only after the application process was stopped. The installed `Web3CollectionStudio.Desktop.dll` hash exactly matched the DLL in the current publish output; this avoids attributing runtime behavior to a stale binary.[1] [2]

## References

[1]: ./installer-sha256.txt "Installer SHA-256"
[2]: ./brand-reinstall-payload-verification.txt "Published and installed DLL comparison"
