$ErrorActionPreference = 'Stop'
$exe = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\src\Web3CollectionStudio.Desktop\bin\Debug\net10.0\Web3CollectionStudio.Desktop.exe'

Get-Process Web3CollectionStudio.Desktop -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Process -FilePath $exe
Start-Sleep -Seconds 3
Get-Process Web3CollectionStudio.Desktop | Select-Object Id, ProcessName, MainWindowTitle, Path | Format-List
