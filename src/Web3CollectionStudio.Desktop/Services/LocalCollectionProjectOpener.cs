using System;
using System.IO;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Infrastructure.ProjectStorage;

namespace Web3CollectionStudio.Desktop.Services;

public sealed class LocalCollectionProjectOpener
{
    private readonly LocalProjectManifestStore manifestStore = new();

    public CollectionProject Open(string projectRoot)
    {
        return OpenWithOutcome(projectRoot).Project;
    }

    public ProjectManifestLoadResult OpenWithOutcome(string projectRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectRoot);

        var normalizedRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(projectRoot));
        if (!Directory.Exists(normalizedRoot))
        {
            throw new DirectoryNotFoundException("The selected local collection folder does not exist.");
        }

        var manifestPath = Path.Combine(normalizedRoot, ".collection-studio", "project.json");
        if (!File.Exists(manifestPath))
        {
            throw new InvalidDataException("The selected folder is not a local collection project.");
        }

        var result = manifestStore.LoadWithOutcome(normalizedRoot);
        var project = result.Project;
        var manifestRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(project.LocalRoot));
        if (!string.Equals(manifestRoot, normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("The local collection manifest points to a different project folder.");
        }

        if (result.State == ProjectManifestLoadState.RecoveredFromBackup)
        {
            new LocalProjectAuditStore().AppendBackupRecovery(normalizedRoot);
        }

        return result;
    }
}
