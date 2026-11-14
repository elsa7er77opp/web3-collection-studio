using Web3CollectionStudio.Desktop.ViewModels;
using Web3CollectionStudio.Application.Rendering;
using Web3CollectionStudio.Domain.Collections;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class MainViewModelContractTests
{
    [Fact]
    public void Default_workstation_state_exposes_product_context_and_local_engine_readiness()
    {
        var workstation = new MainViewModel();

        Assert.Equal("Web3 Collection Studio", workstation.ProductName);
        Assert.Equal("Collections", workstation.SelectedSection);
        Assert.Equal("LOCAL RENDER ENGINE READY", workstation.EngineStatus);
    }

    [Fact]
    public void Workstation_exposes_the_full_local_collection_navigation()
    {
        var workstation = new MainViewModel();

        Assert.Equal(
            ["Dashboard", "Collections", "Projects", "Layers", "Assets", "Traits", "Rules", "Metadata", "Export History"],
            workstation.NavigationSections);
    }

    [Fact]
    public void Workstation_activates_a_new_local_project_context()
    {
        var workstation = new MainViewModel();

        workstation.ActivateProject("Midnight Masks");

        Assert.Equal("Midnight Masks", workstation.ActiveProjectName);
        Assert.Equal("Collections", workstation.SelectedSection);
    }

    [Fact]
    public void Workstation_projects_real_local_collection_details_into_the_visible_workspace_state()
    {
        var workstation = new MainViewModel();
        var project = CollectionProject.Create(
            "Signal Archive",
            @"C:\Collections\SignalArchive",
            CollectionProfile.DigitalArt);
        project.AddLayer("Background", drawOrder: 0);

        workstation.ActivateProject(project);

        Assert.True(workstation.HasActiveProject);
        Assert.Equal("DIGITAL ART", workstation.ActiveProfileLabel);
        Assert.Equal("2048 × 2048 px", workstation.ActiveCanvasSummary);
        Assert.Equal(1, workstation.ActiveLayerCount);
        Assert.Equal("READY FOR LAYERS", workstation.WorkspaceReadinessLabel);
    }

    [Fact]
    public void Workstation_surfaces_a_blocked_local_preflight_result_without_exposing_project_paths()
    {
        var workstation = new MainViewModel();
        var project = CollectionProject.Create(
            "Signal Archive",
            @"C:\Collections\SignalArchive",
            CollectionProfile.Pfp);

        workstation.ActivateProject(project);
        workstation.ApplyPreflight(isReady: false, issueCount: 1);

        Assert.True(workstation.CanRunPreflight);
        Assert.Equal("BLOCKED", workstation.PreflightStatusLabel);
        Assert.Equal("1 local issue requires attention", workstation.PreflightSummary);
        Assert.DoesNotContain("SignalArchive", workstation.PreflightSummary);
    }

    [Fact]
    public void Workstation_projects_an_idle_render_queue_snapshot_into_a_visible_local_status()
    {
        var workstation = new MainViewModel();

        workstation.ApplyRenderQueueSnapshot(new RenderQueueSnapshot(
            QueuedCount: 0,
            IsRendering: false,
            ActiveProgressPercent: 0));

        Assert.Equal("IDLE · 0 JOBS", workstation.RenderQueueStatusLabel);
        Assert.Equal("WORKER STANDING BY", workstation.RenderQueueDetailLabel);
    }

    [Fact]
    public void Workstation_exposes_cancel_only_while_a_local_render_is_active()
    {
        var workstation = new MainViewModel();

        Assert.False(workstation.CanCancelLocalRender);
        Assert.Equal("No active local render", workstation.CancellationAvailabilityLabel);

        workstation.ApplyRenderQueueSnapshot(new RenderQueueSnapshot(
            QueuedCount: 1,
            IsRendering: false,
            ActiveProgressPercent: 0));

        Assert.True(workstation.CanCancelLocalRender);
        Assert.Equal("Cancellation available", workstation.CancellationAvailabilityLabel);

        workstation.ApplyRenderQueueSnapshot(new RenderQueueSnapshot(
            QueuedCount: 0,
            IsRendering: true,
            ActiveProgressPercent: 10));

        Assert.True(workstation.CanCancelLocalRender);
        Assert.Equal("Cancellation available", workstation.CancellationAvailabilityLabel);

        workstation.ApplyRenderQueueSnapshot(new RenderQueueSnapshot(
            QueuedCount: 0,
            IsRendering: false,
            ActiveProgressPercent: 0));

        Assert.False(workstation.CanCancelLocalRender);
        Assert.Equal("No active local render", workstation.CancellationAvailabilityLabel);
    }

    [Fact]
    public void Workstation_exposes_an_ordered_local_layer_ledger_and_selected_layer_inspector()
    {
        var project = CollectionProject.Create("Signal Archive", @"C:\Collections\SignalArchive", CollectionProfile.Pfp);
        var foreground = project.AddLayer("Foreground", drawOrder: 10);
        foreground.AddTrait("Violet Spark", "assets/foreground/violet-spark.png", weight: 70);
        project.AddLayer("Background", drawOrder: 0);
        var workstation = new MainViewModel();

        workstation.ActivateProject(project);
        workstation.SelectLayer("Foreground");

        Assert.Collection(
            workstation.LocalLayers,
            first => Assert.Equal("Background", first.Name),
            second => Assert.Equal("Foreground", second.Name));
        Assert.Equal("Foreground", workstation.SelectedLayerName);
        Assert.Equal(1, workstation.SelectedLayerTraitCount);
        Assert.Equal("1 local trait", workstation.SelectedLayerSummary);
    }

    [Fact]
    public void Workstation_projects_real_traits_into_gallery_and_compare_modes()
    {
        var project = CollectionProject.Create("Signal Archive", @"C:\Collections\SignalArchive", CollectionProfile.Pfp);
        var layer = project.AddLayer("Background", drawOrder: 0);
        var violet = layer.AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var indigo = layer.AddTrait("Indigo Dawn", "assets/background/indigo-dawn.png", weight: 20);
        var workstation = new MainViewModel();

        workstation.ActivateProject(project);
        workstation.SetWorkspaceMode("Gallery");
        workstation.SelectAssetForCompare("assets/background/violet-grid.png");
        workstation.SelectAssetForCompare("assets/background/indigo-dawn.png");

        Assert.True(workstation.IsGalleryMode);
        Assert.False(workstation.IsCanvasMode);
        Assert.Collection(
            workstation.LocalAssetGallery,
            first => Assert.Equal("Violet Grid", first.TraitName),
            second => Assert.Equal("Indigo Dawn", second.TraitName));
        Assert.Equal("Violet Grid", workstation.CompareLeftLabel);
        Assert.Equal("Indigo Dawn", workstation.CompareRightLabel);
        Assert.Equal(violet.Id, workstation.CompareLeftTraitId);
        Assert.Equal(indigo.Id, workstation.CompareRightTraitId);
    }

    [Fact]
    public void Workstation_restores_the_saved_local_workspace_selection_when_activating_a_reopened_project()
    {
        var project = CollectionProject.Create("Signal Archive", @"C:\Collections\SignalArchive", CollectionProfile.Pfp);
        var background = project.AddLayer("Background", drawOrder: 0)
            .AddTrait("Violet Grid", "assets/background/violet-grid.png", weight: 80);
        var foreground = project.AddLayer("Foreground", drawOrder: 1)
            .AddTrait("Moon Mask", "assets/foreground/moon-mask.png", weight: 20);
        project.UpdateWorkspaceState(new CollectionWorkspaceState(
            WorkspaceMode: "Compare",
            SelectedLayerName: "Foreground",
            CompareLeftTraitId: foreground.Id,
            CompareRightTraitId: background.Id));
        var workstation = new MainViewModel();

        workstation.ActivateProject(project);

        Assert.True(workstation.IsCompareMode);
        Assert.Equal("Foreground", workstation.SelectedLayerName);
        Assert.Equal(foreground.Id, workstation.CompareLeftTraitId);
        Assert.Equal(background.Id, workstation.CompareRightTraitId);
    }
    [Fact]
    public void Workstation_surfaces_loading_and_explicit_local_backup_recovery_when_opening_a_collection()
    {
        var workstation = new MainViewModel();
        var project = CollectionProject.Create("Recovered Archive", @"C:\Collections\RecoveredArchive", CollectionProfile.Pfp);

        workstation.BeginLocalCollectionOpen();

        Assert.True(workstation.IsOpeningLocalCollection);
        Assert.Equal("OPENING LOCAL COLLECTION", workstation.EngineStatus);

        workstation.ApplyOpenedLocalCollection(project, recoveredFromBackup: true);

        Assert.False(workstation.IsOpeningLocalCollection);
        Assert.True(workstation.HasActiveProject);
        Assert.Equal("RECOVERED FROM LOCAL BACKUP", workstation.EngineStatus);
        Assert.Contains("Primary project manifest could not be read", workstation.LocalCollectionOpenFeedback);
        Assert.Contains("restored from local backup", workstation.LocalCollectionOpenFeedback);
    }

    [Fact]
    public void Workstation_rejects_an_invalid_local_folder_without_replacing_the_current_project()
    {
        var workstation = new MainViewModel();
        workstation.ActivateProject(CollectionProject.Create("Current Archive", @"C:\Collections\CurrentArchive", CollectionProfile.Pfp));

        workstation.BeginLocalCollectionOpen();
        workstation.ApplyLocalCollectionOpenFailure(LocalCollectionOpenFailure.InvalidProject);

        Assert.False(workstation.IsOpeningLocalCollection);
        Assert.Equal("Current Archive", workstation.ActiveProjectName);
        Assert.Equal("INVALID LOCAL COLLECTION", workstation.EngineStatus);
        Assert.Contains("No project was opened", workstation.LocalCollectionOpenFeedback);
        Assert.Contains("does not contain a CyberBank collection manifest", workstation.LocalCollectionOpenFeedback);
    }

    [Fact]
    public void Workstation_returns_to_a_safe_idle_state_when_open_local_collection_is_cancelled()
    {
        var workstation = new MainViewModel();

        workstation.BeginLocalCollectionOpen();
        workstation.ApplyLocalCollectionOpenCancelled();

        Assert.False(workstation.IsOpeningLocalCollection);
        Assert.Equal("LOCAL COLLECTION OPEN CANCELLED", workstation.EngineStatus);
        Assert.Contains("No project was opened", workstation.LocalCollectionOpenFeedback);
    }

    [Fact]
    public void Workstation_surfaces_an_inaccessible_local_folder_without_crashing_or_creating_a_project()
    {
        var workstation = new MainViewModel();

        workstation.BeginLocalCollectionOpen();
        workstation.ApplyLocalCollectionOpenFailure(LocalCollectionOpenFailure.InaccessibleProject);

        Assert.False(workstation.IsOpeningLocalCollection);
        Assert.False(workstation.HasActiveProject);
        Assert.Equal("INACCESSIBLE LOCAL COLLECTION", workstation.EngineStatus);
        Assert.Contains("could not be accessed", workstation.LocalCollectionOpenFeedback);
    }
}
