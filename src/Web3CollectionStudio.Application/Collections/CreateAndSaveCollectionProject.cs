using Web3CollectionStudio.Domain.Collections;

namespace Web3CollectionStudio.Application.Collections;

public sealed class CreateAndSaveCollectionProject(ICollectionProjectPersistence persistence)
{
    private readonly CreateCollectionProject createCollectionProject = new();

    public CollectionProject Execute(string displayName, string localRoot, CollectionProfile profile)
    {
        var project = createCollectionProject.Execute(displayName, localRoot, profile);
        persistence.Save(project);
        return project;
    }
}
