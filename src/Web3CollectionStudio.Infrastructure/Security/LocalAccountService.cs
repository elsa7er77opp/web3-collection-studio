using System.Security.Cryptography;
using System.Text.Json;

namespace Web3CollectionStudio.Infrastructure.Security;

public sealed class LocalAccountService
{
    private const int SaltBytes = 16;
    private const int HashBytes = 32;
    private const int Iterations = 210_000;
    private const int MaximumFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(10);
    private readonly string statePath;

    public LocalAccountService(string statePath)
    {
        this.statePath = statePath;
    }

    public bool IsConfigured => File.Exists(statePath);

    public AccountOperationResult CreateFirstAccount(string username, string password)
    {
        if (File.Exists(statePath))
        {
            return AccountOperationResult.Failed("account_already_exists");
        }

        if (string.IsNullOrWhiteSpace(username) || !IsStrongPassword(password))
        {
            return AccountOperationResult.Failed("invalid_account_setup");
        }

        var salt = RandomNumberGenerator.GetBytes(SaltBytes);
        var hash = Derive(password, salt);
        var state = new LocalAccountState(
            username.Trim(),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash),
            0,
            null);
        WriteStateAtomically(state);

        return AccountOperationResult.SucceededResult();
    }

    public AccountOperationResult SignIn(string username, string password, DateTimeOffset now)
    {
        if (!File.Exists(statePath))
        {
            return AccountOperationResult.Failed("account_not_initialized");
        }

        var state = JsonSerializer.Deserialize<LocalAccountState>(File.ReadAllText(statePath));
        if (state is null || !string.Equals(state.Username, username.Trim(), StringComparison.Ordinal))
        {
            return AccountOperationResult.Failed("invalid_credentials");
        }

        if (state.LockoutUntilUtc is not null && state.LockoutUntilUtc > now)
        {
            return AccountOperationResult.Failed("account_temporarily_locked");
        }

        var salt = Convert.FromBase64String(state.SaltBase64);
        var expected = Convert.FromBase64String(state.HashBase64);
        var actual = Derive(password, salt);

        if (CryptographicOperations.FixedTimeEquals(expected, actual))
        {
            if (state.FailedAttempts != 0 || state.LockoutUntilUtc is not null)
            {
                WriteStateAtomically(state with { FailedAttempts = 0, LockoutUntilUtc = null });
            }

            return AccountOperationResult.SucceededResult();
        }

        var attempts = state.FailedAttempts + 1;
        DateTimeOffset? lockoutUntil = attempts >= MaximumFailedAttempts ? now.Add(LockoutDuration) : null;
        WriteStateAtomically(state with { FailedAttempts = attempts, LockoutUntilUtc = lockoutUntil });

        return lockoutUntil is null
            ? AccountOperationResult.Failed("invalid_credentials")
            : AccountOperationResult.Failed("account_temporarily_locked");
    }

    private static byte[] Derive(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashBytes);
    }

    private void WriteStateAtomically(LocalAccountState state)
    {
        var directory = Path.GetDirectoryName(statePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temporaryPath = $"{statePath}.{Guid.NewGuid():N}.tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(state));
        File.Move(temporaryPath, statePath, overwrite: true);
    }

    private static bool IsStrongPassword(string password)
    {
        return password.Length >= 12
            && password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit)
            && password.Any(character => !char.IsLetterOrDigit(character));
    }

    private sealed record LocalAccountState(
        string Username,
        string SaltBase64,
        string HashBase64,
        int FailedAttempts,
        DateTimeOffset? LockoutUntilUtc);
}

public sealed record AccountOperationResult(bool Succeeded, string? FailureCode)
{
    public static AccountOperationResult SucceededResult() => new(true, null);

    public static AccountOperationResult Failed(string failureCode) => new(false, failureCode);
}
