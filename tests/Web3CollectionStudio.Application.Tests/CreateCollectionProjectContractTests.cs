using Web3CollectionStudio.Application.Collections;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Rules;

namespace Web3CollectionStudio.Application.Tests;

public sealed class CreateCollectionProjectContractTests
{
    [Fact]
    public void Execute_creates_a_local_pfp_project_with_the_profile_default_canvas()
    {
        var useCase = new CreateCollectionProject();

        var project = useCase.Execute(
            "Midnight Masks",
            Path.Combine(Path.GetTempPath(), $"collection-{Guid.NewGuid():N}"),
            CollectionProfile.Pfp);

        Assert.Equal("Midnight Masks", project.DisplayName);
        Assert.Equal(CollectionProfile.Pfp, project.Profile);
        Assert.Equal(1024, project.Canvas.Width);
        Assert.Equal(1024, project.Canvas.Height);
    }

    [Fact]
    public void CreateAndSave_persists_the_new_local_project_once()
    {
        var persistence = new RecordingProjectPersistence();
        var useCase = new CreateAndSaveCollectionProject(persistence);

        var project = useCase.Execute(
            "Midnight Masks",
            Path.Combine(Path.GetTempPath(), $"collection-{Guid.NewGuid():N}"),
            CollectionProfile.Pfp);

        Assert.Same(project, persistence.SavedProject);
    }

    [Fact]
    public void Preflight_blocks_a_project_without_layers()
    {
        var project = CollectionProject.Create(
            "Midnight Masks",
            Path.Combine(Path.GetTempPath(), $"collection-{Guid.NewGuid():N}"),
            CollectionProfile.Pfp);
        var preflight = new CollectionPreflightService();

        var report = preflight.Evaluate(project, []);

        Assert.False(report.IsReady);
        Assert.Contains(report.Issues, issue => issue.Code == "missing_layers");
    }

    [Fact]
    public void Preflight_blocks_a_trait_with_a_missing_local_png_asset()
    {
        var root = Path.Combine(Path.GetTempPath(), $"collection-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            var project = CollectionProject.Create("Midnight Masks", root, CollectionProfile.Pfp);
            project.AddLayer("Background", 0).AddTrait("Indigo", "assets/background/indigo.png", 100);

            var report = new CollectionPreflightService().Evaluate(project, []);

            Assert.False(report.IsReady);
            Assert.Contains(report.Issues, issue => issue.Code == "missing_asset");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Preflight_blocks_a_rule_that_references_a_missing_trait()
    {
        var root = Path.Combine(Path.GetTempPath(), $"collection-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(root, "assets", "background"));
        File.WriteAllBytes(Path.Combine(root, "assets", "background", "indigo.png"), []);
        try
        {
            var project = CollectionProject.Create("Midnight Masks", root, CollectionProfile.Pfp);
            var trait = project.AddLayer("Background", 0).AddTrait("Indigo", "assets/background/indigo.png", 100);
            var invalidRule = CompatibilityRule.Require(trait.Id, Guid.NewGuid());

            var report = new CollectionPreflightService().Evaluate(project, [invalidRule]);

            Assert.False(report.IsReady);
            Assert.Contains(report.Issues, issue => issue.Code == "invalid_rule_reference");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Preflight_blocks_a_requested_output_count_above_unique_trait_combinations()
    {
        var root = Path.Combine(Path.GetTempPath(), $"collection-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(root, "assets", "background"));
        File.WriteAllBytes(Path.Combine(root, "assets", "background", "indigo.png"), []);
        try
        {
            var project = CollectionProject.Create("Midnight Masks", root, CollectionProfile.Pfp);
            project.AddLayer("Background", 0).AddTrait("Indigo", "assets/background/indigo.png", 100);

            var report = new CollectionPreflightService().Evaluate(project, [], requestedOutputCount: 2);

            Assert.False(report.IsReady);
            Assert.Contains(report.Issues, issue => issue.Code == "requested_output_exceeds_unique_combinations");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class RecordingProjectPersistence : ICollectionProjectPersistence
    {
        public CollectionProject? SavedProject { get; private set; }

        public void Save(CollectionProject project)
        {
            SavedProject = project;
        }
    }
}
