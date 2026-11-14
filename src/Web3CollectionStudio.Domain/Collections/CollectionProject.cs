using Web3CollectionStudio.Domain.Layers;
using Web3CollectionStudio.Domain.Rules;

namespace Web3CollectionStudio.Domain.Collections;

public sealed record CollectionProject(
    Guid Id,
    string DisplayName,
    string LocalRoot,
    CollectionProfile Profile,
    CanvasSpec Canvas)
{
    private readonly List<CollectionLayer> layers = [];
    private readonly List<CompatibilityRule> compatibilityRules = [];

    public IReadOnlyList<CollectionLayer> Layers => layers.OrderBy(layer => layer.DrawOrder).ToArray();

    public IReadOnlyList<CompatibilityRule> CompatibilityRules => compatibilityRules.ToArray();

    public CollectionWorkspaceState WorkspaceState { get; private set; } = CollectionWorkspaceState.Default;

    public static CollectionProject Create(
        string displayName,
        string localRoot,
        CollectionProfile profile)
    {
        return Create(displayName, localRoot, profile, DefaultCanvasFor(profile));
    }

    public static CollectionProject Create(
        string displayName,
        string localRoot,
        CollectionProfile profile,
        CanvasSpec canvas)
    {
        if (canvas.Width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(canvas), "Canvas width must be greater than zero.");
        }

        if (canvas.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(canvas), "Canvas height must be greater than zero.");
        }

        return new CollectionProject(
            Guid.NewGuid(),
            displayName,
            localRoot,
            profile,
            canvas);
    }

    public CollectionLayer AddLayer(string name, int drawOrder)
    {
        if (layers.Any(layer => layer.DrawOrder == drawOrder))
        {
            throw new InvalidOperationException("A collection cannot contain more than one layer at the same draw order.");
        }

        var layer = CollectionLayer.Create(name, drawOrder);
        layers.Add(layer);
        return layer;
    }

    public void AddCompatibilityRule(CompatibilityRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        if (rule.FirstTraitId == rule.SecondTraitId)
        {
            throw new InvalidOperationException("A compatibility rule must reference two different local traits.");
        }

        var localTraitIds = layers.SelectMany(layer => layer.Traits).Select(trait => trait.Id).ToHashSet();
        if (!localTraitIds.Contains(rule.FirstTraitId) || !localTraitIds.Contains(rule.SecondTraitId))
        {
            throw new InvalidOperationException("Compatibility rules may reference only traits contained in this local collection.");
        }

        if (compatibilityRules.Contains(rule))
        {
            throw new InvalidOperationException("The local compatibility rule already exists.");
        }

        compatibilityRules.Add(rule);
    }

    public void UpdateWorkspaceState(CollectionWorkspaceState workspaceState)
    {
        ArgumentNullException.ThrowIfNull(workspaceState);
        if (workspaceState.WorkspaceMode is not ("Canvas" or "Gallery" or "Compare"))
        {
            throw new InvalidOperationException("The local workspace mode is not supported.");
        }

        if (workspaceState.SelectedLayerName is not null
            && !layers.Any(layer => string.Equals(layer.Name, workspaceState.SelectedLayerName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("The selected workspace layer does not belong to this local collection.");
        }

        var localTraitIds = layers.SelectMany(layer => layer.Traits).Select(trait => trait.Id).ToHashSet();
        if ((workspaceState.CompareLeftTraitId is { } left && !localTraitIds.Contains(left))
            || (workspaceState.CompareRightTraitId is { } right && !localTraitIds.Contains(right)))
        {
            throw new InvalidOperationException("The selected workspace trait does not belong to this local collection.");
        }

        WorkspaceState = workspaceState;
    }

    private static CanvasSpec DefaultCanvasFor(CollectionProfile profile) => profile switch
    {
        CollectionProfile.Pfp => new CanvasSpec(1024, 1024),
        CollectionProfile.BadgeGaming => new CanvasSpec(1024, 1024),
        CollectionProfile.DigitalArt => new CanvasSpec(2048, 2048),
        _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, "Unsupported collection profile.")
    };
}

public sealed record CollectionWorkspaceState(
    string WorkspaceMode,
    string? SelectedLayerName,
    Guid? CompareLeftTraitId,
    Guid? CompareRightTraitId)
{
    public static CollectionWorkspaceState Default { get; } = new("Canvas", null, null, null);
}
