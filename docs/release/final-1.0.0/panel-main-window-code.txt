using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CyberBankPanel.Desktop.Services;
using CyberBankPanel.Desktop.ViewModels;

namespace CyberBankPanel.Desktop.Views;

public partial class MainWindow : Window
{
    private readonly PanelLaunchService launchService = new();
    private readonly PanelRegistrationService registrationService = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void InspectProduct_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: PanelProductItem product } && DataContext is MainViewModel panel)
        {
            panel.SelectProduct(product);
        }
    }

    private async void RegisterLocalApp_Click(object? sender, RoutedEventArgs e)
    {
        var selectedFolders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Choose an installed CyberBank application folder",
            AllowMultiple = false
        });
        var installRoot = selectedFolders.FirstOrDefault()?.TryGetLocalPath();
        if (string.IsNullOrWhiteSpace(installRoot))
        {
            return;
        }

        if (DataContext is not MainViewModel panel)
        {
            return;
        }

        var registryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CyberBank",
            "Panel",
            "registry.json");
        try
        {
            DataContext = registrationService.Register(registryPath, installRoot);
        }
        catch (Exception)
        {
            panel.StatusMessage = "REGISTRATION REJECTED — VALID LOCAL MANIFEST REQUIRED";
        }
    }

    private void LaunchSelectedProduct_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel { SelectedProduct: { } product } panel)
        {
            return;
        }

        var panelDataRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CyberBank",
            "Panel");
        var registryPath = Path.Combine(panelDataRoot, "registry.json");
        try
        {
            var startInfo = launchService.Prepare(registryPath, panelDataRoot, product.AppId);
            Process.Start(startInfo);
            panel.StatusMessage = $"LAUNCH DISPATCHED — {product.DisplayName.ToUpperInvariant()}";
        }
        catch (Exception)
        {
            panel.StatusMessage = "LAUNCH BLOCKED — CHECK LOCAL AVAILABILITY";
        }
    }

    private void ViewSafeDiagnostics_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel panel)
        {
            panel.ShowSelectedProductDiagnostics();
        }
    }
}
