using Web3CollectionStudio.Desktop.ViewModels;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class LoginViewModelContractTests : IDisposable
{
    private readonly string stateDirectory = Path.Combine(Path.GetTempPath(), $"collection-studio-login-{Guid.NewGuid():N}");

    [Fact]
    public void FirstRun_setup_authenticates_the_local_administrator()
    {
        var login = new LoginViewModel(stateDirectory);

        var succeeded = login.Submit("Studio Admin", "Synthetic!Passw0rd2026", "Synthetic!Passw0rd2026");

        Assert.True(login.IsFirstRun is false);
        Assert.True(login.IsAuthenticated);
        Assert.True(succeeded);
    }

    public void Dispose()
    {
        if (Directory.Exists(stateDirectory))
        {
            Directory.Delete(stateDirectory, recursive: true);
        }
    }
}
