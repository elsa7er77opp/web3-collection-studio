using Web3CollectionStudio.Desktop.Services;
using Web3CollectionStudio.Infrastructure.ProjectStorage;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class LocalCollectionProjectOpenerContractTests : IDisposable
{
    private readonly string stateDirectory = Path.Combine(Path.GetTempPath(), $"collection-opener-{Guid.NewGuid():N}");

    [Fact]
    public void Open_restores_a_saved_local_collection_into_the_workstation_contract()
    {
        var creator = new LocalCollectionProjectCreator(stateDirectory);
        var created = creator.CreatePfp("Final reopen test");

        var reopened = new LocalCollectionProjectOpener().Open(created.LocalRoot);

        Assert.Equal(created.Id, reopened.Id);
        Assert.Equal("Final reopen test", reopened.DisplayName);
        Assert.Equal(created.Profile, reopened.Profile);
    }

    [Fact]
    public void Open_recovers_from_a_corrupt_primary_manifest_using_the_local_backup()
    {
        var creator = new LocalCollectionProjectCreator(stateDirectory);
        var created = creator.CreatePfp("Backup reopen test");
        var store = new LocalProjectManifestStore();
        store.Save(created);
        File.WriteAllText(Path.Combine(created.LocalRoot, ".collection-studio", "project.json"), "not-json");

        var reopened = new LocalCollectionProjectOpener().Open(created.LocalRoot);

        Assert.Equal(created.Id, reopened.Id);
        Assert.Equal("Backup reopen test", reopened.DisplayName);
    }

    [Fact]
    public void Open_with_outcome_exposes_backup_recovery_to_the_desktop_workflow()
    {
        var creator = new LocalCollectionProjectCreator(stateDirectory);
        var created = creator.CreatePfp("Outcome reopen test");
        var store = new LocalProjectManifestStore();
        store.Save(created);
        File.WriteAllText(Path.Combine(created.LocalRoot, ".collection-studio", "project.json"), "not-json");

        var reopened = new LocalCollectionProjectOpener().OpenWithOutcome(created.LocalRoot);

        Assert.Equal(ProjectManifestLoadState.RecoveredFromBackup, reopened.State);
        Assert.Equal(created.Id, reopened.Project.Id);
    }

    [Fact]
    public void Open_with_outcome_recovers_backup_when_native_folder_selection_has_a_trailing_separator()
    {
        var creator = new LocalCollectionProjectCreator(stateDirectory);
        var created = creator.CreatePfp("Trailing separator recovery test");
        var store = new LocalProjectManifestStore();
        store.Save(created);
        File.WriteAllText(Path.Combine(created.LocalRoot, ".collection-studio", "project.json"), "not-json");

        var reopened = new LocalCollectionProjectOpener().OpenWithOutcome(created.LocalRoot + Path.DirectorySeparatorChar);

        Assert.Equal(ProjectManifestLoadState.RecoveredFromBackup, reopened.State);
        Assert.Equal(created.LocalRoot, reopened.Project.LocalRoot);
    }

    [Fact]
    public void Open_with_outcome_records_a_safe_local_backup_recovery_audit_event()
    {
        var creator = new LocalCollectionProjectCreator(stateDirectory);
        var created = creator.CreatePfp("Audit reopen test");
        var store = new LocalProjectManifestStore();
        store.Save(created);
        File.WriteAllText(Path.Combine(created.LocalRoot, ".collection-studio", "project.json"), "not-json");

        new LocalCollectionProjectOpener().OpenWithOutcome(created.LocalRoot);

        var auditPath = Path.Combine(created.LocalRoot, ".collection-studio", "project-events.jsonl");
        var entry = File.ReadAllText(auditPath);
        Assert.Contains("project_recovered_from_backup", entry);
        Assert.DoesNotContain(created.LocalRoot, entry, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("not-json", entry, StringComparison.Ordinal);
    }

    [Fact]
    public void Open_rejects_a_local_folder_that_is_not_a_collection_project()
    {
        Directory.CreateDirectory(stateDirectory);

        Assert.Throws<InvalidDataException>(() => new LocalCollectionProjectOpener().Open(stateDirectory));
    }

    public void Dispose()
    {
        if (Directory.Exists(stateDirectory))
        {
            Directory.Delete(stateDirectory, recursive: true);
        }
    }
}
