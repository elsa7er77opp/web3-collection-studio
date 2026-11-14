using System;
using System.IO;
using Xunit;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class MainWindowOpenLocalCollectionContractTests
{
    [Fact]
    public void Main_window_binds_open_local_project_to_the_native_local_collection_workflow()
    {
        var xaml = File.ReadAllText(FindFromTestAssembly("src", "Web3CollectionStudio.Desktop", "Views", "MainWindow.axaml"));
        var codeBehind = File.ReadAllText(FindFromTestAssembly("src", "Web3CollectionStudio.Desktop", "Views", "MainWindow.axaml.cs"));

        Assert.Contains("Content=\"Open local project\" Click=\"OpenLocalCollection_Click\"", xaml, StringComparison.Ordinal);
        Assert.Contains("StorageProvider.OpenFolderPickerAsync", codeBehind, StringComparison.Ordinal);
        Assert.Contains("OpenWithOutcome", codeBehind, StringComparison.Ordinal);
        Assert.Contains("ApplyOpenedLocalCollection", codeBehind, StringComparison.Ordinal);
        Assert.Contains("ApplyLocalCollectionOpenFailure", codeBehind, StringComparison.Ordinal);
        Assert.Contains("catch (JsonException)", codeBehind, StringComparison.Ordinal);
        Assert.Contains("Text=\"{Binding LocalCollectionOpenFeedback}\"", xaml, StringComparison.Ordinal);
    }

    [Fact]
    public void Main_window_exposes_visible_local_weight_and_require_exclude_rule_commands()
    {
        var xaml = File.ReadAllText(FindFromTestAssembly("src", "Web3CollectionStudio.Desktop", "Views", "MainWindow.axaml"));
        var codeBehind = File.ReadAllText(FindFromTestAssembly("src", "Web3CollectionStudio.Desktop", "Views", "MainWindow.axaml.cs"));

        Assert.Contains("Content=\"Save trait weight\" Click=\"SaveTraitWeight_Click\"", xaml, StringComparison.Ordinal);
        Assert.Contains("Content=\"Require A → B\" Click=\"AddRequireRule_Click\"", xaml, StringComparison.Ordinal);
        Assert.Contains("Content=\"Exclude A × B\" Click=\"AddExcludeRule_Click\"", xaml, StringComparison.Ordinal);
        Assert.Contains("UpdateTraitWeight", codeBehind, StringComparison.Ordinal);
        Assert.Contains("AddCompatibilityRule", codeBehind, StringComparison.Ordinal);
        Assert.Contains("UpdateWorkspaceState", codeBehind, StringComparison.Ordinal);
    }

    [Fact]
    public void Main_window_exposes_visible_local_render_cancel_and_metadata_export_commands()
    {
        var xaml = File.ReadAllText(FindFromTestAssembly("src", "Web3CollectionStudio.Desktop", "Views", "MainWindow.axaml"));
        var codeBehind = File.ReadAllText(FindFromTestAssembly("src", "Web3CollectionStudio.Desktop", "Views", "MainWindow.axaml.cs"));

        Assert.Contains("Content=\"Render one local PNG\" Click=\"RenderOneLocalPng_Click\"", xaml, StringComparison.Ordinal);
        Assert.Contains("Content=\"Cancel local render\" Click=\"CancelLocalRender_Click\"", xaml, StringComparison.Ordinal);
        Assert.Contains("Content=\"Export local metadata\" Click=\"ExportLocalMetadata_Click\"", xaml, StringComparison.Ordinal);
        Assert.Contains("LocalPngRenderService", codeBehind, StringComparison.Ordinal);
        Assert.Contains("LocalMetadataExporter", codeBehind, StringComparison.Ordinal);
    }

    private static string FindFromTestAssembly(params string[] relativeSegments)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine([directory.FullName, .. relativeSegments]);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Unable to locate Web3 Collection Studio source from the test assembly.");
    }
}
