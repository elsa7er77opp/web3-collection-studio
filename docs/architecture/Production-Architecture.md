# Web3 Collection Studio — Production Architecture

**Status:** Architecture baseline for a clean second CyberBank desktop product.  
**Platform:** Windows desktop, local-first.  
**Product purpose:** Create, validate, render, and export layered digital collections without wallet connections, minting, cloud asset storage, or financial claims.

## 1. Product Decision

Web3 Collection Studio will be a separate **Avalonia/.NET desktop application** with a framework-independent collection engine. It will use the same CyberBank workstation grammar as Secure Code Shield—compact native chrome, labeled navigation, resizable work areas, inspector, queue, and persistent status—but applies a violet/indigo creative identity.

The first commercial-quality release supports three collection profiles through one local engine:

| Profile | Default canvas | Primary use | Shared engine behavior |
|---|---:|---|---|
| **PFP** | 1024 × 1024 | Trait-based avatar collections | Ordered PNG layers, weights, compatibility rules, duplicate prevention, metadata attributes |
| **Badge / Gaming Item** | Custom portrait, landscape, or square preset | Achievement badges, items, cosmetics | Same layer/rule/render engine, profile-specific dimensions and trait vocabulary |
| **Digital Art** | Fully custom canvas | Limited art editions and layered campaigns | Same preview, preflight, queue, backup, and metadata export model |

> **Product boundary:** The application prepares local assets and metadata for a later minting flow performed by the customer. It does not connect to wallets, sign transactions, estimate prices, promise rarity value, publish a collection, or store customer assets in the cloud.

## 2. Core Workflow

```text
Create local project
  → select profile and canvas
  → import PNG layer folders
  → define traits, weights, and rules
  → preview / compare combinations
  → run compatibility + duplicate + metadata preflight
  → enqueue local PNG rendering
  → export image folder + OpenSea-style metadata + project manifest
```

Every destructive or long-running action has a visible queue state, progress, cancellation point, recoverable failure state, and local audit event. Asset files stay in their chosen local project folder; the studio records metadata and safe hashes rather than moving source assets into a cloud or opaque internal format.

## 3. Repository Structure

```text
web3-collection-studio/
├── Web3CollectionStudio.slnx
├── docs/
│   ├── architecture/
│   ├── plans/
│   ├── release/
│   └── runbooks/
├── src/
│   ├── Web3CollectionStudio.Domain/
│   │   ├── Collections/
│   │   ├── Layers/
│   │   ├── Traits/
│   │   ├── Rules/
│   │   ├── Rendering/
│   │   └── Metadata/
│   ├── Web3CollectionStudio.Application/
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Queue/
│   │   └── Preflight/
│   ├── Web3CollectionStudio.Contracts/
│   │   ├── Files/
│   │   ├── Storage/
│   │   ├── Rendering/
│   │   └── Export/
│   ├── Web3CollectionStudio.Infrastructure/
│   │   ├── LocalFiles/
│   │   ├── ProjectStorage/
│   │   ├── Rendering/
│   │   ├── Export/
│   │   └── Backup/
│   └── Web3CollectionStudio.Desktop/
│       ├── Views/
│       ├── ViewModels/
│       ├── Controls/
│       ├── Themes/
│       └── Assets/
├── tests/
│   ├── Web3CollectionStudio.Domain.Tests/
│   ├── Web3CollectionStudio.Application.Tests/
│   ├── Web3CollectionStudio.Infrastructure.Tests/
│   └── Web3CollectionStudio.Desktop.Tests/
└── installer/
```

Dependencies point inward. The domain layer cannot import Avalonia, image codecs, SQLite, or direct filesystem APIs. The desktop shell only calls typed application operations and never handles rendering buffers, project files, or export paths itself.

## 4. Domain Model

| Entity | Responsibility | Sensitive or performance rule |
|---|---|---|
| `CollectionProject` | Profile, canvas, local root, manifest version, display identity | Stores a canonical local root only; never uploads asset data |
| `CanvasSpec` | Width, height, background mode, profile preset | Validates bounds before asset import or render |
| `Layer` | Draw order, visibility, source directory, trait group | Source files must match supported formats and canvas policy |
| `TraitOption` | Display name, weight, asset reference, trait value | Weight is non-negative; identifier is stable and unique inside a layer |
| `CompatibilityRule` | Require/exclude relations between trait options | Rule graph must be validated before render |
| `GenerationPlan` | Requested count, deterministic seed, uniqueness policy | Contains only IDs/weights—no image bytes |
| `RenderJob` | Queued/running/cancelling/cancelled/completed/failed lifecycle | Bounded queue, cancellation token, no UI-thread rendering |
| `RenderedOutput` | Number, trait selection fingerprint, output relative path, content hash | Enables duplicate prevention without loading historic image data |
| `MetadataDocument` | Token name, description, relative image URI, attributes | Never contains absolute local paths or external wallet data |

