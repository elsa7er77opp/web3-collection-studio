using SkiaSharp;
using Web3CollectionStudio.Application.Rendering;
using Web3CollectionStudio.Desktop.Services;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class LocalPngRenderServiceContractTests : IDisposable
{
    private readonly string workspace = Path.Combine(Path.GetTempPath(), $"local-render-{Guid.NewGuid():N}");

    [Fact]
    public async Task Enqueue_composes_a_local_png_through_the_bounded_background_queue()
    {
        Directory.CreateDirectory(workspace);
        var source = Path.Combine(workspace, "source.png");
        var output = Path.Combine(workspace, "output.png");
        WriteSolidPng(source, SKColors.Indigo);
        await using var service = new LocalPngRenderService(maxQueuedJobs: 1);

        var job = service.Enqueue([source], output);
        await service.WhenIdleAsync();

        Assert.Equal(RenderJobStatus.Completed, job.Status);
        Assert.Equal(100, job.ProgressPercent);
        Assert.True(File.Exists(output));
    }

    [Fact]
    public async Task Enqueue_records_a_safe_completed_audit_event()
    {
        Directory.CreateDirectory(workspace);
        var source = Path.Combine(workspace, "source.png");
        var output = Path.Combine(workspace, "output.png");
        WriteSolidPng(source, SKColors.Indigo);
        await using var service = new LocalPngRenderService(workspace, maxQueuedJobs: 1);

        service.Enqueue([source], output);
        await service.WhenIdleAsync();

        var audit = File.ReadAllText(Path.Combine(workspace, "render-events.jsonl"));
        Assert.Contains("render_completed", audit);
        Assert.DoesNotContain(output, audit, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        if (Directory.Exists(workspace))
        {
            Directory.Delete(workspace, recursive: true);
        }
    }

    private static void WriteSolidPng(string path, SKColor color)
    {
        using var bitmap = new SKBitmap(2, 2);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(color);
        using var image = SKImage.FromBitmap(bitmap);
        using var stream = File.OpenWrite(path);
        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
    }
}
