using System;
using System.IO;
using System.Linq;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Layers;
using Web3CollectionStudio.Domain.Rules;
using Web3CollectionStudio.Infrastructure.ProjectStorage;

namespace Web3CollectionStudio.Desktop.Services;

public sealed class LocalProjectEditor
{
    private static readonly byte[] PngSignature = [137, 80, 78, 71, 13, 10, 26, 10];
    private readonly LocalProjectManifestStore manifestStore;

    public LocalProjectEditor(LocalProjectManifestStore? manifestStore = null)
    {
        this.manifestStore = manifestStore ?? new LocalProjectManifestStore();
    }

    public CollectionLayer AddLayer(CollectionProject project, string name)
    {
        ArgumentNullException.ThrowIfNull(project);
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A local layer name is required.", nameof(name));
        }

        var normalizedName = name.Trim();
        if (project.Layers.Any(layer => string.Equals(layer.Name, normalizedName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A local layer with this name already exists.");
        }

        var nextDrawOrder = project.Layers.Count == 0
            ? 0
            : project.Layers.Max(layer => layer.DrawOrder) + 1;
        var layer = project.AddLayer(normalizedName, nextDrawOrder);
        manifestStore.Save(project);
        return layer;
    }

    public TraitOption ImportPngTrait(
        CollectionProject project,
        string layerName,
        string sourcePngPath,
        string displayName,
        int weight)
    {
        ArgumentNullException.ThrowIfNull(project);
        if (string.IsNullOrWhiteSpace(layerName))
        {
            throw new ArgumentException("A local layer must be selected.", nameof(layerName));
        }

        var layer = project.Layers.SingleOrDefault(candidate =>
            string.Equals(candidate.Name, layerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("The selected local layer no longer exists.");

        if (!File.Exists(sourcePngPath)
            || !string.Equals(Path.GetExtension(sourcePngPath), ".png", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Select an existing local PNG asset.");
        }

        ValidatePngSignature(sourcePngPath);

        var assetsDirectory = Path.Combine(project.LocalRoot, "assets", SanitizePathSegment(layer.Name));
        if (!ProjectRootPolicy.Contains(project.LocalRoot, assetsDirectory))
        {
            throw new InvalidOperationException("The asset destination must remain inside the local project.");
        }

        Directory.CreateDirectory(assetsDirectory);
        var destinationPath = Path.Combine(
            assetsDirectory,
            $"{Path.GetFileNameWithoutExtension(sourcePngPath)}-{Guid.NewGuid():N}.png");
        if (!ProjectRootPolicy.Contains(project.LocalRoot, destinationPath))
        {
            throw new InvalidOperationException("The asset destination must remain inside the local project.");
        }

        var temporaryPath = $"{destinationPath}.{Guid.NewGuid():N}.tmp";
        File.Copy(sourcePngPath, temporaryPath, overwrite: false);
        File.Move(temporaryPath, destinationPath, overwrite: false);
        var relativeAssetPath = Path.GetRelativePath(project.LocalRoot, destinationPath);

        try
        {
            var trait = layer.AddTrait(displayName, relativeAssetPath, weight);
            manifestStore.Save(project);
            return trait;
        }
        catch
        {
            File.Delete(destinationPath);
            throw;
        }
    }

    public TraitOption UpdateTraitWeight(CollectionProject project, string layerName, Guid traitId, int weight)
    {
        ArgumentNullException.ThrowIfNull(project);
        if (string.IsNullOrWhiteSpace(layerName))
        {
            throw new ArgumentException("A local layer must be selected.", nameof(layerName));
        }

        var layer = project.Layers.SingleOrDefault(candidate =>
            string.Equals(candidate.Name, layerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("The selected local layer no longer exists.");

        var updated = layer.UpdateTraitWeight(traitId, weight);
        manifestStore.Save(project);
        return updated;
    }

    public void AddCompatibilityRule(CollectionProject project, CompatibilityRule rule)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(rule);

        project.AddCompatibilityRule(rule);
        manifestStore.Save(project);
    }

    public void UpdateWorkspaceState(CollectionProject project, CollectionWorkspaceState workspaceState)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(workspaceState);

        project.UpdateWorkspaceState(workspaceState);
        manifestStore.Save(project);
    }

    private static void ValidatePngSignature(string sourcePngPath)
    {
        using var stream = File.OpenRead(sourcePngPath);
        var header = new byte[PngSignature.Length];
        if (stream.Read(header, 0, header.Length) != PngSignature.Length
            || !header.SequenceEqual(PngSignature))
        {
            throw new InvalidDataException("The selected asset is not a valid PNG file.");
        }
    }

    private static string SanitizePathSegment(string value)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var sanitized = new string(value
            .Trim()
            .Select(character => invalidCharacters.Contains(character) ? '_' : character)
            .ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "layer" : sanitized;
    }
}
