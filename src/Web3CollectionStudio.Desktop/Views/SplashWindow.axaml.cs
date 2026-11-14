using Avalonia.Controls;
using Avalonia.Threading;
using System;
using Web3CollectionStudio.Desktop.ViewModels;

namespace Web3CollectionStudio.Desktop.Views;

public partial class SplashWindow : Window
{
    private readonly DispatcherTimer launchTimer = new() { Interval = TimeSpan.FromMilliseconds(1350) };

    public SplashWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
        Closed += OnClosed;
        launchTimer.Tick += CompleteLaunch;
    }

    public event EventHandler? StartupComplete;

    private void OnOpened(object? sender, EventArgs e)
    {
        launchTimer.Start();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        launchTimer.Stop();
    }

    private void CompleteLaunch(object? sender, EventArgs e)
    {
        launchTimer.Stop();
        StartupComplete?.Invoke(this, EventArgs.Empty);
    }
}
