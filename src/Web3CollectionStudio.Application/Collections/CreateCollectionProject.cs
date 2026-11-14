using Web3CollectionStudio.Domain.Collections;

namespace Web3CollectionStudio.Application.Collections;

public sealed class CreateCollectionProject
{
    public CollectionProject Execute(string displayName, string localRoot, CollectionProfile profile)
    {
        return CollectionProject.Create(displayName, localRoot, profile);
    }
}
