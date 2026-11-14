using System;
using System.IO;
using System.Linq;
using Web3CollectionStudio.Application.Collections;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Infrastructure.ProjectStorage;

namespace Web3CollectionStudio.Desktop.Services;

public sealed class LocalCollectionProjectCreator
{
    private readonly string collectionsDirectory;
    private readonly CreateAndSaveCollectionProject createAndSaveProject;

    public LocalCollectionProjectCreator(string stateDirectory)
    {
        collectionsDirectory = Path.Combine(stateDirectory, "Collections");
        createAndSaveProject = new CreateAndSaveCollectionProject(new LocalProjectManifestStore());
    }

    public CollectionProject CreatePfp(string displayName)
    {
        return Create(displayName, CollectionProfile.Pfp);
    }

    public CollectionProject Create(string displayName, CollectionProfile profile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        var folderName = $"{ToSafeFolderSegment(displayName)}-{Guid.NewGuid():N}";
        var projectRoot = Path.Combine(collectionsDirectory, folderName);
        return createAndSaveProject.Execute(displayName, projectRoot, profile);
    }

    private static string ToSafeFolderSegment(string displayName)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var sanitized = string.Concat(displayName
            .Trim()
            .Select(character => invalidCharacters.Contains(character) ? '-' : character));

        return string.IsNullOrWhiteSpace(sanitized) ? "collection" : sanitized;
    }
}
