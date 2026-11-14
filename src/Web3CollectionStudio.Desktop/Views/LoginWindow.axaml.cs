using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Web3CollectionStudio.Desktop.ViewModels;

namespace Web3CollectionStudio.Desktop.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => ConfigureForAccountState();
    }

    public event EventHandler? Authenticated;

    private void ConfigureForAccountState()
    {
        if (DataContext is not LoginViewModel viewModel)
        {
            return;
        }

        var firstRun = viewModel.IsFirstRun;
        ModeTitle.Text = firstRun ? "CREATE YOUR LOCAL WORKSPACE" : "LOCAL WORKSPACE ACCESS";
        FormTitle.Text = firstRun ? "CREATE LOCAL ACCOUNT" : "SIGN IN LOCALLY";
        FormCaption.Text = firstRun ? "Create the first administrator for this Windows profile." : "Unlock the local workspace.";
        ConfirmationPanel.IsVisible = firstRun;
    }

    private void Submit_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not LoginViewModel viewModel)
        {
            return;
        }

        var authenticated = viewModel.Submit(
            UsernameBox.Text ?? string.Empty,
            PasswordBox.Text ?? string.Empty,
            PasswordConfirmationBox.Text ?? string.Empty);

        StatusText.Text = viewModel.StatusMessage;
        if (authenticated)
        {
            Authenticated?.Invoke(this, EventArgs.Empty);
        }
    }
}
