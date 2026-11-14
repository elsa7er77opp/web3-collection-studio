using System.Text.Json;

namespace Web3CollectionStudio.Infrastructure.ProjectStorage;

public sealed class LocalProjectAuditStore
{
    private const string ManifestDirectoryName = ".collection-studio";
    private const string AuditFileName = "project-events.jsonl";
    private readonly object gate = new();

    public void AppendBackupRecovery(string projectRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectRoot);
        var auditDirectory = Path.Combine(Path.GetFullPath(projectRoot), ManifestDirectoryName);
        Directory.CreateDirectory(auditDirectory);
        var auditPath = Path.Combine(auditDirectory, AuditFileName);
        var entry = new
        {
            timestampUtc = DateTimeOffset.UtcNow,
            eventType = "project_recovered_from_backup"
        };

        lock (gate)
        {
            File.AppendAllText(auditPath, JsonSerializer.Serialize(entry) + Environment.NewLine);
        }
    }
}
