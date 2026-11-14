using Web3CollectionStudio.Application.Collections;
using Web3CollectionStudio.Desktop.Services;
using Web3CollectionStudio.Desktop.ViewModels;
using Web3CollectionStudio.Domain.Collections;
using Web3CollectionStudio.Infrastructure.Security;

namespace Web3CollectionStudio.Desktop.Tests;

public sealed class LocalStudioAcceptanceTests
{
    [Fact]
    public void Local_account_and_every_collection_profile_complete_the_safe_initial_workflow()
    {
        var root = Path.Combine(Path.GetTempPath(), $"collection-studio-acceptance-{Guid.NewGuid():N}");
        try
        {
            var account = new LocalAccountService(Path.Combine(root, "account.json"));
            Assert.True(account.CreateFirstAccount("acceptance.user", "StrongPassword!42").Succeeded);
            Assert.True(account.SignIn("acceptance.user", "StrongPassword!42", DateTimeOffset.UtcNow).Succeeded);

            var creator = new LocalCollectionProjectCreator(root);
            VerifyProfile(creator, CollectionProfile.Pfp, "PFP", "1024 × 1024 px");
            VerifyProfile(creator, CollectionProfile.BadgeGaming, "BADGE / GAMING", "1024 × 1024 px");
            VerifyProfile(creator, CollectionProfile.DigitalArt, "DIGITAL ART", "2048 × 2048 px");
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    private static void VerifyProfile(
        LocalCollectionProjectCreator creator,
        CollectionProfile profile,
        string expectedProfileLabel,
        string expectedCanvas)
    {
        var project = creator.Create($"Acceptance {profile}", profile);
        var workstation = new MainViewModel();
        workstation.ActivateProject(project);

        var report = new CollectionPreflightService().Evaluate(project, []);
        workstation.ApplyPreflight(report.IsReady, report.Issues.Count);

        Assert.True(File.Exists(Path.Combine(project.LocalRoot, ".collection-studio", "project.json")));
        Assert.True(workstation.HasActiveProject);
        Assert.Equal(expectedProfileLabel, workstation.ActiveProfileLabel);
        Assert.Equal(expectedCanvas, workstation.ActiveCanvasSummary);
        Assert.True(workstation.CanRunPreflight);
        Assert.False(report.IsReady);
        Assert.Equal("BLOCKED", workstation.PreflightStatusLabel);
    }
}
