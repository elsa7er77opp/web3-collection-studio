using System.Text.Json;
using Web3CollectionStudio.Infrastructure.Export;

namespace Web3CollectionStudio.Infrastructure.Tests;

public sealed class LocalMetadataExporterContractTests : IDisposable
{
    private readonly string exportRoot = Path.Combine(Path.GetTempPath(), $"metadata-export-{Guid.NewGuid():N}");

    [Fact]
    public void Export_writes_open_sea_style_json_with_relative_image_path_and_traits()
    {
        var exporter = new LocalMetadataExporter();
        var output = new LocalMetadataExportItem(
            1,
            "Midnight Masks #0001",
            "Local collection export",
            "images/0001.png",
            [new LocalMetadataTrait("Background", "Indigo")]);

        exporter.Export(exportRoot, [output]);

        var json = File.ReadAllText(Path.Combine(exportRoot, "metadata", "0001.json"));
        using var document = JsonDocument.Parse(json);
        Assert.Equal("Midnight Masks #0001", document.RootElement.GetProperty("name").GetString());
        Assert.Equal("images/0001.png", document.RootElement.GetProperty("image").GetString());
        Assert.Equal("Background", document.RootElement.GetProperty("attributes")[0].GetProperty("trait_type").GetString());
        Assert.Equal("Indigo", document.RootElement.GetProperty("attributes")[0].GetProperty("value").GetString());
    }

    public void Dispose()
    {
        if (Directory.Exists(exportRoot))
        {
            Directory.Delete(exportRoot, recursive: true);
        }
    }
}
