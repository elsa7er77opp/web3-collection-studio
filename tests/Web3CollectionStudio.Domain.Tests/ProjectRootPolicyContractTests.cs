using Web3CollectionStudio.Domain.Collections;

namespace Web3CollectionStudio.Domain.Tests;

public sealed class ProjectRootPolicyContractTests
{
    [Fact]
    public void Contains_allows_a_child_asset_but_rejects_a_similarly_prefixed_sibling_root()
    {
        var root = @"C:\Collections\NeonCreatures";

        Assert.True(ProjectRootPolicy.Contains(root, @"C:\Collections\NeonCreatures\assets\background\violet.png"));
        Assert.False(ProjectRootPolicy.Contains(root, @"C:\Collections\NeonCreaturesArchive\assets\copied.png"));
    }
}
