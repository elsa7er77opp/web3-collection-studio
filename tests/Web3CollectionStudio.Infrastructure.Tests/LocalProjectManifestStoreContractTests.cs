using Web3CollectionStudio.Application.Collections;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Rules;
using Web3CollectionStudio.Infrastructure.ProjectStorage;

namespace Web3CollectionStudio.Infrastructure.Tests;

public sealed class LocalProjectManifestStoreContractTests : IDisposable
{
    private readonly string projectRoot = Path.Combine(Path.GetTempPath(), $"collection-project-{Guid.NewGuid():N}");

    [Fact]
    public void Save_writes_a_versioned_manifest_under_the_authorized_local_root()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Neon Creatures", projectRoot, CollectionProfile.Pfp);
        var store = new LocalProjectManifestStore();

        store.Save(project);
        var loaded = store.Load(projectRoot);

        Assert.True(File.Exists(Path.Combine(projectRoot, ".collection-studio", "project.json")));
        Assert.Equal(project.Id, loaded.Id);
        Assert.Equal("Neon Creatures", loaded.DisplayName);
        Assert.Equal(CollectionProfile.Pfp, loaded.Profile);
    }

    [Fact]
    public void Store_implements_the_application_project_persistence_contract()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Neon Creatures", projectRoot, CollectionProfile.Pfp);
        ICollectionProjectPersistence store = new LocalProjectManifestStore();

        store.Save(project);

        Assert.True(File.Exists(Path.Combine(projectRoot, ".collection-studio", "project.json")));
    }

    [Fact]
    public void Load_with_outcome_recovers_from_backup_without_overwriting_the_damaged_primary_manifest()
    {
        Directory.CreateDirectory(projectRoot);
        var originalProject = CollectionProject.Create("Neon Creatures", projectRoot, CollectionProfile.Pfp);
        var revisedProject = new CollectionProject(
            originalProject.Id,
            "Neon Creatures Revised",
            projectRoot,
            CollectionProfile.Pfp,
            originalProject.Canvas);
        var store = new LocalProjectManifestStore();

        store.Save(originalProject);
        store.Save(revisedProject);

        var manifestPath = Path.Combine(projectRoot, ".collection-studio", "project.json");
        File.WriteAllText(manifestPath, "{ damaged manifest");

        var recovered = store.LoadWithOutcome(projectRoot);

        Assert.Equal(ProjectManifestLoadState.RecoveredFromBackup, recovered.State);
        Assert.Equal("Neon Creatures", recovered.Project.DisplayName);
        Assert.Equal("{ damaged manifest", File.ReadAllText(manifestPath));
        Assert.True(File.Exists(Path.Combine(projectRoot, ".collection-studio", "project.json.bak")));
    }

    [Fact]
    public void Save_and_load_preserve_the_local_layer_and_trait_inventory()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Neon Creatures", projectRoot, CollectionProfile.Pfp);
        var layer = project.AddLayer("Background", drawOrder: 0);
        layer.AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var store = new LocalProjectManifestStore();

        store.Save(project);
        var loaded = store.Load(projectRoot);

        var loadedLayer = Assert.Single(loaded.Layers);
        Assert.Equal("Background", loadedLayer.Name);
        Assert.Equal(0, loadedLayer.DrawOrder);
        var loadedTrait = Assert.Single(loadedLayer.Traits);
        Assert.Equal("Violet Grid", loadedTrait.DisplayName);
        Assert.Equal("assets/background/violet-grid.png", loadedTrait.RelativeAssetPath);
        Assert.Equal(80, loadedTrait.Weight);
    }

    [Fact]
    public void Save_and_load_preserve_the_stable_trait_identity_needed_for_local_rules()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Neon Creatures", projectRoot, CollectionProfile.Pfp);
        var trait = project.AddLayer("Background", drawOrder: 0)
            .AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var store = new LocalProjectManifestStore();

        store.Save(project);
        var loadedTrait = Assert.Single(Assert.Single(store.Load(projectRoot).Layers).Traits);

        Assert.Equal(trait.Id, loadedTrait.Id);
    }

    [Fact]
    public void Save_and_load_preserve_local_require_and_exclude_rules_between_stable_traits()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Neon Creatures", projectRoot, CollectionProfile.Pfp);
        var background = project.AddLayer("Background", drawOrder: 0)
            .AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var foreground = project.AddLayer("Foreground", drawOrder: 1)
            .AddTrait("Moon Mask", "assets/foreground/moon-mask.png", weight: 20);
        project.AddCompatibilityRule(CompatibilityRule.Require(foreground.Id, background.Id));
        project.AddCompatibilityRule(CompatibilityRule.Exclude(background.Id, foreground.Id));
        var store = new LocalProjectManifestStore();

        store.Save(project);
        var loaded = store.Load(projectRoot);

        Assert.Equal(project.CompatibilityRules, loaded.CompatibilityRules);
    }

    [Fact]
    public void Save_and_load_preserve_the_local_workspace_selection_state()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Neon Creatures", projectRoot, CollectionProfile.Pfp);
        var background = project.AddLayer("Background", drawOrder: 0)
            .AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var foreground = project.AddLayer("Foreground", drawOrder: 1)
            .AddTrait("Moon Mask", "assets/foreground/moon-mask.png", weight: 20);
        project.UpdateWorkspaceState(new CollectionWorkspaceState(
            WorkspaceMode: "Compare",
            SelectedLayerName: "Foreground",
            CompareLeftTraitId: foreground.Id,
            CompareRightTraitId: background.Id));
        var store = new LocalProjectManifestStore();

        store.Save(project);
        var loaded = store.Load(projectRoot);

        Assert.Equal(project.WorkspaceState, loaded.WorkspaceState);
    }

    [Fact]
    public void Load_with_outcome_recovers_a_rich_backup_with_trait_identity_and_workspace_state()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Recovery QA", projectRoot, CollectionProfile.Pfp);
        var trait = project.AddLayer("Background", drawOrder: 0)
            .AddTrait("Acceptance Indigo", "assets/background/acceptance-indigo.png", weight: 100);
        project.UpdateWorkspaceState(new CollectionWorkspaceState("Canvas", null, null, null));
        var store = new LocalProjectManifestStore();

        store.Save(project);
        store.Save(project);
        File.WriteAllText(Path.Combine(projectRoot, ".collection-studio", "project.json"), "{ damaged manifest");

        var recovered = store.LoadWithOutcome(projectRoot);

        Assert.Equal(ProjectManifestLoadState.RecoveredFromBackup, recovered.State);
        var recoveredTrait = Assert.Single(Assert.Single(recovered.Project.Layers).Traits);
        Assert.Equal(trait.Id, recoveredTrait.Id);
        Assert.Equal(new CollectionWorkspaceState("Canvas", null, null, null), recovered.Project.WorkspaceState);
    }

    public void Dispose()
    {
        if (Directory.Exists(projectRoot))
        {
            Directory.Delete(projectRoot, recursive: true);
        }
    }
}
