using System.Text.Json;

namespace Web3CollectionStudio.Infrastructure.Export;

public sealed record LocalMetadataTrait(string TraitType, string Value);

public sealed record LocalMetadataExportItem(
    int Number,
    string Name,
    string Description,
    string RelativeImagePath,
    IReadOnlyList<LocalMetadataTrait> Traits);

public sealed class LocalMetadataExporter
{
    public void Export(string exportRoot, IReadOnlyList<LocalMetadataExportItem> items)
    {
        var metadataDirectory = Path.Combine(exportRoot, "metadata");
        Directory.CreateDirectory(metadataDirectory);

        foreach (var item in items)
        {
            if (Path.IsPathRooted(item.RelativeImagePath))
            {
                throw new ArgumentException("Metadata image paths must be relative to the export directory.", nameof(items));
            }

            var document = new
            {
                name = item.Name,
                description = item.Description,
                image = item.RelativeImagePath.Replace('\\', '/'),
                attributes = item.Traits.Select(trait => new
                {
                    trait_type = trait.TraitType,
                    value = trait.Value
                })
            };
            var outputPath = Path.Combine(metadataDirectory, $"{item.Number:D4}.json");
            var temporaryPath = $"{outputPath}.{Guid.NewGuid():N}.tmp";
            try
            {
                File.WriteAllText(temporaryPath, JsonSerializer.Serialize(document));
                File.Move(temporaryPath, outputPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }
    }
}
