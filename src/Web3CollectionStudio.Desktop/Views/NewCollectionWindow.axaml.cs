using Avalonia.Controls;
using Avalonia.Interactivity;
using Web3CollectionStudio.Domain.Collections;

namespace Web3CollectionStudio.Desktop.Views;

public sealed record NewCollectionRequest(string DisplayName, CollectionProfile Profile);

public partial class NewCollectionWindow : Window
{
    public NewCollectionWindow()
    {
        InitializeComponent();
    }

    private void Create_Click(object? sender, RoutedEventArgs e)
    {
        var displayName = CollectionNameBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(displayName))
        {
            ValidationText.Text = "Enter a collection name before continuing.";
            return;
        }

        Close(new NewCollectionRequest(displayName, SelectedProfile()));
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private CollectionProfile SelectedProfile() =>
        BadgeProfile.IsChecked == true ? CollectionProfile.BadgeGaming :
        DigitalArtProfile.IsChecked == true ? CollectionProfile.DigitalArt :
        CollectionProfile.Pfp;
}
