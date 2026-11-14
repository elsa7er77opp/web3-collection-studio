$installRoot = 'C:\Users\ahmed\AppData\Local\Programs\CyberBank\Web3 Collection Studio'
$projectRoot = 'C:\Users\ahmed\AppData\Local\CyberBank\Web3CollectionStudio\Collections\Final PFP QA-9cfd9fa9e3d84178be43a896ae4842c8'
$evidence = 'C:\Users\ahmed\Desktop\cyberbank\web3-collection-studio\docs\release\final-1.0.0\installed-recovery-library-probe.txt'
try {
    foreach ($name in @('Web3CollectionStudio.Domain.dll', 'Web3CollectionStudio.Application.dll', 'Web3CollectionStudio.Infrastructure.dll')) {
        [System.Reflection.Assembly]::LoadFrom((Join-Path $installRoot $name)) | Out-Null
    }
    $infrastructure = [System.Reflection.Assembly]::LoadFrom((Join-Path $installRoot 'Web3CollectionStudio.Infrastructure.dll'))
    $type = $infrastructure.GetType('Web3CollectionStudio.Infrastructure.ProjectStorage.LocalProjectManifestStore', $true)
    $store = [Activator]::CreateInstance($type)
    $result = $type.GetMethod('LoadWithOutcome').Invoke($store, @($projectRoot))
    $state = $result.GetType().GetProperty('State').GetValue($result)
    $project = $result.GetType().GetProperty('Project').GetValue($result)
    @(
        'Result: Success',
        "State: $state",
        "ProjectName: $($project.DisplayName)",
        "Profile: $($project.Profile)"
    ) | Set-Content -Encoding utf8 $evidence
}
catch {
    $exception = if ($_.Exception.InnerException) { $_.Exception.InnerException } else { $_.Exception }
    @(
        'Result: Failure',
        "ExceptionType: $($exception.GetType().FullName)",
        "Message: $($exception.Message)"
    ) | Set-Content -Encoding utf8 $evidence
}
