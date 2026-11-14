using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.IO;
using Web3CollectionStudio.Desktop.ViewModels;
using Web3CollectionStudio.Desktop.Views;

namespace Web3CollectionStudio.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var stateDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CyberBank",
                "Web3CollectionStudio");

            Directory.CreateDirectory(stateDirectory);

            var splash = new SplashWindow
            {
                DataContext = new SplashViewModel(reducedMotion: false)
            };

            desktop.MainWindow = splash;
            splash.StartupComplete += (_, _) => ShowLogin(desktop, splash, stateDirectory);
            splash.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ShowLogin(IClassicDesktopStyleApplicationLifetime desktop, SplashWindow splash, string stateDirectory)
    {
        var login = new LoginWindow
        {
            DataContext = new LoginViewModel(stateDirectory)
        };

        desktop.MainWindow = login;
        login.Authenticated += (_, _) =>
        {
            var workstation = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            desktop.MainWindow = workstation;
            workstation.Show();
            login.Close();
        };

        login.Show();
        splash.Close();
    }
}