## 5. Project File and Recovery

Every project is a directory selected by the user. The source asset tree remains visible and editable by the user. Studio state is stored in a versioned JSON manifest under `.collection-studio/project.json`; transient queue, cache, and history data use a local SQLite database under the application-data directory.

Before a manifest/schema migration, the application produces an atomic local backup. Startup checks manifest and database integrity. A corrupted state store is quarantined—not silently deleted—and the project can be reopened with source assets intact. Exports are written to new timestamped folders and never overwrite an existing completed export without an explicit user action.

## 6. Local Asset and Rendering Model

Only user-selected local directories are in scope. The engine allows supported raster files in the first release: PNG as the required render input, with optional JPEG preview import added only after explicit support. Unsupported, enormous, missing, or mismatched canvas assets are surfaced as preflight failures rather than silently scaled or ignored.

Rendering happens in an isolated background service with bounded concurrency. Each output is composed from selected trait options in layer order, written atomically to a temporary file, hashed, and then promoted to its final PNG name. A cancellation request is checked between jobs and at controlled image-composition boundaries. The engine never scans arbitrary folders, runs user code, or loads every source image into memory at once.

## 7. Rules, Rarity, and Preflight

The first ruleset supports `Require` and `Exclude` relations. For example, a trait can require a compatible body layer or exclude another trait. The preflight service validates missing assets, duplicate trait IDs, invalid weights, impossible rule combinations, over-requested unique output counts, output naming collisions, and metadata completeness.

Rarity analysis is explanatory rather than speculative. The application presents distribution and estimated combination availability based on current weights/rules. It does not call values “investment rankings,” predict prices, or guarantee a market outcome.

## 8. Metadata and Export

The metadata exporter writes one JSON document per rendered output using an OpenSea-style interoperable structure:

```json
{
  "name": "Collection #0001",
  "description": "Local collection export",
  "image": "images/0001.png",
  "attributes": [
    { "trait_type": "Background", "value": "Indigo" }
  ]
}
```

Metadata paths are relative to the export folder. The exporter does not create blockchain transactions, upload to IPFS, add a contract address, or publish any collection automatically.

## 9. Desktop Workstation

The main desktop shell uses midnight, deep indigo, and violet `#8B5CF6`; it must not reuse Secure Code Shield’s lime as a product accent. The sidebar includes Dashboard, Collections, Projects, Layers, Assets, Traits, Rules, Metadata, and Export History. The adjacent collection browser lists local projects and layer groups. The center switches between Canvas, Gallery, and Compare modes. The right inspector edits the selected layer, trait, asset, rule, or metadata field. The bottom dock contains layer order, rarity distribution, and render queue.

The violet tiled globe is limited to splash/loading and engine-status moments. All operating screens remain dense, calm, keyboard-accessible, and free of marketing-card layouts.

## 10. Test Strategy

The engine is developed test-first. Fixtures use synthetic transparent PNGs only; no user art, customer collections, wallets, or real project data enter tests.

| Layer | Required tests |
|---|---|
| Domain | Profile validation, canvas bounds, trait ID uniqueness, weights, rule compatibility, duplicate fingerprints |
| Application | Queue state, progress snapshots, cancellation, preflight decisions, collection profile defaults |
| Infrastructure | Local path containment, atomic manifest write, SQLite migration/backup, PNG composition, atomic export |
| Desktop | Initial profile selection, empty collection state, profile-aware navigation, render queue state, no wallet/minting routes |
| Performance | Bounded queue, canvas-size limit, batch rendering without unbounded image cache, lazy gallery thumbnails |

## 11. Delivery Phases

| Phase | Deliverable | Acceptance gate |
|---|---|---|
| Foundation | New solution, architecture, design spec, profile contracts | Scope excludes wallet/mint/cloud and core tests run |
| Project core | Collection manifest, profile defaults, layers, traits, rule validation, recovery | Domain and local-storage tests are green |
| Workstation | Violet/indigo desktop shell, browser, canvas, inspector, queue | Visual review and desktop smoke test |
| Render/export | PNG job engine, preflight, duplicate prevention, metadata/export history | Deterministic synthetic collection renders correctly |
| Windows Alpha | Installer, clean install, packaged version, release notes | Installer and installed EXE launch successfully |

## 12. Non-Negotiable Boundaries

The product will not include wallet connections, seed phrases, private keys, contract deployment, token minting, payment flows, marketplace automation, price forecasts, or investment advice in Version 1. Those are separate product and compliance decisions, not extensions of a local creative workstation.
