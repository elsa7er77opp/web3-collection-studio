using SkiaSharp;

namespace Web3CollectionStudio.Infrastructure.Rendering;

public sealed class PngLayerComposer
{
    public void Compose(IReadOnlyList<string> layerPaths, string outputPath, CancellationToken cancellationToken)
    {
        if (layerPaths.Count == 0)
        {
            throw new ArgumentException("At least one local PNG layer is required.", nameof(layerPaths));
        }

        cancellationToken.ThrowIfCancellationRequested();
        using var firstLayer = DecodeLayer(layerPaths[0]);
        var imageInfo = new SKImageInfo(firstLayer.Width, firstLayer.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(imageInfo)
            ?? throw new InvalidOperationException("Unable to create a PNG render surface.");

        surface.Canvas.Clear(SKColors.Transparent);
        foreach (var layerPath in layerPaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var layer = DecodeLayer(layerPath);
            if (layer.Width != imageInfo.Width || layer.Height != imageInfo.Height)
            {
                throw new InvalidDataException("All local PNG layers must match the project canvas dimensions.");
            }

            surface.Canvas.DrawBitmap(layer, 0, 0);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var outputDirectory = Path.GetDirectoryName(outputPath)
            ?? throw new ArgumentException("The output path must include a directory.", nameof(outputPath));
        Directory.CreateDirectory(outputDirectory);
        var temporaryPath = $"{outputPath}.{Guid.NewGuid():N}.tmp";

        try
        {
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using (var stream = File.Create(temporaryPath))
            {
                data.SaveTo(stream);
            }

            cancellationToken.ThrowIfCancellationRequested();
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

    private static SKBitmap DecodeLayer(string layerPath)
    {
        return SKBitmap.Decode(layerPath)
            ?? throw new InvalidDataException("The local layer could not be decoded as a PNG image.");
    }
}
