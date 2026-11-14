using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Web3CollectionStudio.Application.Rendering;
using Web3CollectionStudio.Infrastructure.Rendering;

namespace Web3CollectionStudio.Desktop.Services;

public sealed class LocalPngRenderService : IAsyncDisposable
{
    private readonly BoundedRenderQueue queue;
    private readonly PngLayerComposer composer = new();
    private readonly LocalRenderAuditStore? auditStore;

    public LocalPngRenderService(int maxQueuedJobs)
        : this(maxQueuedJobs, null)
    {
    }

    public LocalPngRenderService(string auditRoot, int maxQueuedJobs)
        : this(maxQueuedJobs, new LocalRenderAuditStore(auditRoot))
    {
    }

    private LocalPngRenderService(int maxQueuedJobs, LocalRenderAuditStore? auditStore)
    {
        queue = new BoundedRenderQueue(maxQueuedJobs);
        this.auditStore = auditStore;
    }

    public RenderQueueJob Enqueue(IReadOnlyList<string> orderedLayerPaths, string outputPath)
    {
        return queue.EnqueueWithContext(context =>
        {
            auditStore?.Append(context.JobId, "render_started", outputPath);
            try
            {
                context.ReportProgress(10);
                composer.Compose(orderedLayerPaths, outputPath, context.CancellationToken);
                context.ReportProgress(95);
                auditStore?.Append(context.JobId, "render_completed", outputPath);
                return Task.CompletedTask;
            }
            catch (OperationCanceledException)
            {
                auditStore?.Append(context.JobId, "render_cancelled", outputPath);
                throw;
            }
            catch
            {
                auditStore?.Append(context.JobId, "render_failed", outputPath);
                throw;
            }
        });
    }

    public bool Cancel(Guid jobId) => queue.Cancel(jobId);

    public RenderQueueSnapshot GetSnapshot() => queue.GetSnapshot();

    public Task WhenIdleAsync() => queue.WhenIdleAsync();

    public ValueTask DisposeAsync() => queue.DisposeAsync();
}
