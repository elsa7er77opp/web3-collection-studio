using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Web3CollectionStudio.Application.Rendering;
using Web3CollectionStudio.Domain.Collections;

namespace Web3CollectionStudio.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string ProductName { get; set; } = "Web3 Collection Studio";

    [ObservableProperty]
    public partial string SelectedSection { get; set; } = "Collections";

    [ObservableProperty]
    public partial string EngineStatus { get; set; } = "LOCAL RENDER ENGINE READY";

    [ObservableProperty]
    public partial bool IsOpeningLocalCollection { get; set; }

    [ObservableProperty]
    public partial string LocalCollectionOpenFeedback { get; set; } = "Open an existing local collection project to restore its saved workspace.";

    [ObservableProperty]
    public partial string ActiveProjectName { get; set; } = "No collection open";

    [ObservableProperty]
    public partial bool HasActiveProject { get; set; }

    [ObservableProperty]
    public partial string ActiveProfileLabel { get; set; } = "NO ACTIVE PROFILE";

    [ObservableProperty]
    public partial string ActiveCanvasSummary { get; set; } = "—";

    [ObservableProperty]
    public partial int ActiveLayerCount { get; set; }

    [ObservableProperty]
    public partial string WorkspaceReadinessLabel { get; set; } = "CREATE A LOCAL COLLECTION";

    [ObservableProperty]
    public partial bool CanRunPreflight { get; set; }

    [ObservableProperty]
    public partial string PreflightStatusLabel { get; set; } = "NOT RUN";

    [ObservableProperty]
    public partial string PreflightSummary { get; set; } = "Open a local collection to validate assets and rules.";

    [ObservableProperty]
    public partial string RenderQueueStatusLabel { get; set; } = "IDLE · 0 JOBS";

    [ObservableProperty]
    public partial string RenderQueueDetailLabel { get; set; } = "WORKER STANDING BY";

    [ObservableProperty]
    public partial bool CanCancelLocalRender { get; set; }

    [ObservableProperty]
    public partial string CancellationAvailabilityLabel { get; set; } = "No active local render";

    [ObservableProperty]
    public partial string SelectedLayerName { get; set; } = "No layer selected";

    [ObservableProperty]
    public partial int SelectedLayerTraitCount { get; set; }

    [ObservableProperty]
    public partial string SelectedLayerSummary { get; set; } = "Create a local layer to begin.";

    public ObservableCollection<LocalLayerLedgerItem> LocalLayers { get; } = [];

    public ObservableCollection<LocalAssetGalleryItem> LocalAssetGallery { get; } = [];

    [ObservableProperty]
    public partial string WorkspaceMode { get; set; } = "Canvas";

    [ObservableProperty]
    public partial bool IsCanvasMode { get; set; } = true;

    [ObservableProperty]
    public partial bool IsGalleryMode { get; set; }

    [ObservableProperty]
    public partial bool IsCompareMode { get; set; }

    [ObservableProperty]
    public partial string CompareLeftLabel { get; set; } = "Choose a local asset";

    [ObservableProperty]
    public partial string CompareRightLabel { get; set; } = "Choose a second asset";

    [ObservableProperty]
    public partial string? CompareLeftAssetUri { get; set; }

    [ObservableProperty]
    public partial string? CompareRightAssetUri { get; set; }

    [ObservableProperty]
    private partial string? CompareLeftAssetPath { get; set; }

    [ObservableProperty]
    private partial string? CompareRightAssetPath { get; set; }

    [ObservableProperty]
    public partial Guid? CompareLeftTraitId { get; set; }

    [ObservableProperty]
    public partial Guid? CompareRightTraitId { get; set; }

    public IReadOnlyList<string> NavigationSections { get; } =
    [
        "Dashboard",
        "Collections",
        "Projects",
        "Layers",
        "Assets",
        "Traits",
        "Rules",
        "Metadata",
        "Export History"
    ];

    public void ActivateProject(string displayName)
    {
        ActiveProjectName = displayName;
        HasActiveProject = true;
        SelectedSection = "Collections";
    }

    public void ActivateProject(CollectionProject project)
    {
        ArgumentNullException.ThrowIfNull(project);

        ActiveProjectName = project.DisplayName;
        ActiveProfileLabel = FormatProfile(project.Profile);
        ActiveCanvasSummary = $"{project.Canvas.Width} × {project.Canvas.Height} px";
        ActiveLayerCount = project.Layers.Count;
        WorkspaceReadinessLabel = "READY FOR LAYERS";
        HasActiveProject = true;
        CanRunPreflight = true;
        PreflightStatusLabel = "NOT RUN";
        PreflightSummary = "Validate local layers, PNG assets, and rules.";
        RefreshLocalLayers(project);
        RefreshLocalAssetGallery(project);
        RestoreWorkspaceState(project.WorkspaceState);
        SelectedSection = "Collections";
    }

    public void BeginLocalCollectionOpen()
    {
        IsOpeningLocalCollection = true;
        EngineStatus = "OPENING LOCAL COLLECTION";
        LocalCollectionOpenFeedback = "Choose an existing local collection project folder.";
    }

    public void ApplyOpenedLocalCollection(CollectionProject project, bool recoveredFromBackup)
    {
        ActivateProject(project);
        IsOpeningLocalCollection = false;
        EngineStatus = recoveredFromBackup ? "RECOVERED FROM LOCAL BACKUP" : "LOCAL COLLECTION OPENED";
        LocalCollectionOpenFeedback = recoveredFromBackup
            ? "Primary project manifest could not be read. Project restored from local backup."
            : "Existing local collection project opened successfully.";
    }

    public void ApplyLocalCollectionOpenFailure(LocalCollectionOpenFailure failure)
    {
        IsOpeningLocalCollection = false;
        EngineStatus = failure switch
        {
            LocalCollectionOpenFailure.InvalidProject => "INVALID LOCAL COLLECTION",
            LocalCollectionOpenFailure.UnrecoverableProject => "UNRECOVERABLE LOCAL COLLECTION",
            LocalCollectionOpenFailure.InaccessibleProject => "INACCESSIBLE LOCAL COLLECTION",
            _ => "LOCAL COLLECTION OPEN BLOCKED"
        };
        LocalCollectionOpenFeedback = failure switch
        {
            LocalCollectionOpenFailure.InvalidProject => "No project was opened. The selected folder does not contain a CyberBank collection manifest.",
            LocalCollectionOpenFailure.UnrecoverableProject => "No project was opened. The primary project manifest is invalid and no usable local backup is available.",
            LocalCollectionOpenFailure.InaccessibleProject => "No project was opened. The selected local folder could not be accessed.",
            _ => "No project was opened. Local project validation blocked this action."
        };
    }

    public void ApplyLocalCollectionOpenCancelled()
    {
        IsOpeningLocalCollection = false;
        EngineStatus = "LOCAL COLLECTION OPEN CANCELLED";
        LocalCollectionOpenFeedback = "No project was opened. Local collection selection was cancelled.";
    }

    public void SelectLayer(string layerName)
    {
        var selectedLayer = LocalLayers.FirstOrDefault(layer => string.Equals(layer.Name, layerName, StringComparison.Ordinal));
        if (selectedLayer is null)
        {
            SelectedLayerName = "No layer selected";
            SelectedLayerTraitCount = 0;
            SelectedLayerSummary = "Choose a local layer from the explorer.";
            return;
        }

        SelectedLayerName = selectedLayer.Name;
        SelectedLayerTraitCount = selectedLayer.TraitCount;
        SelectedLayerSummary = $"{selectedLayer.TraitCount} local trait{(selectedLayer.TraitCount == 1 ? string.Empty : "s")}";
    }

    public void ApplyPreflight(bool isReady, int issueCount)
    {
        if (issueCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(issueCount));
        }

        PreflightStatusLabel = isReady ? "READY" : "BLOCKED";
        PreflightSummary = isReady
            ? "Local collection passed preflight"
            : $"{issueCount} local issue{(issueCount == 1 ? string.Empty : "s")} require{(issueCount == 1 ? "s" : string.Empty)} attention";
    }

    public void ApplyRenderQueueSnapshot(RenderQueueSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        RenderQueueStatusLabel = snapshot.IsRendering
            ? $"RENDERING · {snapshot.QueuedCount} QUEUED"
            : $"IDLE · {snapshot.QueuedCount} JOBS";
        RenderQueueDetailLabel = snapshot.IsRendering
            ? $"ACTIVE · {snapshot.ActiveProgressPercent}%"
            : "WORKER STANDING BY";
        CanCancelLocalRender = snapshot.IsRendering || snapshot.QueuedCount > 0;
        CancellationAvailabilityLabel = CanCancelLocalRender
            ? "Cancellation available"
            : "No active local render";
    }

    public void SetWorkspaceMode(string mode)
    {
        WorkspaceMode = mode switch
        {
            "Canvas" or "Gallery" or "Compare" => mode,
            _ => "Canvas"
        };
        IsCanvasMode = string.Equals(WorkspaceMode, "Canvas", StringComparison.Ordinal);
        IsGalleryMode = string.Equals(WorkspaceMode, "Gallery", StringComparison.Ordinal);
        IsCompareMode = string.Equals(WorkspaceMode, "Compare", StringComparison.Ordinal);
    }

    public void SelectAssetForCompare(string relativeAssetPath)
    {
        var asset = LocalAssetGallery.FirstOrDefault(candidate =>
            string.Equals(candidate.RelativeAssetPath, relativeAssetPath, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("The selected local asset is not part of this collection.");

        if (CompareLeftAssetPath is null)
        {
            CompareLeftAssetPath = asset.RelativeAssetPath;
            CompareLeftTraitId = asset.TraitId;
            CompareLeftLabel = asset.TraitName;
            CompareLeftAssetUri = asset.AssetUri;
            return;
        }

        CompareRightAssetPath = asset.RelativeAssetPath;
        CompareRightTraitId = asset.TraitId;
        CompareRightLabel = asset.TraitName;
        CompareRightAssetUri = asset.AssetUri;
    }

    private void RefreshLocalLayers(CollectionProject project)
    {
        LocalLayers.Clear();
        foreach (var layer in project.Layers.OrderBy(layer => layer.DrawOrder))
        {
            LocalLayers.Add(new LocalLayerLedgerItem(layer.Name, layer.DrawOrder, layer.Traits.Count));
        }

        if (LocalLayers.Count == 0)
        {
            SelectLayer(string.Empty);
            return;
        }

        SelectLayer(LocalLayers[0].Name);
    }

    private void RefreshLocalAssetGallery(CollectionProject project)
    {
        LocalAssetGallery.Clear();
        foreach (var layer in project.Layers.OrderBy(layer => layer.DrawOrder))
        {
            foreach (var trait in layer.Traits)
            {
                LocalAssetGallery.Add(new LocalAssetGalleryItem(
                    trait.Id,
                    layer.Name,
                    trait.DisplayName,
                    trait.RelativeAssetPath,
                    trait.Weight,
                    new Uri(Path.GetFullPath(Path.Combine(project.LocalRoot, trait.RelativeAssetPath))).AbsoluteUri));
            }
        }

        CompareLeftAssetPath = null;
        CompareRightAssetPath = null;
        CompareLeftTraitId = null;
        CompareRightTraitId = null;
        CompareLeftLabel = "Choose a local asset";
        CompareRightLabel = "Choose a second asset";
        CompareLeftAssetUri = null;
        CompareRightAssetUri = null;
    }

    private void RestoreWorkspaceState(CollectionWorkspaceState workspaceState)
    {
        if (workspaceState.SelectedLayerName is not null)
        {
            SelectLayer(workspaceState.SelectedLayerName);
        }

        if (workspaceState.CompareLeftTraitId is { } leftTraitId)
        {
            SelectAssetForCompare(LocalAssetGallery.Single(asset => asset.TraitId == leftTraitId).RelativeAssetPath);
        }

        if (workspaceState.CompareRightTraitId is { } rightTraitId)
        {
            SelectAssetForCompare(LocalAssetGallery.Single(asset => asset.TraitId == rightTraitId).RelativeAssetPath);
        }

        SetWorkspaceMode(workspaceState.WorkspaceMode);
    }

    private static string FormatProfile(CollectionProfile profile) => profile switch
    {
        CollectionProfile.Pfp => "PFP",
        CollectionProfile.BadgeGaming => "BADGE / GAMING",
        CollectionProfile.DigitalArt => "DIGITAL ART",
        _ => "CUSTOM"
    };
}

public sealed record LocalLayerLedgerItem(string Name, int DrawOrder, int TraitCount);

public sealed record LocalAssetGalleryItem(
    Guid TraitId,
    string LayerName,
    string TraitName,
    string RelativeAssetPath,
    int Weight,
    string AssetUri);

public enum LocalCollectionOpenFailure
{
    InvalidProject,
    UnrecoverableProject,
    InaccessibleProject
}
