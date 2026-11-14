$ErrorActionPreference = 'Stop'
$iscc = 'C:\Users\ahmed\AppData\Local\Programs\Inno Setup 6\ISCC.exe'
$script = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\installer\Web3CollectionStudio.iss'

$process = Start-Process -FilePath $iscc -ArgumentList ('"' + $script + '"') -Wait -PassThru -NoNewWindow
if ($process.ExitCode -ne 0) { throw "Inno Setup failed with exit code $($process.ExitCode)." }
