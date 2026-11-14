using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Rules;

namespace Web3CollectionStudio.Application.Collections;

public sealed class CollectionPreflightService
{
    public CollectionPreflightReport Evaluate(
        CollectionProject project,
        IReadOnlyList<CompatibilityRule> rules,
        int requestedOutputCount = 0)
    {
        var issues = new List<CollectionPreflightIssue>();
        if (project.Layers.Count == 0)
        {
            issues.Add(new CollectionPreflightIssue(
                "missing_layers",
                "Add at least one local layer before rendering a collection."));
        }

        foreach (var trait in project.Layers.SelectMany(layer => layer.Traits))
        {
            var assetPath = Path.Combine(project.LocalRoot, trait.RelativeAssetPath);
            if (!File.Exists(assetPath))
            {
                issues.Add(new CollectionPreflightIssue(
                    "missing_asset",
                    $"The local PNG asset for trait '{trait.DisplayName}' is missing."));
            }
        }

        var knownTraitIds = project.Layers
            .SelectMany(layer => layer.Traits)
            .Select(trait => trait.Id)
            .ToHashSet();
        foreach (var rule in rules)
        {
            if (!knownTraitIds.Contains(rule.FirstTraitId) || !knownTraitIds.Contains(rule.SecondTraitId))
            {
                issues.Add(new CollectionPreflightIssue(
                    "invalid_rule_reference",
                "A compatibility rule references a trait that is not in this collection."));
            }
        }

        if (requestedOutputCount > 0)
        {
            var uniqueCombinationCapacity = UniqueCombinationCapacity(project);
            if (requestedOutputCount > uniqueCombinationCapacity)
            {
                issues.Add(new CollectionPreflightIssue(
                    "requested_output_exceeds_unique_combinations",
                    "The requested output count exceeds the possible unique trait combinations."));
            }
        }

        return new CollectionPreflightReport(issues);
    }

    private static long UniqueCombinationCapacity(CollectionProject project)
    {
        long capacity = 1;
        foreach (var layer in project.Layers)
        {
            if (layer.Traits.Count == 0)
            {
                return 0;
            }

            if (capacity > long.MaxValue / layer.Traits.Count)
            {
                return long.MaxValue;
            }

            capacity *= layer.Traits.Count;
        }

        return capacity;
    }
}

public sealed record CollectionPreflightIssue(string Code, string Message);

public sealed record CollectionPreflightReport(IReadOnlyList<CollectionPreflightIssue> Issues)
{
    public bool IsReady => Issues.Count == 0;
}
