# Web3 Collection Studio — Four-Column Workstation Redesign

## Decision

The owner confirmed that Collection Studio must match the **information density, hierarchy, and perceived engineering quality** of Secure Code Shield’s four-column desktop workstation. This is a structural benchmark only. Collection Studio retains its violet/indigo creative identity, tiled-globe launch identity, and local collection workflow; it does not copy security terminology, lime styling, findings tables, or code-inspector content.

## Desktop composition

The authenticated window uses a compact 52px command strip, a 138px labeled global navigation rail, a 230px **Collection Explorer**, a dominant center workspace, and a 360px **Collection Inspector**. A persistent 28px status strip anchors local-engine, project, queue, and storage state. Cool indigo hairlines separate all elevations. The design is dense and calm: 8px spacing rhythm, compact 10–12px operational labels, 14px baseline text, and no oversized cards or marketing copy.

| Zone | Purpose | Initial empty-state content |
|---|---|---|
| Navigation | Fixed destinations for Dashboard, Collections, Projects, Layers, Assets, Traits, Rules, Metadata, and Export History. | Collections is visibly active; local-only status remains anchored at the bottom. |
| Collection Explorer | Project/profile context, local collection ledger, layer outline, and asset state. | Explicitly says no collection is open and points to the single `New collection` command. |
| Center Workspace | Canvas/Gallery/Compare command row, a compact collection telemetry row, preview surface, and lower rarity/queue split. | Empty canvas names the local profiles and explains that preview is separate from final render. |
| Inspector | Selected layer/trait details, metadata preview, preflight state, and disabled explanations. | Shows selected-state absence rather than fake trait or rarity data. |

## Operational states

The first implementation must visibly distinguish: no local collection, project created, missing PNG asset, invalid compatibility rule, preflight required, queued render, running render with progress/cancel, render failure, duplicate output block, and metadata export complete. Any unavailable data is labelled as local empty state; no fake collection metrics or sample customer content is shown.

## Acceptance criteria

The result is acceptable only when the workstation is recognizably a native desktop control surface at 1180px wide: fixed labeled navigation, separate explorer, dense command/status surfaces, a focused center work area, and a deep inspector. Violet appears as the active/creative accent rather than as a full-screen gradient. The existing local login, project creation, manifest persistence, preflight, and renderer interfaces remain intact.
