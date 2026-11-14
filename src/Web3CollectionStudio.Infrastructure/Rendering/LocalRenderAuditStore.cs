using System.Text.Json;

namespace Web3CollectionStudio.Infrastructure.Rendering;

public sealed class LocalRenderAuditStore
{
    private readonly object gate = new();
    private readonly string auditPath;

    public LocalRenderAuditStore(string auditRoot)
    {
        auditPath = Path.Combine(auditRoot, "render-events.jsonl");
    }

    public void Append(Guid jobId, string eventType, string outputPath)
    {
        var auditDirectory = Path.GetDirectoryName(auditPath)
            ?? throw new InvalidOperationException("The audit path must have a directory.");
        Directory.CreateDirectory(auditDirectory);
        var entry = new
        {
            timestampUtc = DateTimeOffset.UtcNow,
            jobId,
            eventType,
            outputFileName = Path.GetFileName(outputPath)
        };

        lock (gate)
        {
            File.AppendAllText(auditPath, JsonSerializer.Serialize(entry) + Environment.NewLine);
        }
    }
}
