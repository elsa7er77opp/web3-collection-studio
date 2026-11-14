using Web3CollectionStudio.Application.Rendering;

namespace Web3CollectionStudio.Application.Tests;

public sealed class BoundedRenderQueueContractTests
{
    [Fact]
    public async Task Queue_limits_pending_work_and_cancels_a_queued_job_without_starting_it()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var queue = new BoundedRenderQueue(maxQueuedJobs: 1);

        var running = queue.Enqueue(async cancellationToken =>
        {
            started.SetResult();
            await release.Task.WaitAsync(cancellationToken);
        });
        await started.Task;
        var queued = queue.Enqueue(_ => Task.CompletedTask);

        Assert.False(queue.TryEnqueue(_ => Task.CompletedTask, out _));
        Assert.Equal(RenderJobStatus.Running, running.Status);
        Assert.Equal(RenderJobStatus.Queued, queued.Status);

        Assert.True(queue.Cancel(queued.Id));
        Assert.Equal(RenderJobStatus.Cancelled, queued.Status);

        release.SetResult();
        await queue.WhenIdleAsync();
        Assert.Equal(RenderJobStatus.Completed, running.Status);
    }

    [Fact]
    public async Task Queue_exposes_measurable_progress_for_the_running_job()
    {
        var progressReported = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var queue = new BoundedRenderQueue(maxQueuedJobs: 1);

        var job = queue.EnqueueWithContext(async context =>
        {
            context.ReportProgress(42);
            progressReported.SetResult();
            await release.Task.WaitAsync(context.CancellationToken);
        });

        await progressReported.Task;
        Assert.Equal(42, job.ProgressPercent);

        release.SetResult();
        await queue.WhenIdleAsync();
    }

    [Fact]
    public async Task Queue_exposes_an_idle_snapshot_for_a_workstation_without_render_jobs()
    {
        await using var queue = new BoundedRenderQueue(maxQueuedJobs: 1);

        var snapshot = queue.GetSnapshot();

        Assert.Equal(0, snapshot.QueuedCount);
        Assert.False(snapshot.IsRendering);
        Assert.Equal(0, snapshot.ActiveProgressPercent);
    }
}
