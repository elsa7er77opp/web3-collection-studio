namespace Web3CollectionStudio.Domain.Rules;

public sealed record CompatibilityRule(Guid FirstTraitId, Guid SecondTraitId, CompatibilityRuleType Type)
{
    public static CompatibilityRule Require(Guid dependentTraitId, Guid requiredTraitId)
    {
        return new CompatibilityRule(dependentTraitId, requiredTraitId, CompatibilityRuleType.Require);
    }

    public static CompatibilityRule Exclude(Guid firstTraitId, Guid secondTraitId)
    {
        return new CompatibilityRule(firstTraitId, secondTraitId, CompatibilityRuleType.Exclude);
    }

    public bool IsSatisfiedBy(IReadOnlySet<Guid> selectedTraitIds)
    {
        return Type switch
        {
            CompatibilityRuleType.Require => !selectedTraitIds.Contains(FirstTraitId)
                || selectedTraitIds.Contains(SecondTraitId),
            CompatibilityRuleType.Exclude => !selectedTraitIds.Contains(FirstTraitId)
                || !selectedTraitIds.Contains(SecondTraitId),
            _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, "Unsupported compatibility rule.")
        };
    }
}

public enum CompatibilityRuleType
{
    Require,
    Exclude
}
