using Web3CollectionStudio.Desktop.Services;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Rules;
using Web3CollectionStudio.Infrastructure.ProjectStorage;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class LocalProjectEditorContractTests : IDisposable
{
    private readonly string projectRoot = Path.Combine(Path.GetTempPath(), $"collection-studio-editor-{Guid.NewGuid():N}");
    private readonly string sourceRoot = Path.Combine(Path.GetTempPath(), $"collection-studio-source-{Guid.NewGuid():N}");

    [Fact]
    public void Editor_adds_a_local_layer_imports_a_png_trait_inside_the_project_and_persists_it()
    {
        Directory.CreateDirectory(projectRoot);
        Directory.CreateDirectory(sourceRoot);
        var project = CollectionProject.Create("Signal Archive", projectRoot, CollectionProfile.Pfp);
        var sourcePng = Path.Combine(sourceRoot, "violet-grid.png");
        File.WriteAllBytes(sourcePng, [137, 80, 78, 71, 13, 10, 26, 10]);
        var editor = new LocalProjectEditor();

        var layer = editor.AddLayer(project, "Background");
        var trait = editor.ImportPngTrait(project, layer.Name, sourcePng, "Violet Grid", weight: 80);
        var persisted = new LocalProjectManifestStore().Load(projectRoot);

        Assert.Equal(0, layer.DrawOrder);
        Assert.True(ProjectRootPolicy.Contains(projectRoot, Path.Combine(projectRoot, trait.RelativeAssetPath)));
        Assert.True(File.Exists(Path.Combine(projectRoot, trait.RelativeAssetPath)));
        var persistedLayer = Assert.Single(persisted.Layers);
        Assert.Equal("Background", persistedLayer.Name);
        Assert.Equal("Violet Grid", Assert.Single(persistedLayer.Traits).DisplayName);
    }

    [Fact]
    public void Editor_updates_a_local_trait_weight_and_persists_the_same_trait_identity()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Signal Archive", projectRoot, CollectionProfile.Pfp);
        var layer = project.AddLayer("Background", drawOrder: 0);
        var trait = layer.AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var editor = new LocalProjectEditor();

        var updated = editor.UpdateTraitWeight(project, layer.Name, trait.Id, weight: 42);
        var persisted = new LocalProjectManifestStore().Load(projectRoot);
        var persistedTrait = Assert.Single(Assert.Single(persisted.Layers).Traits);

        Assert.Equal(trait.Id, updated.Id);
        Assert.Equal(42, updated.Weight);
        Assert.Equal(trait.Id, persistedTrait.Id);
        Assert.Equal(42, persistedTrait.Weight);
    }

    [Fact]
    public void Editor_adds_and_persists_a_local_compatibility_rule_between_contained_traits()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Signal Archive", projectRoot, CollectionProfile.Pfp);
        var first = project.AddLayer("Background", drawOrder: 0)
            .AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var second = project.AddLayer("Foreground", drawOrder: 1)
            .AddTrait("Moon Mask", "assets/foreground/moon-mask.png", weight: 20);
        var editor = new LocalProjectEditor();

        editor.AddCompatibilityRule(project, CompatibilityRule.Require(second.Id, first.Id));
        var persisted = new LocalProjectManifestStore().Load(projectRoot);

        Assert.Equal(
            CompatibilityRule.Require(second.Id, first.Id),
            Assert.Single(persisted.CompatibilityRules));
    }

    [Fact]
    public void Editor_persists_the_local_workspace_mode_layer_and_compare_selection()
    {
        Directory.CreateDirectory(projectRoot);
        var project = CollectionProject.Create("Signal Archive", projectRoot, CollectionProfile.Pfp);
        var background = project.AddLayer("Background", drawOrder: 0)
            .AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var foreground = project.AddLayer("Foreground", drawOrder: 1)
            .AddTrait("Moon Mask", "assets/foreground/moon-mask.png", weight: 20);
        var editor = new LocalProjectEditor();
        var state = new CollectionWorkspaceState("Compare", "Foreground", foreground.Id, background.Id);

        editor.UpdateWorkspaceState(project, state);
        var persisted = new LocalProjectManifestStore().Load(projectRoot);

        Assert.Equal(state, persisted.WorkspaceState);
    }

    public void Dispose()
    {
        if (Directory.Exists(projectRoot))
        {
            Directory.Delete(projectRoot, recursive: true);
        }

        if (Directory.Exists(sourceRoot))
        {
            Directory.Delete(sourceRoot, recursive: true);
        }
    }
}
