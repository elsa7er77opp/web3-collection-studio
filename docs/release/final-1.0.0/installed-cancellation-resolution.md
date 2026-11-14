# Installed Local Render Cancellation — Resolution

## Initial Observation

The first installed cancellation attempt clicked an always-enabled **Cancel local render** control after a fast one-layer render had already completed. A new PNG was present, but the queue did not report a cancellable job. This did not prove that a live cancellation produced an output; it exposed misleading command availability and an inadequate acceptance harness.

## TDD Correction

`Workstation_exposes_cancel_only_while_a_local_render_is_active` was extended first to require that cancellation is available for either a running job or a queued job, and unavailable when neither exists. It failed for the queued snapshot because `CanCancelLocalRender` followed `IsRendering` only. The minimal correction now derives command availability from:

```csharp
snapshot.IsRendering || snapshot.QueuedCount > 0
```

The Workstation binds the command directly to this state and shows **No active local render** when no cancellable job exists. The focused contract passed, and the post-fix full regression passed **74/74**.

## Installed Acceptance

An isolated `Final Cancellation QA` project was created entirely through the installed Workstation. It contains two local 4096×4096 PNG layers, ensuring a nontrivial composition workload. The current Final installer was used for all of the following evidence.

| Gate | Result |
|---|---|
| Idle command state | **PASS** — installed UI exposed `CancelButtonEnabled: false` and `No active local render`. |
| Cancellation request | **PASS** — the driver invoked Cancel only after the control became enabled. |
| Output integrity | **PASS** — `NewOutputFiles` was empty after cancellation. |
| Temp/partial output | **PASS** — no final PNG was added to the isolated output directory; composer tests additionally cover cancelled-before-work cleanup. |

The observed cancellation landed while the render was queued, which is a valid bounded-queue cancellation path. The local render audit therefore contains no `render_started` record for that cancelled queued job; no local work was begun and no output was committed.

## Evidence Files

- `cancel-availability-red.log`
- `cancel-availability-green.log`
- `cancel-queued-red.log`
- `cancel-queued-green.log`
- `post-queued-cancel-fix-full-regression.log`
- `installed-cancel-availability.json`
- `installed-cancellation-result.json`
