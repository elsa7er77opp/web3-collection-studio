namespace Web3CollectionStudio.Application.Rendering;

public enum RenderJobStatus
{
    Queued,
    Running,
    Cancelled,
    Completed,
    Failed
}

public sealed class RenderWorkContext
{
    private readonly Action<int> reportProgress;

    internal RenderWorkContext(Guid jobId, CancellationToken cancellationToken, Action<int> reportProgress)
    {
        JobId = jobId;
        CancellationToken = cancellationToken;
        this.reportProgress = reportProgress;
    }

    public Guid JobId { get; }

    public CancellationToken CancellationToken { get; }

    public void ReportProgress(int progressPercent)
    {
        reportProgress(Math.Clamp(progressPercent, 0, 100));
    }
}

public sealed class RenderQueueJob
{
    internal RenderQueueJob(Func<RenderWorkContext, Task> work)
    {
        Work = work;
    }

    internal Func<RenderWorkContext, Task> Work { get; }

    internal CancellationTokenSource Cancellation { get; } = new();

    public Guid Id { get; } = Guid.NewGuid();

    public RenderJobStatus Status { get; internal set; } = RenderJobStatus.Queued;

    public int ProgressPercent { get; internal set; }
}

public sealed record RenderQueueSnapshot(
    int QueuedCount,
    bool IsRendering,
    int ActiveProgressPercent);

public sealed class BoundedRenderQueue : IAsyncDisposable
{
    private readonly object gate = new();
    private readonly int maxQueuedJobs;
    private readonly LinkedList<RenderQueueJob> queuedJobs = [];
    private readonly Dictionary<Guid, LinkedListNode<RenderQueueJob>> queuedJobNodes = [];
    private readonly SemaphoreSlim pendingSignal = new(0);
    private readonly CancellationTokenSource shutdown = new();
    private readonly Task worker;
    private RenderQueueJob? activeJob;
    private TaskCompletionSource idle = CompletedIdleSignal();

    public BoundedRenderQueue(int maxQueuedJobs)
    {
        if (maxQueuedJobs < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxQueuedJobs));
        }

        this.maxQueuedJobs = maxQueuedJobs;
        worker = Task.Run(ProcessLoopAsync);
    }

    public RenderQueueJob Enqueue(Func<CancellationToken, Task> work)
    {
        return EnqueueWithContext(context => work(context.CancellationToken));
    }

    public RenderQueueJob EnqueueWithContext(Func<RenderWorkContext, Task> work)
    {
        if (!TryEnqueueWithContext(work, out var job))
        {
            throw new InvalidOperationException("The local render queue is at capacity.");
        }

        return job!;
    }

    public bool TryEnqueue(Func<CancellationToken, Task> work, out RenderQueueJob? job)
    {
        return TryEnqueueWithContext(context => work(context.CancellationToken), out job);
    }

    public bool TryEnqueueWithContext(Func<RenderWorkContext, Task> work, out RenderQueueJob? job)
    {
        ArgumentNullException.ThrowIfNull(work);
        lock (gate)
        {
            if (queuedJobs.Count >= maxQueuedJobs)
            {
                job = null;
                return false;
            }

            job = new RenderQueueJob(work);
            var node = queuedJobs.AddLast(job);
            queuedJobNodes.Add(job.Id, node);
            ResetIdleIfNecessary();
            pendingSignal.Release();
            return true;
        }
    }

    public bool Cancel(Guid jobId)
    {
        lock (gate)
        {
            if (queuedJobNodes.Remove(jobId, out var queuedNode))
            {
                queuedJobs.Remove(queuedNode);
                queuedNode.Value.Cancellation.Cancel();
                queuedNode.Value.Status = RenderJobStatus.Cancelled;
                CompleteIdleIfNecessary();
                return true;
            }

            if (activeJob?.Id == jobId)
            {
                activeJob.Cancellation.Cancel();
                activeJob.Status = RenderJobStatus.Cancelled;
                return true;
            }

            return false;
        }
    }

    public Task WhenIdleAsync()
    {
        lock (gate)
        {
            return idle.Task;
        }
    }

    public RenderQueueSnapshot GetSnapshot()
    {
        lock (gate)
        {
            return new RenderQueueSnapshot(
                queuedJobs.Count,
                activeJob is not null,
                activeJob?.ProgressPercent ?? 0);
        }
    }

    public async ValueTask DisposeAsync()
    {
        shutdown.Cancel();
        lock (gate)
        {
            foreach (var queuedJob in queuedJobs)
            {
                queuedJob.Cancellation.Cancel();
                queuedJob.Status = RenderJobStatus.Cancelled;
            }

            queuedJobs.Clear();
            queuedJobNodes.Clear();
            activeJob?.Cancellation.Cancel();
            CompleteIdleIfNecessary();
        }

        pendingSignal.Release();
        await worker.ConfigureAwait(false);
        pendingSignal.Dispose();
        shutdown.Dispose();
    }

    private async Task ProcessLoopAsync()
    {
        try
        {
            while (true)
            {
                await pendingSignal.WaitAsync(shutdown.Token).ConfigureAwait(false);
                RenderQueueJob? job;
                lock (gate)
                {
                    if (queuedJobs.First is null)
                    {
                        if (shutdown.IsCancellationRequested)
                        {
                            return;
                        }

                        continue;
                    }

                    job = queuedJobs.First.Value;
                    queuedJobNodes.Remove(job.Id);
                    queuedJobs.RemoveFirst();
                    activeJob = job;
                    job.Status = RenderJobStatus.Running;
                }

                try
                {
                    await job.Work(new RenderWorkContext(job.Id, job.Cancellation.Token, progress => job.ProgressPercent = progress)).ConfigureAwait(false);
                    if (!job.Cancellation.IsCancellationRequested)
                    {
                        job.ProgressPercent = 100;
                        job.Status = RenderJobStatus.Completed;
                    }
                }
                catch (OperationCanceledException) when (job.Cancellation.IsCancellationRequested)
                {
                    job.Status = RenderJobStatus.Cancelled;
                }
                catch
                {
                    job.Status = RenderJobStatus.Failed;
                }
                finally
                {
                    job.Cancellation.Dispose();
                    lock (gate)
                    {
                        activeJob = null;
                        CompleteIdleIfNecessary();
                    }
                }
            }
        }
        catch (OperationCanceledException) when (shutdown.IsCancellationRequested)
        {
        }
    }

    private void ResetIdleIfNecessary()
    {
        if (idle.Task.IsCompleted)
        {
            idle = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }

    private void CompleteIdleIfNecessary()
    {
        if (queuedJobs.Count == 0 && activeJob is null)
        {
            idle.TrySetResult();
        }
    }

    private static TaskCompletionSource CompletedIdleSignal()
    {
        var signal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        signal.SetResult();
        return signal;
    }
}
