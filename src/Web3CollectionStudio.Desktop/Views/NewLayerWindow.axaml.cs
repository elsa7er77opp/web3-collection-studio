using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Web3CollectionStudio.Desktop.Views;

public partial class NewLayerWindow : Window
{
    public NewLayerWindow()
    {
        InitializeComponent();
    }

    private void Add_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(LayerNameBox.Text))
        {
            StatusText.Text = "Enter a local layer name.";
            return;
        }

        Close(LayerNameBox.Text.Trim());
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
