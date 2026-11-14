namespace Web3CollectionStudio.Domain.Collections;

public static class ProjectRootPolicy
{
    public static bool Contains(string projectRoot, string candidatePath)
    {
        var root = Path.GetFullPath(projectRoot)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var candidate = Path.GetFullPath(candidatePath);
        var rootWithSeparator = string.Concat(root, Path.DirectorySeparatorChar);

        return string.Equals(root, candidate, StringComparison.OrdinalIgnoreCase)
            || candidate.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase);
    }
}
