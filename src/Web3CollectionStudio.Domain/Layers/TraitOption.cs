namespace Web3CollectionStudio.Domain.Layers;

public sealed record TraitOption
{
    public TraitOption(Guid id, string displayName, string relativeAssetPath, int weight)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(relativeAssetPath);
        if (Path.IsPathRooted(relativeAssetPath) || !relativeAssetPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Trait assets must use a relative local PNG path.", nameof(relativeAssetPath));
        }

        if (weight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), "Trait weight cannot be negative.");
        }

        Id = id;
        DisplayName = displayName;
        RelativeAssetPath = relativeAssetPath;
        Weight = weight;
    }

    public Guid Id { get; }

    public string DisplayName { get; }

    public string RelativeAssetPath { get; }

    public int Weight { get; }
}
