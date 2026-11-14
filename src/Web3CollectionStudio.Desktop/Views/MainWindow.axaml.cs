using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Web3CollectionStudio.Application.Collections;
using Web3CollectionStudio.Application.Rendering;
using Web3CollectionStudio.Desktop.Services;
using Web3CollectionStudio.Desktop.ViewModels;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Domain.Rules;
using Web3CollectionStudio.Infrastructure.Export;
using Web3CollectionStudio.Infrastructure.ProjectStorage;

namespace Web3CollectionStudio.Desktop.Views;

public partial class MainWindow : Window
{
    private CollectionProject? activeProject;
    private readonly BoundedRenderQueue renderQueue = new(maxQueuedJobs: 1);
    private readonly LocalProjectEditor projectEditor = new();
    private LocalPngRenderService? localRenderService;
    private RenderQueueJob? activeLocalRenderJob;
    private string? lastRenderedRelativePath;

    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
        Closed += OnClosed;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.ApplyRenderQueueSnapshot(renderQueue.GetSnapshot());
        }
    }

    private async void OnClosed(object? sender, EventArgs e)
    {
        if (localRenderService is not null)
        {
            await localRenderService.DisposeAsync();
        }

        await renderQueue.DisposeAsync();
    }

    private void Navigation_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string section } && DataContext is MainViewModel viewModel)
        {
            viewModel.SelectedSection = section;
        }
    }

    private async void NewCollection_Click(object? sender, RoutedEventArgs e)
    {
        var request = await new NewCollectionWindow().ShowDialog<NewCollectionRequest?>(this);
        if (request is null || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var stateDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CyberBank",
            "Web3CollectionStudio");
        var project = new LocalCollectionProjectCreator(stateDirectory).Create(request.DisplayName, request.Profile);
        activeProject = project;
        viewModel.ActivateProject(project);
    }

    private async void OpenLocalCollection_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (!StorageProvider.CanOpen)
        {
            viewModel.ApplyLocalCollectionOpenFailure(LocalCollectionOpenFailure.InaccessibleProject);
            return;
        }

        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Open local collection project",
            AllowMultiple = false
        });
        var selectedFolder = folders.Count == 1 ? folders[0] : null;
        if (selectedFolder is null)
        {
            viewModel.ApplyLocalCollectionOpenCancelled();
            return;
        }

        var projectRoot = selectedFolder.Path.LocalPath;
        viewModel.BeginLocalCollectionOpen();
        try
        {
            var opened = new LocalCollectionProjectOpener().OpenWithOutcome(projectRoot);
            activeProject = opened.Project;
            viewModel.ApplyOpenedLocalCollection(
                opened.Project,
                recoveredFromBackup: opened.State == ProjectManifestLoadState.RecoveredFromBackup);
        }
        catch (UnauthorizedAccessException)
        {
            viewModel.ApplyLocalCollectionOpenFailure(LocalCollectionOpenFailure.InaccessibleProject);
        }
        catch (DirectoryNotFoundException)
        {
            viewModel.ApplyLocalCollectionOpenFailure(LocalCollectionOpenFailure.InvalidProject);
        }
        catch (FileNotFoundException)
        {
            viewModel.ApplyLocalCollectionOpenFailure(LocalCollectionOpenFailure.InvalidProject);
        }
        catch (JsonException)
        {
            viewModel.ApplyLocalCollectionOpenFailure(LocalCollectionOpenFailure.UnrecoverableProject);
        }
        catch (InvalidDataException)
        {
            var manifestPath = Path.Combine(projectRoot, ".collection-studio", "project.json");
            viewModel.ApplyLocalCollectionOpenFailure(
                File.Exists(manifestPath)
                    ? LocalCollectionOpenFailure.UnrecoverableProject
                    : LocalCollectionOpenFailure.InvalidProject);
        }
    }

    private void RunPreflight_Click(object? sender, RoutedEventArgs e)
    {
        if (activeProject is null || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var report = new CollectionPreflightService().Evaluate(activeProject, []);
        viewModel.ApplyPreflight(report.IsReady, report.Issues.Count);
    }

    private async void AddLayer_Click(object? sender, RoutedEventArgs e)
    {
        if (activeProject is null || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var layerName = await new NewLayerWindow().ShowDialog<string?>(this);
        if (string.IsNullOrWhiteSpace(layerName))
        {
            return;
        }

        try
        {
            var layer = projectEditor.AddLayer(activeProject, layerName);
            viewModel.ActivateProject(activeProject);
            viewModel.SelectLayer(layer.Name);
        }
        catch (Exception exception)
        {
            viewModel.ApplyPreflight(isReady: false, issueCount: 1);
            viewModel.EngineStatus = $"LOCAL INPUT BLOCKED: {exception.Message}";
        }
    }

    private void LayerSelection_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string layerName } && DataContext is MainViewModel viewModel)
        {
            viewModel.SelectLayer(layerName);
            PersistWorkspaceState(viewModel);
        }
    }

    private void WorkspaceMode_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string mode } && DataContext is MainViewModel viewModel)
        {
            viewModel.SetWorkspaceMode(mode);
            PersistWorkspaceState(viewModel);
        }
    }

    private void AssetCompare_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string relativeAssetPath } && DataContext is MainViewModel viewModel)
        {
            viewModel.SelectAssetForCompare(relativeAssetPath);
            viewModel.SetWorkspaceMode("Compare");
            PersistWorkspaceState(viewModel);
        }
    }

    private void PersistWorkspaceState(MainViewModel viewModel)
    {
        if (activeProject is null)
        {
            return;
        }

        try
        {
            projectEditor.UpdateWorkspaceState(activeProject, new CollectionWorkspaceState(
                viewModel.WorkspaceMode,
                viewModel.SelectedLayerName == "No layer selected" ? null : viewModel.SelectedLayerName,
                viewModel.CompareLeftTraitId,
                viewModel.CompareRightTraitId));
        }
        catch (Exception exception)
        {
            viewModel.EngineStatus = $"LOCAL WORKSPACE SAVE BLOCKED: {exception.Message}";
        }
    }

    private void SaveTraitWeight_Click(object? sender, RoutedEventArgs e)
    {
        if (activeProject is null || DataContext is not MainViewModel viewModel || viewModel.CompareLeftTraitId is not { } traitId)
        {
            if (DataContext is MainViewModel blockedViewModel)
            {
                blockedViewModel.EngineStatus = "LOCAL WEIGHT UPDATE BLOCKED: SELECT COMPARE A";
            }

            return;
        }

        if (!int.TryParse(TraitWeightTextBox.Text, out var weight) || weight < 0)
        {
            viewModel.EngineStatus = "LOCAL WEIGHT UPDATE BLOCKED: ENTER A NON-NEGATIVE INTEGER";
            return;
        }

        try
        {
            var layer = activeProject.Layers.Single(candidate => candidate.Traits.Any(trait => trait.Id == traitId));
            projectEditor.UpdateTraitWeight(activeProject, layer.Name, traitId, weight);
            viewModel.ActivateProject(activeProject);
            viewModel.EngineStatus = "LOCAL TRAIT WEIGHT SAVED";
        }
        catch (Exception exception)
        {
            viewModel.EngineStatus = $"LOCAL WEIGHT UPDATE BLOCKED: {exception.Message}";
        }
    }

    private void AddRequireRule_Click(object? sender, RoutedEventArgs e)
    {
        AddLocalCompatibilityRule(CompatibilityRuleType.Require);
    }

    private void AddExcludeRule_Click(object? sender, RoutedEventArgs e)
    {
        AddLocalCompatibilityRule(CompatibilityRuleType.Exclude);
    }

    private void AddLocalCompatibilityRule(CompatibilityRuleType type)
    {
        if (activeProject is null || DataContext is not MainViewModel viewModel
            || viewModel.CompareLeftTraitId is not { } firstTraitId
            || viewModel.CompareRightTraitId is not { } secondTraitId)
        {
            if (DataContext is MainViewModel blockedViewModel)
            {
                blockedViewModel.EngineStatus = "LOCAL RULE BLOCKED: SELECT COMPARE A AND B";
            }

            return;
        }

        try
        {
            var rule = type == CompatibilityRuleType.Require
                ? CompatibilityRule.Require(firstTraitId, secondTraitId)
                : CompatibilityRule.Exclude(firstTraitId, secondTraitId);
            projectEditor.AddCompatibilityRule(activeProject, rule);
            viewModel.EngineStatus = type == CompatibilityRuleType.Require
                ? "LOCAL REQUIRE RULE SAVED"
                : "LOCAL EXCLUDE RULE SAVED";
        }
        catch (Exception exception)
        {
            viewModel.EngineStatus = $"LOCAL RULE BLOCKED: {exception.Message}";
        }
    }

    private async void RenderOneLocalPng_Click(object? sender, RoutedEventArgs e)
    {
        if (activeProject is null || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var preflight = new CollectionPreflightService().Evaluate(activeProject, []);
        viewModel.ApplyPreflight(preflight.IsReady, preflight.Issues.Count);
        if (!preflight.IsReady)
        {
            viewModel.EngineStatus = "LOCAL RENDER BLOCKED: FIX PREFLIGHT ISSUES";
            return;
        }

        var sourcePaths = activeProject.Layers
            .OrderBy(layer => layer.DrawOrder)
            .Select(layer => layer.Traits.OrderByDescending(trait => trait.Weight).First())
            .Select(trait => Path.Combine(activeProject.LocalRoot, trait.RelativeAssetPath))
            .ToArray();
        if (sourcePaths.Length == 0 || sourcePaths.Any(path => !File.Exists(path)))
        {
            viewModel.EngineStatus = "LOCAL RENDER BLOCKED: LOCAL PNG ASSETS MISSING";
            return;
        }

        var outputsDirectory = Path.Combine(activeProject.LocalRoot, "outputs");
        Directory.CreateDirectory(outputsDirectory);
        var outputPath = Path.Combine(outputsDirectory, $"preview-{Guid.NewGuid():N}.png");

        if (localRenderService is not null)
        {
            await localRenderService.DisposeAsync();
        }

        localRenderService = new LocalPngRenderService(activeProject.LocalRoot, maxQueuedJobs: 1);
        activeLocalRenderJob = localRenderService.Enqueue(sourcePaths, outputPath);
        viewModel.ApplyRenderQueueSnapshot(localRenderService.GetSnapshot());
        viewModel.EngineStatus = "LOCAL PNG RENDER STARTED";

        await localRenderService.WhenIdleAsync();
        viewModel.ApplyRenderQueueSnapshot(localRenderService.GetSnapshot());
        if (activeLocalRenderJob.Status == RenderJobStatus.Completed && File.Exists(outputPath))
        {
            lastRenderedRelativePath = Path.GetRelativePath(activeProject.LocalRoot, outputPath);
            viewModel.EngineStatus = "LOCAL PNG RENDER COMPLETED";
        }
        else if (activeLocalRenderJob.Status == RenderJobStatus.Cancelled)
        {
            viewModel.EngineStatus = "LOCAL PNG RENDER CANCELLED";
        }
        else
        {
            viewModel.EngineStatus = "LOCAL PNG RENDER DID NOT COMPLETE";
        }
    }

    private void CancelLocalRender_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel || localRenderService is null || activeLocalRenderJob is null)
        {
            return;
        }

        if (localRenderService.Cancel(activeLocalRenderJob.Id))
        {
            viewModel.ApplyRenderQueueSnapshot(localRenderService.GetSnapshot());
            viewModel.EngineStatus = "LOCAL PNG RENDER CANCELLATION REQUESTED";
        }
        else
        {
            viewModel.EngineStatus = "NO CANCELLABLE LOCAL RENDER";
        }
    }

    private void ExportLocalMetadata_Click(object? sender, RoutedEventArgs e)
    {
        if (activeProject is null || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(lastRenderedRelativePath)
            || !File.Exists(Path.Combine(activeProject.LocalRoot, lastRenderedRelativePath)))
        {
            viewModel.EngineStatus = "LOCAL METADATA EXPORT BLOCKED: RENDER A LOCAL PNG FIRST";
            return;
        }

        try
        {
            var traits = activeProject.Layers
                .OrderBy(layer => layer.DrawOrder)
                .Select(layer => new LocalMetadataTrait(
                    layer.Name,
                    layer.Traits.OrderByDescending(trait => trait.Weight).First().DisplayName))
                .ToArray();
            new LocalMetadataExporter().Export(activeProject.LocalRoot,
            [
                new LocalMetadataExportItem(
                    Number: 1,
                    Name: activeProject.DisplayName,
                    Description: $"Local-only {activeProject.Profile} collection preview.",
                    RelativeImagePath: lastRenderedRelativePath,
                    Traits: traits)
            ]);
            viewModel.EngineStatus = "LOCAL METADATA EXPORTED";
        }
        catch (Exception exception)
        {
            viewModel.EngineStatus = $"LOCAL METADATA EXPORT BLOCKED: {exception.Message}";
        }
    }

    private async void ImportPngTrait_Click(object? sender, RoutedEventArgs e)
    {
        if (activeProject is null || DataContext is not MainViewModel viewModel || !StorageProvider.CanOpen)
        {
            return;
        }

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select local PNG trait",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("PNG image") { Patterns = ["*.png"] }]
        });
        var selectedFile = files.Count == 1 ? files[0] : null;
        if (selectedFile?.Path.IsFile is not true)
        {
            return;
        }

        try
        {
            var sourcePath = selectedFile.Path.LocalPath;
            var traitName = Path.GetFileNameWithoutExtension(sourcePath);
            projectEditor.ImportPngTrait(activeProject, viewModel.SelectedLayerName, sourcePath, traitName, weight: 100);
            var selectedLayerName = viewModel.SelectedLayerName;
            viewModel.ActivateProject(activeProject);
            viewModel.SelectLayer(selectedLayerName);
        }
        catch (Exception exception)
        {
            viewModel.ApplyPreflight(isReady: false, issueCount: 1);
            viewModel.EngineStatus = $"LOCAL PNG IMPORT BLOCKED: {exception.Message}";
        }
    }
}
