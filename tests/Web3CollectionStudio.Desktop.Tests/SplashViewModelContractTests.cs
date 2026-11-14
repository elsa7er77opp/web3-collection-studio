using Web3CollectionStudio.Desktop.ViewModels;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class SplashViewModelContractTests
{
    [Fact]
    public void Default_splash_reports_local_workspace_initialization_without_brand_animation_policy()
    {
        var splash = new SplashViewModel(reducedMotion: false);

        Assert.Equal("INITIALIZING COLLECTION WORKSPACE...", splash.StatusLine);
        Assert.Equal(72, splash.ProgressPercent);
    }
}
