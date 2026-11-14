namespace Web3CollectionStudio.Desktop.Tests;

public sealed class PanelManifestFinalContractTests
{
    [Fact]
    public void Panel_manifest_declares_final_version_and_confined_payload_paths()
    {
        var manifestPath = FindFromTestAssembly("src", "Web3CollectionStudio.Desktop", "cyberbank-app.json");
        var manifest = File.ReadAllText(manifestPath);

        Assert.Contains("\"version\": \"1.0.0\"", manifest, StringComparison.Ordinal);
        Assert.Contains("\"executablePath\": \"Web3CollectionStudio.Desktop.exe\"", manifest, StringComparison.Ordinal);
        Assert.Contains("\"iconPath\": \"Assets/Brand/collection-studio-app-icon.ico\"", manifest, StringComparison.Ordinal);
        Assert.DoesNotContain("alpha", manifest, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(":\\", manifest, StringComparison.Ordinal);
    }

    private static string FindFromTestAssembly(params string[] relativeParts)
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null && !File.Exists(Path.Combine(current.FullName, "Web3CollectionStudio.slnx")))
        {
            current = current.Parent;
        }

        if (current is null)
        {
            throw new DirectoryNotFoundException("Unable to locate the Web3 Collection Studio solution root.");
        }

        return Path.Combine([current.FullName, .. relativeParts]);
    }
}
