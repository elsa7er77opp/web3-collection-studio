namespace Web3CollectionStudio.Domain.Layers;

public sealed class CollectionLayer
{
    private readonly List<TraitOption> traits = [];

    private CollectionLayer(string name, int drawOrder)
    {
        Name = name;
        DrawOrder = drawOrder;
    }

    public string Name { get; }

    public int DrawOrder { get; }

    public IReadOnlyList<TraitOption> Traits => traits;

    public static CollectionLayer Create(string name, int drawOrder)
    {
        return new CollectionLayer(name, drawOrder);
    }

    public TraitOption AddTrait(string displayName, string relativeAssetPath, int weight)
    {
        return AddTrait(Guid.NewGuid(), displayName, relativeAssetPath, weight);
    }

    public TraitOption AddTrait(Guid id, string displayName, string relativeAssetPath, int weight)
    {
        if (traits.Any(trait => string.Equals(trait.RelativeAssetPath, relativeAssetPath, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A layer cannot contain the same local asset path more than once.");
        }

        var trait = new TraitOption(id, displayName, relativeAssetPath, weight);
        traits.Add(trait);
        return trait;
    }

    public TraitOption UpdateTraitWeight(Guid traitId, int weight)
    {
        var index = traits.FindIndex(trait => trait.Id == traitId);
        if (index < 0)
        {
            throw new InvalidOperationException("The selected trait does not belong to this local layer.");
        }

        var existing = traits[index];
        var updated = new TraitOption(existing.Id, existing.DisplayName, existing.RelativeAssetPath, weight);
        traits[index] = updated;
        return updated;
    }
}
