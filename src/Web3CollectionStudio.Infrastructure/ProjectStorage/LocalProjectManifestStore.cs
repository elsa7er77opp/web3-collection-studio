using System.Text.Json;
using Web3CollectionStudio.Application.Collections;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Rules;

namespace Web3CollectionStudio.Infrastructure.ProjectStorage;

public sealed class LocalProjectManifestStore : ICollectionProjectPersistence
{
    private const int CurrentSchemaVersion = 2;
    private const int LegacySchemaVersion = 1;
    private const string ManifestDirectoryName = ".collection-studio";
    private const string ManifestFileName = "project.json";
    private const string BackupManifestFileName = "project.json.bak";

    public void Save(CollectionProject project)
    {
        var manifestDirectory = Path.Combine(project.LocalRoot, ManifestDirectoryName);
        Directory.CreateDirectory(manifestDirectory);

        var manifestPath = Path.Combine(manifestDirectory, ManifestFileName);
        var backupPath = Path.Combine(manifestDirectory, BackupManifestFileName);
        if (File.Exists(manifestPath))
        {
            File.Copy(manifestPath, backupPath, overwrite: true);
        }

        var document = new ProjectManifestDocument(
            CurrentSchemaVersion,
            project.Id,
            project.DisplayName,
            project.LocalRoot,
            project.Profile,
            project.Canvas.Width,
            project.Canvas.Height,
            project.Layers
                .Select(layer => new LayerDocument(
                    layer.Name,
                    layer.DrawOrder,
                    layer.Traits
                        .Select(trait => new TraitDocument(
                            trait.DisplayName,
                            trait.RelativeAssetPath,
                            trait.Weight,
                            trait.Id))
                        .ToArray()))
                .ToArray(),
            project.CompatibilityRules
                .Select(rule => new CompatibilityRuleDocument(rule.FirstTraitId, rule.SecondTraitId, rule.Type))
                .ToArray(),
            new WorkspaceStateDocument(
                project.WorkspaceState.WorkspaceMode,
                project.WorkspaceState.SelectedLayerName,
                project.WorkspaceState.CompareLeftTraitId,
                project.WorkspaceState.CompareRightTraitId));
        var temporaryPath = $"{manifestPath}.{Guid.NewGuid():N}.tmp";

        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(document));
        File.Move(temporaryPath, manifestPath, overwrite: true);
    }

    public CollectionProject Load(string projectRoot)
    {
        return LoadWithOutcome(projectRoot).Project;
    }

    public ProjectManifestLoadResult LoadWithOutcome(string projectRoot)
    {
        var manifestPath = Path.Combine(projectRoot, ManifestDirectoryName, ManifestFileName);
        var backupPath = Path.Combine(projectRoot, ManifestDirectoryName, BackupManifestFileName);
        try
        {
            return new ProjectManifestLoadResult(ReadProject(manifestPath), ProjectManifestLoadState.PrimaryLoaded);
        }
        catch (Exception exception) when (exception is JsonException or InvalidDataException && File.Exists(backupPath))
        {
            var recoveredProject = ReadProject(backupPath);
            return new ProjectManifestLoadResult(recoveredProject, ProjectManifestLoadState.RecoveredFromBackup);
        }
    }

    private static CollectionProject ReadProject(string manifestPath)
    {
        var document = JsonSerializer.Deserialize<ProjectManifestDocument>(File.ReadAllText(manifestPath))
            ?? throw new InvalidDataException("The collection project manifest is empty.");

        if (document.SchemaVersion is not (CurrentSchemaVersion or LegacySchemaVersion))
        {
            throw new InvalidDataException("The collection project manifest schema is not supported.");
        }

        var project = new CollectionProject(
            document.ProjectId,
            document.DisplayName,
            document.LocalRoot,
            document.Profile,
            new CanvasSpec(document.CanvasWidth, document.CanvasHeight));

        foreach (var layerDocument in document.Layers ?? [])
        {
            var layer = project.AddLayer(layerDocument.Name, layerDocument.DrawOrder);
            foreach (var traitDocument in layerDocument.Traits)
            {
                layer.AddTrait(
                    traitDocument.TraitId is { } traitId && traitId != Guid.Empty ? traitId : Guid.NewGuid(),
                    traitDocument.DisplayName,
                    traitDocument.RelativeAssetPath,
                    traitDocument.Weight);
            }
        }

        foreach (var ruleDocument in document.CompatibilityRules ?? [])
        {
            project.AddCompatibilityRule(new CompatibilityRule(
                ruleDocument.FirstTraitId,
                ruleDocument.SecondTraitId,
                ruleDocument.Type));
        }

        if (document.WorkspaceState is not null)
        {
            project.UpdateWorkspaceState(new CollectionWorkspaceState(
                document.WorkspaceState.WorkspaceMode,
                document.WorkspaceState.SelectedLayerName,
                document.WorkspaceState.CompareLeftTraitId,
                document.WorkspaceState.CompareRightTraitId));
        }

        return project;
    }

    private sealed record ProjectManifestDocument(
        int SchemaVersion,
        Guid ProjectId,
        string DisplayName,
        string LocalRoot,
        CollectionProfile Profile,
        int CanvasWidth,
        int CanvasHeight,
        IReadOnlyList<LayerDocument>? Layers = null,
        IReadOnlyList<CompatibilityRuleDocument>? CompatibilityRules = null,
        WorkspaceStateDocument? WorkspaceState = null);

    private sealed record LayerDocument(
        string Name,
        int DrawOrder,
        IReadOnlyList<TraitDocument> Traits);

    private sealed record TraitDocument(
        string DisplayName,
        string RelativeAssetPath,
        int Weight,
        Guid? TraitId = null);

    private sealed record CompatibilityRuleDocument(
        Guid FirstTraitId,
        Guid SecondTraitId,
        CompatibilityRuleType Type);

    private sealed record WorkspaceStateDocument(
        string WorkspaceMode,
        string? SelectedLayerName,
        Guid? CompareLeftTraitId,
        Guid? CompareRightTraitId);
}

public enum ProjectManifestLoadState
{
    PrimaryLoaded,
    RecoveredFromBackup
}

public sealed record ProjectManifestLoadResult(
    CollectionProject Project,
    ProjectManifestLoadState State);
