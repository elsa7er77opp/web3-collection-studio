using Web3CollectionStudio.Desktop.Services;
using Web3CollectionStudio.Domain.Collections;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class LocalCollectionProjectCreatorContractTests : IDisposable
{
    private readonly string stateDirectory = Path.Combine(Path.GetTempPath(), $"collection-workspace-{Guid.NewGuid():N}");

    [Fact]
    public void CreatePfp_creates_and_saves_a_project_under_the_local_workspace()
    {
        var creator = new LocalCollectionProjectCreator(stateDirectory);

        var project = creator.CreatePfp("Midnight Masks");

        Assert.StartsWith(Path.Combine(stateDirectory, "Collections"), project.LocalRoot, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1024, project.Canvas.Width);
        Assert.True(File.Exists(Path.Combine(project.LocalRoot, ".collection-studio", "project.json")));
    }

    [Fact]
    public void Create_uses_the_selected_local_collection_profile()
    {
        var creator = new LocalCollectionProjectCreator(stateDirectory);

        var project = creator.Create("Arcade Badge Set", CollectionProfile.BadgeGaming);

        Assert.Equal(CollectionProfile.BadgeGaming, project.Profile);
        Assert.Equal(1024, project.Canvas.Width);
        Assert.True(File.Exists(Path.Combine(project.LocalRoot, ".collection-studio", "project.json")));
    }

    public void Dispose()
    {
        if (Directory.Exists(stateDirectory))
        {
            Directory.Delete(stateDirectory, recursive: true);
        }
    }
}
