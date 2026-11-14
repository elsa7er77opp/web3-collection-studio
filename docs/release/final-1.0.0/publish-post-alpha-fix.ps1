$ErrorActionPreference = 'Stop'
$env:ProgramFiles = 'C:\Program Files'
${env:ProgramFiles(x86)} = 'C:\Program Files (x86)'
$env:ProgramW6432 = 'C:\Program Files'
$env:CommonProgramFiles = 'C:\Program Files\Common Files'
${env:CommonProgramFiles(x86)} = 'C:\Program Files (x86)\Common Files'
$env:MSBUILDDISABLENODEREUSE = '1'

$project = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\src\Web3CollectionStudio.Desktop\Web3CollectionStudio.Desktop.csproj'
$output = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\artifacts\Web3CollectionStudio-Final-win-x64'
$log = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\post-alpha-fix-publish.log'

& dotnet publish $project -c Release -r win-x64 --self-contained true --no-restore -o $output *>&1 | Set-Content -Encoding utf8 $log
exit $LASTEXITCODE
