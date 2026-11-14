using Web3CollectionStudio.Infrastructure.Security;

namespace Web3CollectionStudio.Infrastructure.Tests;

public sealed class LocalAccountServiceContractTests : IDisposable
{
    private readonly string stateDirectory = Path.Combine(Path.GetTempPath(), $"collection-studio-auth-{Guid.NewGuid():N}");

    [Fact]
    public void CreateFirstAccount_hashes_password_and_allows_a_later_local_sign_in()
    {
        Directory.CreateDirectory(stateDirectory);
        var statePath = Path.Combine(stateDirectory, "account.json");
        var accounts = new LocalAccountService(statePath);

        var setup = accounts.CreateFirstAccount("Studio Admin", "Synthetic!Passw0rd2026");
        var signIn = accounts.SignIn("Studio Admin", "Synthetic!Passw0rd2026", DateTimeOffset.UtcNow);

        Assert.True(setup.Succeeded);
        Assert.True(signIn.Succeeded);
        Assert.DoesNotContain("Synthetic!Passw0rd2026", File.ReadAllText(statePath));
    }

    [Fact]
    public void SignIn_locks_the_local_account_after_five_failed_attempts()
    {
        Directory.CreateDirectory(stateDirectory);
        var accounts = new LocalAccountService(Path.Combine(stateDirectory, "account.json"));
        accounts.CreateFirstAccount("Studio Admin", "Synthetic!Passw0rd2026");
        var now = DateTimeOffset.UtcNow;

        for (var attempt = 0; attempt < 5; attempt++)
        {
            Assert.False(accounts.SignIn("Studio Admin", "Wrong!Passw0rd2026", now).Succeeded);
        }

        var locked = accounts.SignIn("Studio Admin", "Synthetic!Passw0rd2026", now);

        Assert.False(locked.Succeeded);
        Assert.Equal("account_temporarily_locked", locked.FailureCode);
    }

    public void Dispose()
    {
        if (Directory.Exists(stateDirectory))
        {
            Directory.Delete(stateDirectory, recursive: true);
        }
    }
}
