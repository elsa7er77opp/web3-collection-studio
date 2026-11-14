using Web3CollectionStudio.Infrastructure.Rendering;

namespace Web3CollectionStudio.Infrastructure.Tests;

public sealed class LocalRenderAuditStoreContractTests : IDisposable
{
    private readonly string auditRoot = Path.Combine(Path.GetTempPath(), $"render-audit-{Guid.NewGuid():N}");

    [Fact]
    public void Append_records_a_safe_event_without_the_absolute_output_path()
    {
        var store = new LocalRenderAuditStore(auditRoot);
        var absoluteOutputPath = Path.Combine(auditRoot, "exports", "images", "0001.png");

        store.Append(Guid.NewGuid(), "render_completed", absoluteOutputPath);

        var audit = File.ReadAllText(Path.Combine(auditRoot, "render-events.jsonl"));
        Assert.Contains("render_completed", audit);
        Assert.Contains("0001.png", audit);
        Assert.DoesNotContain(absoluteOutputPath, audit, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        if (Directory.Exists(auditRoot))
        {
            Directory.Delete(auditRoot, recursive: true);
        }
    }
}
