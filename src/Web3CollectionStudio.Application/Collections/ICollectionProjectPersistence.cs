using Web3CollectionStudio.Domain.Collections;

namespace Web3CollectionStudio.Application.Collections;

public interface ICollectionProjectPersistence
{
    void Save(CollectionProject project);
}
