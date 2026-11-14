using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;
using Web3CollectionStudio.Infrastructure.Security;

namespace Web3CollectionStudio.Desktop.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly LocalAccountService accounts;

    public LoginViewModel(string stateDirectory)
    {
        accounts = new LocalAccountService(Path.Combine(stateDirectory, "account.json"));
        StatusMessage = IsFirstRun
            ? "Create the first local administrator for this collection workspace."
            : "Sign in to unlock your local collection workspace.";
    }

    public bool IsFirstRun => !accounts.IsConfigured;

    [ObservableProperty]
    public partial bool IsAuthenticated { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; }

    public bool Submit(string username, string password, string passwordConfirmation)
    {
        AccountOperationResult result;
        if (IsFirstRun)
        {
            result = password == passwordConfirmation
                ? accounts.CreateFirstAccount(username, password)
                : AccountOperationResult.Failed("password_confirmation_mismatch");
        }
        else
        {
            result = accounts.SignIn(username, password, DateTimeOffset.UtcNow);
        }

        IsAuthenticated = result.Succeeded;
        StatusMessage = result.Succeeded
            ? "Local workspace unlocked."
            : result.FailureCode ?? "local_sign_in_failed";
        OnPropertyChanged(nameof(IsFirstRun));

        return result.Succeeded;
    }
}
