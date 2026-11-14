using SkiaSharp;
using Web3CollectionStudio.Infrastructure.Rendering;

namespace Web3CollectionStudio.Infrastructure.Tests;

public sealed class PngLayerComposerContractTests : IDisposable
{
    private readonly string workspace = Path.Combine(Path.GetTempPath(), $"png-render-{Guid.NewGuid():N}");

    [Fact]
    public void Compose_writes_an_atomic_png_with_layers_drawn_in_order()
    {
        Directory.CreateDirectory(workspace);
        var background = Path.Combine(workspace, "background.png");
        var foreground = Path.Combine(workspace, "foreground.png");
        var output = Path.Combine(workspace, "output.png");
        WriteSolidPng(background, 4, 4, SKColors.Red);
        WriteSinglePixelPng(foreground, 4, 4, 0, 0, SKColors.Blue);

        new PngLayerComposer().Compose([background, foreground], output, CancellationToken.None);

        using var rendered = SKBitmap.Decode(output);
        Assert.Equal(SKColors.Blue, rendered.GetPixel(0, 0));
        Assert.Equal(SKColors.Red, rendered.GetPixel(3, 3));
        Assert.Empty(Directory.GetFiles(workspace, "*.tmp"));
    }

    [Fact]
    public void Compose_cancelled_before_work_leaves_no_output_or_temporary_artifact()
    {
        Directory.CreateDirectory(workspace);
        var source = Path.Combine(workspace, "source.png");
        var output = Path.Combine(workspace, "cancelled.png");
        WriteSolidPng(source, 4, 4, SKColors.Indigo);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        Assert.Throws<OperationCanceledException>(() =>
            new PngLayerComposer().Compose([source], output, cancellation.Token));

        Assert.False(File.Exists(output));
        Assert.Empty(Directory.GetFiles(workspace, "*.tmp"));
    }

    public void Dispose()
    {
        if (Directory.Exists(workspace))
        {
            Directory.Delete(workspace, recursive: true);
        }
    }

    private static void WriteSolidPng(string path, int width, int height, SKColor color)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(color);
        using var image = SKImage.FromBitmap(bitmap);
        using var stream = File.OpenWrite(path);
        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
    }

    private static void WriteSinglePixelPng(string path, int width, int height, int x, int y, SKColor color)
    {
        using var bitmap = new SKBitmap(width, height);
        bitmap.Erase(SKColors.Transparent);
        bitmap.SetPixel(x, y, color);
        using var image = SKImage.FromBitmap(bitmap);
        using var stream = File.OpenWrite(path);
        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
    }
}
