using SkiaSharp;
using Web3CollectionStudio.Application.Collections;
using Web3CollectionStudio.Application.Rendering;
using Web3CollectionStudio.Desktop.Services;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Rules;
using Web3CollectionStudio.Infrastructure.Export;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class FinalLocalWorkflowAcceptanceTests : IDisposable
{
    private readonly string workspace = Path.Combine(Path.GetTempPath(), $"collection-studio-final-local-{Guid.NewGuid():N}");

    [Fact]
    public async Task Every_local_profile_completes_the_project_asset_rule_render_export_and_reopen_workflow()
    {
        Directory.CreateDirectory(workspace);
        var sourceRoot = Path.Combine(workspace, "source-assets");
        Directory.CreateDirectory(sourceRoot);
        var indigoPng = Path.Combine(sourceRoot, "indigo.png");
        var violetPng = Path.Combine(sourceRoot, "violet.png");
        WriteSolidPng(indigoPng, SKColors.Indigo);
        WriteSolidPng(violetPng, SKColors.MediumPurple);
        var creator = new LocalCollectionProjectCreator(Path.Combine(workspace, "collections"));

        foreach (var profile in new[] { CollectionProfile.Pfp, CollectionProfile.BadgeGaming, CollectionProfile.DigitalArt })
        {
            var project = creator.Create($"Final {profile}", profile);
            var editor = new LocalProjectEditor();
            var background = editor.AddLayer(project, "Background");
            var foreground = editor.AddLayer(project, "Foreground");
            var indigo = editor.ImportPngTrait(project, background.Name, indigoPng, "Indigo Field", weight: 80);
            var violet = editor.ImportPngTrait(project, foreground.Name, violetPng, "Violet Mask", weight: 20);
            editor.UpdateTraitWeight(project, foreground.Name, violet.Id, weight: 35);
            editor.AddCompatibilityRule(project, CompatibilityRule.Require(violet.Id, indigo.Id));
            editor.UpdateWorkspaceState(project, new CollectionWorkspaceState(
                "Compare",
                foreground.Name,
                violet.Id,
                indigo.Id));

            var preflight = new CollectionPreflightService().Evaluate(project, []);
            Assert.True(preflight.IsReady);

            var outputPath = Path.Combine(project.LocalRoot, "outputs", "preview.png");
            await using (var render = new LocalPngRenderService(project.LocalRoot, maxQueuedJobs: 1))
            {
                var job = render.Enqueue(
                    project.Layers
                        .OrderBy(layer => layer.DrawOrder)
                        .Select(layer => Path.Combine(project.LocalRoot, layer.Traits.Single().RelativeAssetPath))
                        .ToArray(),
                    outputPath);
                await render.WhenIdleAsync();
                Assert.Equal(RenderJobStatus.Completed, job.Status);
            }

            var relativeOutput = Path.GetRelativePath(project.LocalRoot, outputPath);
            new LocalMetadataExporter().Export(project.LocalRoot,
            [
                new LocalMetadataExportItem(
                    1,
                    project.DisplayName,
                    "Local Final acceptance preview.",
                    relativeOutput,
                    [new LocalMetadataTrait(background.Name, indigo.DisplayName), new LocalMetadataTrait(foreground.Name, violet.DisplayName)])
            ]);

            var reopened = new LocalCollectionProjectOpener().OpenWithOutcome(project.LocalRoot).Project;
            Assert.Equal(profile, reopened.Profile);
            Assert.Equal(2, reopened.Layers.Count);
            Assert.Single(reopened.CompatibilityRules);
            Assert.Equal("Compare", reopened.WorkspaceState.WorkspaceMode);
            Assert.Equal(foreground.Name, reopened.WorkspaceState.SelectedLayerName);
            Assert.True(File.Exists(outputPath));
            var metadata = File.ReadAllText(Path.Combine(project.LocalRoot, "metadata", "0001.json"));
            Assert.Contains(relativeOutput.Replace('\\', '/'), metadata);
            Assert.DoesNotContain(project.LocalRoot, metadata, StringComparison.OrdinalIgnoreCase);
        }
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
        using var bitmap = new SKBitmap(16, 16);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(color);
        using var image = SKImage.FromBitmap(bitmap);
        using var stream = File.OpenWrite(path);
        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
    }
}
